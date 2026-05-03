using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Nomad.Tools.ApiSurfaceValidator;

internal static class Program {
	public static int Main( string[] args ) {
		Options options = Options.Parse( args );
		if ( options.ShowHelp ) {
			Options.PrintHelp();
			return 0;
		}

		ColorLog.UseColor = !options.NoColor;

		try {
			ApiSurfaceValidator validator = new ApiSurfaceValidator( options );
			return validator.Run();
		} catch ( Exception exception ) {
			ColorLog.WriteLine( Severity.Error, "FATAL", exception.Message );
			return 2;
		}
	}
};

internal sealed class ApiSurfaceValidator {
	private static readonly Regex InternalsVisibleToRegex = new Regex( "InternalsVisibleTo\\s*\\(\\s*\"(?<friend>[^\"]+)\"\\s*\\)", RegexOptions.Compiled );

	private readonly Options _options;
	private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

	public ApiSurfaceValidator( Options options ) {
		_options = options;
	}

	public int Run() {
		ColorLog.WriteHeader( "Nomad.ApiSurfaceValidator" );

		string repoRoot = Path.GetFullPath( _options.RepoRoot );
		string sourceRoot = Path.GetFullPath( Path.Combine( repoRoot, _options.SourcePath ) );
		string testsRoot = Path.GetFullPath( Path.Combine( repoRoot, _options.TestsPath ) );

		if ( !Directory.Exists( sourceRoot ) ) {
			Add( Severity.Error, "API001", "Source directory was not found.", RelativePath( sourceRoot ) );
			return FlushAndExit();
		}

		List<ModuleInfo> modules = DiscoverModules( sourceRoot );
		if ( modules.Count == 0 ) {
			Add( Severity.Error, "API002", "No Nomad source modules were discovered.", RelativePath( sourceRoot ) );
			return FlushAndExit();
		}

		HashSet<string> moduleNames = new HashSet<string>( modules.Select( module => module.Name ), StringComparer.Ordinal );
		ValidateEngineTemplatesCompileAccess( modules );
		ValidateModules( modules, moduleNames, testsRoot );
		ValidateCrossModulePrivateNamespaceAccess( modules, moduleNames );

		return FlushAndExit();
	}

	private static List<ModuleInfo> DiscoverModules( string sourceRoot ) {
		List<ModuleInfo> modules = new List<ModuleInfo>();
		foreach ( string directory in Directory.EnumerateDirectories( sourceRoot ) ) {
			string name = Path.GetFileName( directory );
			if ( !name.StartsWith( "Nomad.", StringComparison.Ordinal ) ) {
				continue;
			}

			string projectPath = Path.Combine( directory, name + ".csproj" );
			if ( !File.Exists( projectPath ) ) {
				string[] projectFiles = Directory.GetFiles( directory, "*.csproj", SearchOption.TopDirectoryOnly );
				if ( projectFiles.Length == 0 ) {
					continue;
				}
				projectPath = projectFiles[0];
			}

			modules.Add( new ModuleInfo( name, directory, projectPath ) );
		}

		return modules.OrderBy( module => module.Name, StringComparer.Ordinal ).ToList();
	}

	private void ValidateModules( List<ModuleInfo> modules, HashSet<string> moduleNames, string testsRoot ) {
		for ( int i = 0; i < modules.Count; i++ ) {
			ModuleInfo module = modules[i];
			if ( !string.IsNullOrWhiteSpace( _options.ModuleName ) && !string.Equals( module.Name, _options.ModuleName, StringComparison.Ordinal ) ) {
				continue;
			}

			if ( string.Equals( module.Name, "Nomad.Core", StringComparison.Ordinal ) ) {
				ValidateCoreModule( module );
				ValidateInternalsVisibleTo( module, testsRoot );
				continue;
			}

			if ( string.Equals( module.Name, "Nomad.EngineTemplates", StringComparison.Ordinal ) ) {
				ValidateEngineTemplatesModule( module );
				ValidateInternalsVisibleTo( module, testsRoot );
				continue;
			}

			ValidatePublicPrivateFolders( module );
			ValidatePrivateTopLevelTypes( module );
			ValidateInternalsVisibleTo( module, testsRoot );
		}
	}

	private void ValidateCoreModule( ModuleInfo module ) {
		string publicPath = Path.Combine( module.Directory, "Public" );
		string privatePath = Path.Combine( module.Directory, "Private" );

		if ( Directory.Exists( publicPath ) || Directory.Exists( privatePath ) ) {
			Add( Severity.Warning, "API010", "Nomad.Core is configured as the no-split foundation module; Public/Private folders are unexpected.", RelativePath( module.Directory ) );
		}
	}

	private void ValidatePublicPrivateFolders( ModuleInfo module ) {
		string publicPath = Path.Combine( module.Directory, "Public" );
		string privatePath = Path.Combine( module.Directory, "Private" );

		if ( !Directory.Exists( publicPath ) ) {
			Add( Severity.Warning, "API020", $"Module '{module.Name}' has no Public directory.", RelativePath( publicPath ) );
		}

		if ( !Directory.Exists( privatePath ) ) {
			Add( Severity.Warning, "API021", $"Module '{module.Name}' has no Private directory.", RelativePath( privatePath ) );
		}
	}

	private void ValidatePrivateTopLevelTypes( ModuleInfo module ) {
		string privatePath = Path.Combine( module.Directory, "Private" );
		if ( !Directory.Exists( privatePath ) ) {
			return;
		}

		foreach ( string file in EnumerateCSharpFiles( privatePath ) ) {
			ValidateTopLevelTypesAreExplicitInternal( module, file, requireAllInternal: false );
		}
	}

	private void ValidateEngineTemplatesModule( ModuleInfo module ) {
		string publicPath = Path.Combine( module.Directory, "Public" );
		if ( Directory.Exists( publicPath ) ) {
			Add( Severity.Error, "API030", "Nomad.EngineTemplates is an internal template/source-generated module and should not expose a Public directory.", RelativePath( publicPath ) );
		}

		foreach ( string file in EnumerateCSharpFiles( module.Directory ) ) {
			ValidateTopLevelTypesAreExplicitInternal( module, file, requireAllInternal: true );
		}
	}

	private void ValidateTopLevelTypesAreExplicitInternal( ModuleInfo module, string file, bool requireAllInternal ) {
		SyntaxTree tree = CSharpSyntaxTree.ParseText( File.ReadAllText( file ), path: file );
		CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
		IEnumerable<MemberDeclarationSyntax> topLevelMembers = root.DescendantNodes()
			.OfType<MemberDeclarationSyntax>()
			.Where( IsTopLevelTypeDeclaration );

		foreach ( MemberDeclarationSyntax member in topLevelMembers ) {
			SyntaxTokenList modifiers = GetModifiers( member );
			string typeName = GetTypeName( member );
			LocationInfo location = GetLocationInfo( member, file );

			bool isPublic = HasModifier( modifiers, SyntaxKind.PublicKeyword );
			bool isProtected = HasModifier( modifiers, SyntaxKind.ProtectedKeyword );
			bool isInternal = HasModifier( modifiers, SyntaxKind.InternalKeyword );
			bool isPrivate = HasModifier( modifiers, SyntaxKind.PrivateKeyword );

			if ( isPublic || isProtected ) {
				Add( Severity.Error, requireAllInternal ? "API031" : "API040", $"Top-level type '{typeName}' in '{module.Name}' must not be public/protected here; use explicit internal.", location.ToString() );
				continue;
			}

			if ( isPrivate ) {
				Add( Severity.Error, requireAllInternal ? "API032" : "API041", $"Top-level type '{typeName}' uses an invalid private modifier; use internal.", location.ToString() );
				continue;
			}

			if ( !isInternal ) {
				Add( Severity.Error, requireAllInternal ? "API033" : "API042", $"Top-level type '{typeName}' must explicitly use the internal keyword.", location.ToString() );
			}
		}
	}

	private void ValidateCrossModulePrivateNamespaceAccess( List<ModuleInfo> modules, HashSet<string> moduleNames ) {
		for ( int moduleIndex = 0; moduleIndex < modules.Count; moduleIndex++ ) {
			ModuleInfo currentModule = modules[moduleIndex];
			foreach ( string file in EnumerateCSharpFiles( currentModule.Directory ) ) {
				string text = File.ReadAllText( file );
				for ( int otherIndex = 0; otherIndex < modules.Count; otherIndex++ ) {
					ModuleInfo targetModule = modules[otherIndex];
					if ( string.Equals( currentModule.Name, targetModule.Name, StringComparison.Ordinal ) ) {
						continue;
					}

					string privateNamespace = targetModule.Name + ".Private";
					if ( text.Contains( privateNamespace, StringComparison.Ordinal ) ) {
						Add( Severity.Error, "API050", $"Module '{currentModule.Name}' references private namespace '{privateNamespace}'. Production modules may only reference their own Private namespace; tests are the exception.", RelativePath( file ) );
					}
				}
			}
		}
	}

	private void ValidateEngineTemplatesCompileAccess( List<ModuleInfo> modules ) {
		for ( int i = 0; i < modules.Count; i++ ) {
			ModuleInfo consumer = modules[i];
			ProjectInfo projectInfo = ProjectInfo.Load( consumer.ProjectPath );

			foreach ( string reference in projectInfo.EngineTemplateProjectReferences ) {
				if ( IsEngineTemplatesConsumerAllowed( consumer.Name ) ) {
					continue;
				}

				Add( Severity.Error, "API060", $"Module '{consumer.Name}' references Nomad.EngineTemplates. Only Nomad.EngineUtils.* modules may reference or compile EngineTemplates directly.", RelativePath( consumer.ProjectPath ) );
			}

			foreach ( string compileInclude in projectInfo.EngineTemplateCompileIncludes ) {
				if ( IsEngineTemplatesConsumerAllowed( consumer.Name ) ) {
					continue;
				}

				Add( Severity.Error, "API061", $"Module '{consumer.Name}' compiles files from Nomad.EngineTemplates. Only Nomad.EngineUtils.* modules may compile EngineTemplates directly.", RelativePath( consumer.ProjectPath ) );
			}
		}
	}

	private static bool IsEngineTemplatesConsumerAllowed( string moduleName ) {
		return moduleName.StartsWith( "Nomad.EngineUtils.", StringComparison.Ordinal );
	}

	private void ValidateInternalsVisibleTo( ModuleInfo module, string testsRoot ) {
		List<string> friends = new List<string>();
		foreach ( string file in EnumerateCSharpFiles( module.Directory ) ) {
			string text = File.ReadAllText( file );
			foreach ( Match match in InternalsVisibleToRegex.Matches( text ) ) {
				string friend = match.Groups["friend"].Value.Trim();
				if ( !string.IsNullOrWhiteSpace( friend ) ) {
					friends.Add( friend );
				}
			}
		}

		for ( int i = 0; i < friends.Count; i++ ) {
			string friend = friends[i];
			if ( IsAllowedFriendAssembly( module, friend ) ) {
				continue;
			}

			string expected = string.Equals( module.Name, "Nomad.EngineTemplates", StringComparison.Ordinal )
				? "Expected tests, Nomad.EngineUtils.* assemblies, plus DynamicProxyGenAssembly2 when needed."
				: "Expected tests only, plus DynamicProxyGenAssembly2 when needed.";
			Add( Severity.Warning, "API070", $"Module '{module.Name}' exposes internals to unexpected friend assembly '{friend}'. {expected}", RelativePath( module.Directory ) );
		}

		string dedicatedTestProject = Path.Combine( testsRoot, module.Name + ".Tests", module.Name + ".Tests.csproj" );
		if ( File.Exists( dedicatedTestProject ) && !friends.Any( friend => string.Equals( friend, module.Name + ".Tests", StringComparison.Ordinal ) ) ) {
			Add( Severity.Warning, "API071", $"Module '{module.Name}' has a dedicated test project but no InternalsVisibleTo for '{module.Name}.Tests'.", RelativePath( module.Directory ) );
		}
	}

	private static bool IsAllowedFriendAssembly( ModuleInfo module, string friend ) {
		string friendAssemblyName = GetFriendAssemblyName( friend );

		if ( string.Equals( friendAssemblyName, module.Name + ".Tests", StringComparison.Ordinal ) ) {
			return true;
		}

		if ( string.Equals( friendAssemblyName, "Nomad.Tests", StringComparison.Ordinal ) ) {
			return true;
		}

		if ( string.Equals( friendAssemblyName, "DynamicProxyGenAssembly2", StringComparison.Ordinal ) ) {
			return true;
		}

		if ( string.Equals( module.Name, "Nomad.EngineTemplates", StringComparison.Ordinal )
			&& friendAssemblyName.StartsWith( "Nomad.EngineUtils.", StringComparison.Ordinal ) ) {
			return true;
		}

		return false;
	}

	private static string GetFriendAssemblyName( string friend ) {
		int commaIndex = friend.IndexOf( ',' );
		if ( commaIndex < 0 ) {
			return friend.Trim();
		}

		return friend.Substring( 0, commaIndex ).Trim();
	}

	private static bool IsTopLevelTypeDeclaration( MemberDeclarationSyntax member ) {
		if ( member is BaseTypeDeclarationSyntax || member is DelegateDeclarationSyntax ) {
			SyntaxNode? parent = member.Parent;
			return parent is CompilationUnitSyntax
				|| parent is NamespaceDeclarationSyntax
				|| parent is FileScopedNamespaceDeclarationSyntax;
		}

		return false;
	}

	private static SyntaxTokenList GetModifiers( MemberDeclarationSyntax member ) {
		return member switch {
			BaseTypeDeclarationSyntax baseType => baseType.Modifiers,
			DelegateDeclarationSyntax delegateDeclaration => delegateDeclaration.Modifiers,
			_ => default
		};
	}

	private static string GetTypeName( MemberDeclarationSyntax member ) {
		return member switch {
			BaseTypeDeclarationSyntax baseType => baseType.Identifier.ValueText,
			DelegateDeclarationSyntax delegateDeclaration => delegateDeclaration.Identifier.ValueText,
			_ => "<unknown>"
		};
	}

	private static bool HasModifier( SyntaxTokenList modifiers, SyntaxKind kind ) {
		return modifiers.Any( token => token.IsKind( kind ) );
	}

	private LocationInfo GetLocationInfo( MemberDeclarationSyntax member, string file ) {
		FileLinePositionSpan span = member.GetLocation().GetLineSpan();
		return new LocationInfo( RelativePath( file ), span.StartLinePosition.Line + 1, span.StartLinePosition.Character + 1 );
	}

	private static IEnumerable<string> EnumerateCSharpFiles( string directory ) {
		if ( !Directory.Exists( directory ) ) {
			return Array.Empty<string>();
		}

		return Directory.EnumerateFiles( directory, "*.cs", SearchOption.AllDirectories )
			.Where( file => !IsExcludedPath( file ) );
	}

	private static bool IsExcludedPath( string file ) {
		string normalized = file.Replace( '\\', '/' );
		return normalized.Contains( "/bin/", StringComparison.OrdinalIgnoreCase )
			|| normalized.Contains( "/obj/", StringComparison.OrdinalIgnoreCase )
			|| normalized.Contains( "/Generated/", StringComparison.OrdinalIgnoreCase );
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
};

internal sealed class ModuleInfo {
	public string Name { get; }
	public string Directory { get; }
	public string ProjectPath { get; }

	public ModuleInfo( string name, string directory, string projectPath ) {
		Name = name;
		Directory = directory;
		ProjectPath = projectPath;
	}
};

internal sealed class ProjectInfo {
	public List<string> EngineTemplateProjectReferences { get; private init; } = new List<string>();
	public List<string> EngineTemplateCompileIncludes { get; private init; } = new List<string>();

	public static ProjectInfo Load( string projectPath ) {
		try {
			XDocument document = XDocument.Load( projectPath, LoadOptions.PreserveWhitespace );
			List<string> projectReferences = document.Descendants()
				.Where( element => element.Name.LocalName == "ProjectReference" )
				.Select( element => element.Attribute( "Include" )?.Value ?? string.Empty )
				.Where( value => value.Contains( "Nomad.EngineTemplates", StringComparison.OrdinalIgnoreCase ) )
				.ToList();

			List<string> compileIncludes = document.Descendants()
				.Where( element => element.Name.LocalName == "Compile" )
				.Select( element => element.Attribute( "Include" )?.Value ?? string.Empty )
				.Where( value => value.Contains( "Nomad.EngineTemplates", StringComparison.OrdinalIgnoreCase ) )
				.ToList();

			return new ProjectInfo {
				EngineTemplateProjectReferences = projectReferences,
				EngineTemplateCompileIncludes = compileIncludes
			};
		} catch {
			return new ProjectInfo();
		}
	}
};

internal sealed class Options {
	public string RepoRoot { get; private init; } = Directory.GetCurrentDirectory();
	public string SourcePath { get; private init; } = "Source";
	public string TestsPath { get; private init; } = "Tests";
	public string ModuleName { get; private init; } = string.Empty;
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
				case "--source":
					builder.SourcePath = RequireValue( args, ref i, arg );
					break;
				case "--tests":
					builder.TestsPath = RequireValue( args, ref i, arg );
					break;
				case "--module":
					builder.ModuleName = RequireValue( args, ref i, arg );
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
					throw new ArgumentException( $"Unknown argument '{arg}'." );
			}
		}

		return builder.Build();
	}

	public static void PrintHelp() {
		Console.WriteLine( "Nomad.ApiSurfaceValidator" );
		Console.WriteLine();
		Console.WriteLine( "Usage:" );
		Console.WriteLine( "  dotnet run --project Tools/Nomad.ApiSurfaceValidator -- [options]" );
		Console.WriteLine();
		Console.WriteLine( "Options:" );
		Console.WriteLine( "  --repo-root <path>        Repository root. Defaults to current directory." );
		Console.WriteLine( "  --source <path>           Source path relative to repo root. Defaults to Source." );
		Console.WriteLine( "  --tests <path>            Tests path relative to repo root. Defaults to Tests." );
		Console.WriteLine( "  --module <name>           Validate one module only." );
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
		public string SourcePath { get; set; } = "Source";
		public string TestsPath { get; set; } = "Tests";
		public string ModuleName { get; set; } = string.Empty;
		public bool WarningsAsErrors { get; set; }
		public bool NoColor { get; set; }
		public bool ShowHelp { get; set; }

		public Options Build() {
			return new Options {
				RepoRoot = RepoRoot,
				SourcePath = SourcePath,
				TestsPath = TestsPath,
				ModuleName = ModuleName,
				WarningsAsErrors = WarningsAsErrors,
				NoColor = NoColor,
				ShowHelp = ShowHelp
			};
		}
	}
};

internal readonly record struct Diagnostic( Severity Severity, string Rule, string Message, string? Location );

internal readonly record struct LocationInfo( string Path, int Line, int Column ) {
	public override string ToString() {
		return $"{Path}:{Line}:{Column}";
	}
};

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
