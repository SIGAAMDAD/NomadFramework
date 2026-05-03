using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace Nomad.Tools.ManifestValidator;

internal static class Program {
	public static int Main( string[] args ) {
		Options options = Options.Parse( args );
		if ( options.ShowHelp ) {
			Options.PrintHelp();
			return 0;
		}

		ColorLog.UseColor = !options.NoColor;

		try {
			ManifestValidator validator = new ManifestValidator( options );
			return validator.Run();
		} catch ( Exception exception ) {
			ColorLog.WriteLine( Severity.Error, "FATAL", exception.Message );
			return 2;
		}
	}
};

internal sealed class ManifestValidator {
	private readonly Options _options;
	private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

	public ManifestValidator( Options options ) {
		_options = options;
	}

	public int Run() {
		ColorLog.WriteHeader( "Nomad.ManifestValidator" );

		string repoRoot = Path.GetFullPath( _options.RepoRoot );
		string manifestPath = Path.GetFullPath( Path.Combine( repoRoot, _options.ManifestPath ) );
		string sourceRoot = Path.Combine( repoRoot, "Source" );
		string testsRoot = Path.Combine( repoRoot, "Tests" );

		if ( !File.Exists( manifestPath ) ) {
			Add( Severity.Error, "MANIFEST001", "Manifest file was not found.", manifestPath );
			return FlushAndExit();
		}
		if ( !Directory.Exists( sourceRoot ) ) {
			Add( Severity.Error, "MANIFEST002", "Source directory was not found.", sourceRoot );
			return FlushAndExit();
		}

		List<SubsystemInfo> subsystems = LoadManifest( manifestPath );
		Dictionary<string, SubsystemInfo> byName = ValidateManifestShape( subsystems );
		ValidateSourceModules( sourceRoot, subsystems, byName );
		ValidateSubsystems( repoRoot, sourceRoot, testsRoot, subsystems, byName );

		return FlushAndExit();
	}

	private List<SubsystemInfo> LoadManifest( string manifestPath ) {
		try {
			using FileStream stream = File.OpenRead( manifestPath );
			using JsonDocument document = JsonDocument.Parse( stream, new JsonDocumentOptions {
				AllowTrailingCommas = true,
				CommentHandling = JsonCommentHandling.Skip
			} );

			JsonElement root = document.RootElement;
			if ( !TryGetPropertyIgnoreCase( root, "Subsystems", out JsonElement subsystemsElement ) || subsystemsElement.ValueKind != JsonValueKind.Array ) {
				Add( Severity.Error, "MANIFEST010", "Manifest must contain a top-level Subsystems array.", manifestPath );
				return new List<SubsystemInfo>();
			}

			List<SubsystemInfo> subsystems = new List<SubsystemInfo>();
			int index = 0;
			foreach ( JsonElement subsystemElement in subsystemsElement.EnumerateArray() ) {
				index++;
				if ( subsystemElement.ValueKind != JsonValueKind.Object ) {
					Add( Severity.Error, "MANIFEST011", $"Subsystem entry #{index} must be an object.", manifestPath );
					continue;
				}

				subsystems.Add( SubsystemInfo.FromJson( subsystemElement, index ) );
			}
			return subsystems;
		} catch ( JsonException exception ) {
			Add( Severity.Error, "MANIFEST012", $"Manifest JSON is invalid: {exception.Message}", manifestPath );
			return new List<SubsystemInfo>();
		}
	}

	private Dictionary<string, SubsystemInfo> ValidateManifestShape( List<SubsystemInfo> subsystems ) {
		Dictionary<string, SubsystemInfo> byName = new Dictionary<string, SubsystemInfo>( StringComparer.Ordinal );
		HashSet<string> defines = new HashSet<string>( StringComparer.Ordinal );

		if ( subsystems.Count == 0 ) {
			Add( Severity.Error, "MANIFEST020", "Manifest does not define any subsystems.", _options.ManifestPath );
			return byName;
		}

		for ( int i = 0; i < subsystems.Count; i++ ) {
			SubsystemInfo subsystem = subsystems[i];
			string location = $"Subsystems[{subsystem.Index}]";

			if ( string.IsNullOrWhiteSpace( subsystem.Name ) ) {
				Add( Severity.Error, "MANIFEST021", "Subsystem is missing Name.", location );
				continue;
			}

			if ( !subsystem.Name.StartsWith( "Nomad.", StringComparison.Ordinal ) ) {
				Add( Severity.Warning, "MANIFEST022", $"Subsystem name '{subsystem.Name}' does not start with 'Nomad.'.", location );
			}

			if ( !byName.TryAdd( subsystem.Name, subsystem ) ) {
				Add( Severity.Error, "MANIFEST023", $"Duplicate subsystem name '{subsystem.Name}'.", location );
			}

			if ( !string.IsNullOrWhiteSpace( subsystem.Define ) && !defines.Add( subsystem.Define ) ) {
				Add( Severity.Error, "MANIFEST024", $"Duplicate preprocessor define '{subsystem.Define}'.", location );
			}

			if ( string.IsNullOrWhiteSpace( subsystem.Description ) ) {
				Add( Severity.Warning, "MANIFEST025", $"Subsystem '{subsystem.Name}' has no description.", location );
			} else if ( IsGenericDescription( subsystem ) ) {
				Add( Severity.Warning, "MANIFEST026", $"Subsystem '{subsystem.Name}' has a placeholder/generic description: '{subsystem.Description}'.", location );
			}

			if ( subsystem.Description.Contains( "FileSyste", StringComparison.OrdinalIgnoreCase ) ) {
				Add( Severity.Warning, "MANIFEST027", $"Subsystem '{subsystem.Name}' description appears to contain a typo: '{subsystem.Description}'.", location );
			}
		}

		for ( int i = 0; i < subsystems.Count; i++ ) {
			SubsystemInfo subsystem = subsystems[i];
			if ( string.IsNullOrWhiteSpace( subsystem.Name ) ) {
				continue;
			}

			for ( int j = 0; j < subsystem.DependsOn.Count; j++ ) {
				string dependency = subsystem.DependsOn[j];
				if ( string.Equals( dependency, subsystem.Name, StringComparison.Ordinal ) ) {
					Add( Severity.Error, "MANIFEST030", $"Subsystem '{subsystem.Name}' depends on itself.", subsystem.Name );
				} else if ( !byName.ContainsKey( dependency ) ) {
					Add( Severity.Error, "MANIFEST031", $"Subsystem '{subsystem.Name}' depends on unknown subsystem '{dependency}'.", subsystem.Name );
				}
			}
		}

		return byName;
	}

	private void ValidateSourceModules( string sourceRoot, List<SubsystemInfo> subsystems, Dictionary<string, SubsystemInfo> byName ) {
		HashSet<string> manifestNames = new HashSet<string>( subsystems.Select( subsystem => subsystem.Name ), StringComparer.Ordinal );
		foreach ( string moduleDirectory in Directory.EnumerateDirectories( sourceRoot ) ) {
			string moduleName = Path.GetFileName( moduleDirectory );
			if ( !moduleName.StartsWith( "Nomad.", StringComparison.Ordinal ) ) {
				continue;
			}

			string[] projectFiles = Directory.GetFiles( moduleDirectory, "*.csproj", SearchOption.TopDirectoryOnly );
			if ( projectFiles.Length == 0 ) {
				continue;
			}

			if ( !manifestNames.Contains( moduleName ) ) {
				Add( Severity.Warning, "MANIFEST040", $"Source module '{moduleName}' exists but is not listed in NomadSubsystems.json.", RelativePath( moduleDirectory ) );
			}
		}
	}

	private void ValidateSubsystems( string repoRoot, string sourceRoot, string testsRoot, List<SubsystemInfo> subsystems, Dictionary<string, SubsystemInfo> byName ) {
		for ( int i = 0; i < subsystems.Count; i++ ) {
			SubsystemInfo subsystem = subsystems[i];
			if ( string.IsNullOrWhiteSpace( subsystem.Name ) ) {
				continue;
			}

			string moduleDirectory = Path.Combine( sourceRoot, subsystem.Name );
			string projectPath = Path.Combine( moduleDirectory, subsystem.Name + ".csproj" );
			string readmePath = Path.Combine( moduleDirectory, "README.md" );
			string roadmapPath = Path.Combine( moduleDirectory, "ROADMAP.md" );
			string testProjectPath = Path.Combine( testsRoot, subsystem.Name + ".Tests", subsystem.Name + ".Tests.csproj" );

			if ( !Directory.Exists( moduleDirectory ) ) {
				Add( Severity.Error, "MANIFEST050", $"Subsystem '{subsystem.Name}' has no matching Source/{subsystem.Name} directory.", RelativePath( moduleDirectory ) );
				continue;
			}

			if ( !File.Exists( projectPath ) ) {
				string[] projectFiles = Directory.GetFiles( moduleDirectory, "*.csproj", SearchOption.TopDirectoryOnly );
				if ( projectFiles.Length == 1 ) {
					Add( Severity.Warning, "MANIFEST051", $"Subsystem '{subsystem.Name}' project file is named '{Path.GetFileName( projectFiles[0] )}' instead of '{subsystem.Name}.csproj'.", RelativePath( projectFiles[0] ) );
					projectPath = projectFiles[0];
				} else {
					Add( Severity.Error, "MANIFEST052", $"Subsystem '{subsystem.Name}' has no matching .csproj file.", RelativePath( moduleDirectory ) );
					continue;
				}
			}

			ProjectInfo projectInfo = ProjectInfo.Load( projectPath );
			if ( string.IsNullOrWhiteSpace( projectInfo.PackageId ) ) {
				Add( Severity.Warning, "MANIFEST053", $"Subsystem '{subsystem.Name}' project has no PackageId.", RelativePath( projectPath ) );
			} else if ( !string.IsNullOrWhiteSpace( subsystem.PackageId ) && !string.Equals( subsystem.PackageId, projectInfo.PackageId, StringComparison.Ordinal ) ) {
				Add( Severity.Error, "MANIFEST054", $"Subsystem '{subsystem.Name}' manifest PackageId '{subsystem.PackageId}' does not match project PackageId '{projectInfo.PackageId}'.", RelativePath( projectPath ) );
			}

			if ( !File.Exists( readmePath ) ) {
				Add( Severity.Warning, "MANIFEST055", $"Subsystem '{subsystem.Name}' has no module README.md.", RelativePath( readmePath ) );
			}

			if ( !File.Exists( roadmapPath ) ) {
				Add( Severity.Warning, "MANIFEST056", $"Subsystem '{subsystem.Name}' has no module ROADMAP.md.", RelativePath( roadmapPath ) );
			}

			if ( !File.Exists( testProjectPath ) && !string.Equals( subsystem.Name, "Nomad.EngineTemplates", StringComparison.Ordinal ) ) {
				Add( Severity.Warning, "MANIFEST057", $"Subsystem '{subsystem.Name}' has no dedicated test project at Tests/{subsystem.Name}.Tests/.", RelativePath( testProjectPath ) );
			}

			for ( int dependencyIndex = 0; dependencyIndex < subsystem.DependsOn.Count; dependencyIndex++ ) {
				string dependency = subsystem.DependsOn[dependencyIndex];
				if ( !byName.ContainsKey( dependency ) ) {
					continue;
				}

				bool hasProjectReference = projectInfo.ProjectReferences.Any( reference => ReferenceLooksLikeModule( reference, dependency ) );
				if ( !hasProjectReference && !string.Equals( subsystem.Name, "Nomad.Core", StringComparison.Ordinal ) ) {
					Add( Severity.Warning, "MANIFEST058", $"Subsystem '{subsystem.Name}' declares dependency '{dependency}' but the project file does not appear to reference it.", RelativePath( projectPath ) );
				}
			}
		}
	}

	private static bool ReferenceLooksLikeModule( string reference, string moduleName ) {
		string normalizedReference = reference.Replace( '\\', '/' );
		return normalizedReference.Contains( $"/{moduleName}/{moduleName}.csproj", StringComparison.Ordinal )
			|| normalizedReference.EndsWith( $"{moduleName}.csproj", StringComparison.Ordinal );
	}

	private static bool IsGenericDescription( SubsystemInfo subsystem ) {
		string expected = "Subsystem " + subsystem.Name.Replace( "Nomad.", string.Empty, StringComparison.Ordinal ).Replace( '.', ' ' );
		return string.Equals( subsystem.Description.Trim(), expected, StringComparison.OrdinalIgnoreCase );
	}

	private int FlushAndExit() {
		int errorCount = _diagnostics.Count( diagnostic => diagnostic.Severity == Severity.Error );
		int warningCount = _diagnostics.Count( diagnostic => diagnostic.Severity == Severity.Warning );
		if ( _options.WarningsAsErrors ) {
			errorCount += warningCount;
		}

		for ( int i = 0; i < _diagnostics.Count; i++ ) {
			Diagnostic diagnostic = _diagnostics[i];
			ColorLog.WriteLine( diagnostic.Severity, diagnostic.Rule, diagnostic.Message, diagnostic.Location );
		}

		ColorLog.WriteSummary( errorCount, warningCount, _diagnostics.Count );
		return errorCount == 0 ? 0 : 1;
	}

	private void Add( Severity severity, string rule, string message, string? location = null ) {
		_diagnostics.Add( new Diagnostic( severity, rule, message, location ) );
	}

	private string RelativePath( string path ) {
		try {
			return Path.GetRelativePath( _options.RepoRoot, path );
		} catch {
			return path;
		}
	}

	private static bool TryGetPropertyIgnoreCase( JsonElement element, string name, out JsonElement value ) {
		foreach ( JsonProperty property in element.EnumerateObject() ) {
			if ( string.Equals( property.Name, name, StringComparison.OrdinalIgnoreCase ) ) {
				value = property.Value;
				return true;
			}
		}

		value = default;
		return false;
	}
};

internal sealed class SubsystemInfo {
	public int Index { get; private init; }
	public string Name { get; private init; } = string.Empty;
	public string Define { get; private init; } = string.Empty;
	public string Description { get; private init; } = string.Empty;
	public string PackageId { get; private init; } = string.Empty;
	public List<string> DependsOn { get; private init; } = new List<string>();
	public bool? UnityCompatible { get; private init; }
	public bool? DefaultEnabled { get; private init; }

	public static SubsystemInfo FromJson( JsonElement element, int index ) {
		return new SubsystemInfo {
			Index = index,
			Name = GetString( element, "Name" ),
			Define = GetString( element, "Define" ),
			Description = GetString( element, "Description" ),
			PackageId = GetString( element, "PackageId" ),
			DependsOn = GetStringArray( element, "DependsOn" ),
			UnityCompatible = GetBoolean( element, "UnityCompatible" ),
			DefaultEnabled = GetBoolean( element, "DefaultEnabled" )
		};
	}

	private static string GetString( JsonElement element, string name ) {
		if ( !TryGetPropertyIgnoreCase( element, name, out JsonElement value ) || value.ValueKind == JsonValueKind.Null ) {
			return string.Empty;
		}
		return value.ValueKind == JsonValueKind.String ? value.GetString() ?? string.Empty : value.ToString();
	}

	private static bool? GetBoolean( JsonElement element, string name ) {
		if ( !TryGetPropertyIgnoreCase( element, name, out JsonElement value ) || value.ValueKind == JsonValueKind.Null ) {
			return null;
		}
		if ( value.ValueKind == JsonValueKind.True ) {
			return true;
		}
		if ( value.ValueKind == JsonValueKind.False ) {
			return false;
		}
		return null;
	}

	private static List<string> GetStringArray( JsonElement element, string name ) {
		List<string> values = new List<string>();
		if ( !TryGetPropertyIgnoreCase( element, name, out JsonElement value ) || value.ValueKind != JsonValueKind.Array ) {
			return values;
		}

		foreach ( JsonElement item in value.EnumerateArray() ) {
			if ( item.ValueKind == JsonValueKind.String ) {
				string? text = item.GetString();
				if ( !string.IsNullOrWhiteSpace( text ) ) {
					values.Add( text );
				}
			}
		}
		return values;
	}

	private static bool TryGetPropertyIgnoreCase( JsonElement element, string name, out JsonElement value ) {
		foreach ( JsonProperty property in element.EnumerateObject() ) {
			if ( string.Equals( property.Name, name, StringComparison.OrdinalIgnoreCase ) ) {
				value = property.Value;
				return true;
			}
		}

		value = default;
		return false;
	}
};

internal sealed class ProjectInfo {
	public string PackageId { get; private init; } = string.Empty;
	public List<string> ProjectReferences { get; private init; } = new List<string>();

	public static ProjectInfo Load( string projectPath ) {
		try {
			XDocument document = XDocument.Load( projectPath );
			string packageId = document.Descendants()
				.FirstOrDefault( element => element.Name.LocalName == "PackageId" )?.Value.Trim() ?? string.Empty;

			List<string> references = document.Descendants()
				.Where( element => element.Name.LocalName == "ProjectReference" )
				.Select( element => element.Attribute( "Include" )?.Value ?? string.Empty )
				.Where( value => !string.IsNullOrWhiteSpace( value ) )
				.ToList();

			return new ProjectInfo {
				PackageId = packageId,
				ProjectReferences = references
			};
		} catch {
			return new ProjectInfo();
		}
	}
};

internal sealed class Options {
	public string RepoRoot { get; private init; } = Directory.GetCurrentDirectory();
	public string ManifestPath { get; private init; } = "NomadSubsystems.json";
	public bool WarningsAsErrors { get; private init; }
	public bool NoColor { get; private init; }
	public bool ShowHelp { get; private init; }

	public static Options Parse( string[] args ) {
		OptionsBuilder builder = new OptionsBuilder();
		for ( int i = 0; i < args.Length; i++ ) {
			string arg = args[i];
			switch ( arg ) {
				case "--repo-root":
					builder.RepoRoot = RequireValue( args, ref i, arg );
					break;
				case "--manifest":
					builder.ManifestPath = RequireValue( args, ref i, arg );
					break;
				case "--warnings-as-errors":
					builder.WarningsAsErrors = true;
					break;
				case "--no-color":
					builder.NoColor = true;
					break;
				case "--help":
				case "-h":
					builder.ShowHelp = true;
					break;
				default:
					if ( !arg.StartsWith( "-", StringComparison.Ordinal ) ) {
						builder.ManifestPath = arg;
					} else {
						throw new ArgumentException( $"Unknown argument '{arg}'." );
					}
					break;
			}
		}

		return builder.Build();
	}

	public static void PrintHelp() {
		Console.WriteLine( "Nomad.ManifestValidator" );
		Console.WriteLine();
		Console.WriteLine( "Usage:" );
		Console.WriteLine( "  dotnet run --project Tools/Nomad.ManifestValidator -- [options]" );
		Console.WriteLine();
		Console.WriteLine( "Options:" );
		Console.WriteLine( "  --repo-root <path>        Repository root. Defaults to current directory." );
		Console.WriteLine( "  --manifest <path>         Manifest path relative to repo root. Defaults to NomadSubsystems.json." );
		Console.WriteLine( "  --warnings-as-errors      Treat warnings as errors." );
		Console.WriteLine( "  --no-color                Disable ANSI color output." );
		Console.WriteLine( "  --help                    Show help." );
	}

	private static string RequireValue( string[] args, ref int index, string option ) {
		if ( index + 1 >= args.Length ) {
			throw new ArgumentException( $"Option '{option}' requires a value." );
		}
		index++;
		return args[index];
	}

	private sealed class OptionsBuilder {
		public string RepoRoot { get; set; } = Directory.GetCurrentDirectory();
		public string ManifestPath { get; set; } = "NomadSubsystems.json";
		public bool WarningsAsErrors { get; set; }
		public bool NoColor { get; set; }
		public bool ShowHelp { get; set; }

		public Options Build() {
			return new Options {
				RepoRoot = RepoRoot,
				ManifestPath = ManifestPath,
				WarningsAsErrors = WarningsAsErrors,
				NoColor = NoColor,
				ShowHelp = ShowHelp
			};
		}
	}
};

internal readonly record struct Diagnostic( Severity Severity, string Rule, string Message, string? Location );

internal enum Severity {
	Info,
	Warning,
	Error
};

internal static class ColorLog {
	public static bool UseColor { get; set; } = true;

	private const string RESET = "\u001b[0m";
	private const string RED = "\u001b[31m";
	private const string GREEN = "\u001b[32m";
	private const string YELLOW = "\u001b[33m";
	private const string CYAN = "\u001b[36m";
	private const string GRAY = "\u001b[90m";
	private const string BOLD = "\u001b[1m";

	public static void WriteHeader( string text ) {
		Console.WriteLine( Colorize( BOLD + CYAN, text ) );
		Console.WriteLine( Colorize( GRAY, new string( '-', text.Length ) ) );
	}

	public static void WriteLine( Severity severity, string rule, string message, string? location = null ) {
		string label = severity switch {
			Severity.Error => Colorize( RED, "error" ),
			Severity.Warning => Colorize( YELLOW, "warning" ),
			_ => Colorize( CYAN, "info" )
		};

		Console.Write( $"{label} {Colorize( GRAY, rule )}: {message}" );
		if ( !string.IsNullOrWhiteSpace( location ) ) {
			Console.Write( Colorize( GRAY, $" [{location}]" ) );
		}
		Console.WriteLine();
	}

	public static void WriteSummary( int errors, int warnings, int total ) {
		Console.WriteLine();
		string result = errors == 0 ? Colorize( GREEN, "PASS" ) : Colorize( RED, "FAIL" );
		Console.WriteLine( $"{result}: {total} diagnostics, {Colorize( RED, errors.ToString() )} errors, {Colorize( YELLOW, warnings.ToString() )} warnings" );
	}

	private static string Colorize( string color, string text ) {
		return UseColor ? color + text + RESET : text;
	}
};
