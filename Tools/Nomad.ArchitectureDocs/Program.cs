/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Nomad.Tools.ArchitectureDocs {
	/*
	===================================================================================

	Program

	===================================================================================
	*/
	/// <summary>
	/// Generates architecture documentation for the NomadFramework repository.
	/// </summary>

	internal static class Program {
		public const string DEFAULT_OUTPUT_DIRECTORY = "Documentation/Architecture/Generated";
		public const string MANIFEST_FILE_NAME = "NomadSubsystems.json";

		/*
		===============
		Main
		===============
		*/
		/// <summary>
		/// Tool entry point.
		/// </summary>
		/// <param name="args">Command-line arguments.</param>
		/// <returns>The process exit code.</returns>
		public static int Main( string[] args ) {
			try {
				GeneratorOptions options = GeneratorOptions.Parse( args );
				ArchitectureGenerator generator = new ArchitectureGenerator( options );
				ArchitectureModel model = generator.LoadModel();

				ArchitectureReportWriter writer = new ArchitectureReportWriter( options );
				writer.Write( model );

				Console.WriteLine( $"Architecture docs generated at '{Path.GetFullPath( options.OutputDirectory )}'." );
				return 0;
			} catch ( Exception exception ) {
				Console.Error.WriteLine( exception.Message );
				return 1;
			}
		}
	};

	/*
	===================================================================================

	GeneratorOptions

	===================================================================================
	*/
	/// <summary>
	/// Command-line options for the architecture documentation generator.
	/// </summary>

	internal sealed class GeneratorOptions {
		public string RepositoryRoot { get; private set; } = Directory.GetCurrentDirectory();
		public string OutputDirectory { get; private set; } = Program.DEFAULT_OUTPUT_DIRECTORY;
		public bool Strict { get; private set; } = false;
		public bool IncludeTypeInventory { get; private set; } = true;

		/*
		===============
		Parse
		===============
		*/
		/// <summary>
		/// Parses command-line options.
		/// </summary>
		/// <param name="args">The command-line argument array.</param>
		/// <returns>The parsed generator options.</returns>
		public static GeneratorOptions Parse( string[] args ) {
			GeneratorOptions options = new GeneratorOptions();

			for ( int i = 0; i < args.Length; i++ ) {
				string arg = args[i];
				switch ( arg ) {
					case "--repo-root":
					case "-r":
						options.RepositoryRoot = ReadValue( args, ref i, arg );
						break;
					case "--output":
					case "-o":
						options.OutputDirectory = ReadValue( args, ref i, arg );
						break;
					case "--strict":
						options.Strict = true;
						break;
					case "--no-type-inventory":
						options.IncludeTypeInventory = false;
						break;
					case "--help":
					case "-h":
						throw new InvalidOperationException( GetHelpText() );
					default:
						throw new ArgumentException( $"Unknown argument '{arg}'.{Environment.NewLine}{GetHelpText()}" );
				}
			}

			options.RepositoryRoot = Path.GetFullPath( options.RepositoryRoot );
			if ( !Directory.Exists( options.RepositoryRoot ) ) {
				throw new DirectoryNotFoundException( $"Repository root does not exist: '{options.RepositoryRoot}'." );
			}

			if ( !Path.IsPathRooted( options.OutputDirectory ) ) {
				options.OutputDirectory = Path.Combine( options.RepositoryRoot, options.OutputDirectory );
			}
			options.OutputDirectory = Path.GetFullPath( options.OutputDirectory );

			return options;
		}

		/*
		===============
		ReadValue
		===============
		*/
		/// <summary>
		/// Reads an option value from the command-line argument array.
		/// </summary>
		/// <param name="args">The full argument array.</param>
		/// <param name="index">The current index.</param>
		/// <param name="optionName">The option name.</param>
		/// <returns>The option value.</returns>
		private static string ReadValue( string[] args, ref int index, string optionName ) {
			if ( index + 1 >= args.Length ) {
				throw new ArgumentException( $"Missing value for '{optionName}'." );
			}
			index++;
			return args[index];
		}

		/*
		===============
		GetHelpText
		===============
		*/
		/// <summary>
		/// Gets the command-line help text.
		/// </summary>
		/// <returns>The help text.</returns>
		private static string GetHelpText() {
			return "Usage: dotnet run --project Tools/Nomad.ArchitectureDocs -- --repo-root . [--output Documentation/Architecture/Generated] [--strict] [--no-type-inventory]";
		}
	};

	/*
	===================================================================================

	ArchitectureGenerator

	===================================================================================
	*/
	/// <summary>
	/// Loads repository architecture information from manifests, project files, and source folders.
	/// </summary>

	internal sealed class ArchitectureGenerator {
		private readonly GeneratorOptions _options;

		/*
		===============
		ArchitectureGenerator
		===============
		*/
		/// <summary>
		/// Creates a new architecture model generator.
		/// </summary>
		/// <param name="options">The generator options.</param>
		public ArchitectureGenerator( GeneratorOptions options ) {
			_options = options ?? throw new ArgumentNullException( nameof( options ) );
		}

		/*
		===============
		LoadModel
		===============
		*/
		/// <summary>
		/// Loads the full architecture model.
		/// </summary>
		/// <returns>The loaded architecture model.</returns>
		public ArchitectureModel LoadModel() {
			string sourceDirectory = Path.Combine( _options.RepositoryRoot, "Source" );
			string testsDirectory = Path.Combine( _options.RepositoryRoot, "Tests" );
			string toolsDirectory = Path.Combine( _options.RepositoryRoot, "Tools" );
			string samplesDirectory = Path.Combine( _options.RepositoryRoot, "Samples" );

			List<SubsystemManifestEntry> manifestEntries = ManifestReader.Read( Path.Combine( _options.RepositoryRoot, Program.MANIFEST_FILE_NAME ) );
			List<ProjectInfo> sourceProjects = ProjectReader.ReadProjects( sourceDirectory, "Source" );
			List<ProjectInfo> testProjects = ProjectReader.ReadProjects( testsDirectory, "Tests" );
			List<ProjectInfo> toolProjects = ProjectReader.ReadProjects( toolsDirectory, "Tools" );
			List<ProjectInfo> sampleProjects = ProjectReader.ReadProjects( samplesDirectory, "Samples" );
			List<ModuleInfo> modules = ModuleScanner.Scan( sourceDirectory, manifestEntries, sourceProjects, testProjects, _options.IncludeTypeInventory );

			ArchitectureModel model = new ArchitectureModel(
				DateTimeOffset.UtcNow,
				_options.RepositoryRoot,
				manifestEntries,
				sourceProjects,
				testProjects,
				toolProjects,
				sampleProjects,
				modules
			);

			List<ArchitectureWarning> warnings = ArchitectureValidator.Validate( model );
			if ( _options.Strict && warnings.Any( warning => warning.Severity == ArchitectureWarningSeverity.Error ) ) {
				StringBuilder builder = new StringBuilder();
				builder.AppendLine( "Architecture validation failed:" );
				foreach ( ArchitectureWarning warning in warnings.Where( value => value.Severity == ArchitectureWarningSeverity.Error ) ) {
					builder.AppendLine( $"- {warning.Message}" );
				}
				throw new InvalidOperationException( builder.ToString() );
			}

			model.Warnings.AddRange( warnings );
			return model;
		}
	};

	/*
	===================================================================================

	ManifestReader

	===================================================================================
	*/
	/// <summary>
	/// Reads Nomad subsystem manifest files.
	/// </summary>

	internal static class ManifestReader {
		/*
		===============
		Read
		===============
		*/
		/// <summary>
		/// Reads subsystem manifest entries from disk.
		/// </summary>
		/// <param name="path">The manifest path.</param>
		/// <returns>The manifest entries.</returns>
		public static List<SubsystemManifestEntry> Read( string path ) {
			if ( !File.Exists( path ) ) {
				return new List<SubsystemManifestEntry>();
			}

			JsonSerializerOptions options = new JsonSerializerOptions {
				AllowTrailingCommas = true,
				ReadCommentHandling = JsonCommentHandling.Skip,
				PropertyNameCaseInsensitive = true
			};

			using FileStream stream = File.OpenRead( path );
			SubsystemManifest? manifest = JsonSerializer.Deserialize<SubsystemManifest>( stream, options );
			return manifest?.Subsystems ?? new List<SubsystemManifestEntry>();
		}
	};

	/*
	===================================================================================

	ProjectReader

	===================================================================================
	*/
	/// <summary>
	/// Reads .NET project metadata.
	/// </summary>

	internal static class ProjectReader {
		/*
		===============
		ReadProjects
		===============
		*/
		/// <summary>
		/// Reads all project files under a directory.
		/// </summary>
		/// <param name="directory">The directory to scan.</param>
		/// <param name="group">The project group name.</param>
		/// <returns>The project metadata list.</returns>
		public static List<ProjectInfo> ReadProjects( string directory, string group ) {
			if ( !Directory.Exists( directory ) ) {
				return new List<ProjectInfo>();
			}

			string[] projectFiles = Directory.GetFiles( directory, "*.csproj", SearchOption.AllDirectories );
			List<ProjectInfo> projects = new List<ProjectInfo>( projectFiles.Length );
			for ( int i = 0; i < projectFiles.Length; i++ ) {
				projects.Add( ReadProject( projectFiles[i], group ) );
			}
			projects.Sort( static ( left, right ) => StringComparer.OrdinalIgnoreCase.Compare( left.Name, right.Name ) );
			return projects;
		}

		/*
		===============
		ReadProject
		===============
		*/
		/// <summary>
		/// Reads one project file.
		/// </summary>
		/// <param name="path">The project path.</param>
		/// <param name="group">The project group name.</param>
		/// <returns>The project metadata.</returns>
		private static ProjectInfo ReadProject( string path, string group ) {
			XDocument document = XDocument.Load( path, LoadOptions.PreserveWhitespace );
			XElement root = document.Root ?? throw new InvalidDataException( $"Project file '{path}' has no root element." );
			XElement[] propertyGroups = root.Elements( "PropertyGroup" ).ToArray();

			string name = Path.GetFileNameWithoutExtension( path );
			string? packageId = GetProperty( propertyGroups, "PackageId" );
			string? version = GetProperty( propertyGroups, "Version" );
			string? targetFramework = GetProperty( propertyGroups, "TargetFramework" );
			string? targetFrameworks = GetProperty( propertyGroups, "TargetFrameworks" );
			string? isPackable = GetProperty( propertyGroups, "IsPackable" );
			string? generatePackageOnBuild = GetProperty( propertyGroups, "GeneratePackageOnBuild" );
			string? isTrimmable = GetProperty( propertyGroups, "IsTrimmable" );
			string? allowUnsafeBlocks = GetProperty( propertyGroups, "AllowUnsafeBlocks" );
			string? nullable = GetProperty( propertyGroups, "Nullable" );
			string? langVersion = GetProperty( propertyGroups, "LangVersion" );

			List<string> projectReferences = root
				.Descendants( "ProjectReference" )
				.Select( element => NormalizeSlash( element.Attribute( "Include" )?.Value ?? String.Empty ) )
				.Where( static value => !String.IsNullOrWhiteSpace( value ) )
				.OrderBy( static value => value, StringComparer.OrdinalIgnoreCase )
				.ToList();

			List<PackageReferenceInfo> packageReferences = root
				.Descendants( "PackageReference" )
				.Select( static element => new PackageReferenceInfo(
					element.Attribute( "Include" )?.Value ?? String.Empty,
					element.Attribute( "Version" )?.Value ?? String.Empty
				) )
				.Where( static value => !String.IsNullOrWhiteSpace( value.Name ) )
				.OrderBy( static value => value.Name, StringComparer.OrdinalIgnoreCase )
				.ToList();

			return new ProjectInfo(
				name,
				path,
				group,
				packageId,
				version,
				SplitFrameworks( targetFrameworks, targetFramework ),
				ParseNullableBoolean( isPackable ),
				ParseNullableBoolean( generatePackageOnBuild ),
				ParseNullableBoolean( isTrimmable ),
				ParseNullableBoolean( allowUnsafeBlocks ),
				nullable,
				langVersion,
				projectReferences,
				packageReferences
			);
		}

		/*
		===============
		GetProperty
		===============
		*/
		/// <summary>
		/// Gets a project property value.
		/// </summary>
		/// <param name="propertyGroups">The project property groups.</param>
		/// <param name="name">The property name.</param>
		/// <returns>The property value, if present.</returns>
		private static string? GetProperty( XElement[] propertyGroups, string name ) {
			for ( int i = propertyGroups.Length - 1; i >= 0; i-- ) {
				XElement? element = propertyGroups[i].Elements( name ).LastOrDefault();
				if ( element != null && !String.IsNullOrWhiteSpace( element.Value ) ) {
					return element.Value.Trim();
				}
			}
			return null;
		}

		/*
		===============
		SplitFrameworks
		===============
		*/
		/// <summary>
		/// Splits project target frameworks.
		/// </summary>
		/// <param name="targetFrameworks">The TargetFrameworks property.</param>
		/// <param name="targetFramework">The TargetFramework property.</param>
		/// <returns>The target framework list.</returns>
		private static List<string> SplitFrameworks( string? targetFrameworks, string? targetFramework ) {
			string value = !String.IsNullOrWhiteSpace( targetFrameworks ) ? targetFrameworks! : targetFramework ?? String.Empty;
			return value
				.Split( ';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries )
				.OrderBy( static item => item, StringComparer.OrdinalIgnoreCase )
				.ToList();
		}

		/*
		===============
		ParseNullableBoolean
		===============
		*/
		/// <summary>
		/// Parses a nullable boolean project property.
		/// </summary>
		/// <param name="value">The property value.</param>
		/// <returns>The parsed boolean value.</returns>
		private static bool? ParseNullableBoolean( string? value ) {
			if ( String.IsNullOrWhiteSpace( value ) ) {
				return null;
			}
			if ( Boolean.TryParse( value, out bool result ) ) {
				return result;
			}
			return null;
		}

		/*
		===============
		NormalizeSlash
		===============
		*/
		/// <summary>
		/// Normalizes path separators.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>The normalized path.</returns>
		private static string NormalizeSlash( string path ) {
			return path.Replace( '\\', '/' );
		}
	};

	/*
	===================================================================================

	ModuleScanner

	===================================================================================
	*/
	/// <summary>
	/// Scans source modules and their public/private API boundaries.
	/// </summary>

	internal static class ModuleScanner {
		private static readonly Regex TypeRegex = new Regex(
			@"(?<visibility>public|internal|protected|private)?\s*(?<modifiers>sealed|abstract|static|readonly|partial|ref|unsafe|record|class|struct|interface|enum|delegate|event|new|\s)*\s*(?<kind>class|struct|interface|enum|delegate|record)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)",
			RegexOptions.Compiled | RegexOptions.CultureInvariant
		);

		private static readonly Regex NamespaceRegex = new Regex(
			@"namespace\s+(?<name>[A-Za-z_][A-Za-z0-9_.]*)",
			RegexOptions.Compiled | RegexOptions.CultureInvariant
		);

		private static readonly Regex UsingPrivateRegex = new Regex(
			@"using\s+Nomad\.[A-Za-z0-9_.]*\.Private(\.|;)",
			RegexOptions.Compiled | RegexOptions.CultureInvariant
		);

		private static readonly Regex InternalsVisibleToRegex = new Regex(
			@"InternalsVisibleTo\s*\(\s*""(?<assembly>[^""]+)""\s*\)",
			RegexOptions.Compiled | RegexOptions.CultureInvariant
		);

		/*
		===============
		Scan
		===============
		*/
		/// <summary>
		/// Scans all modules under Source.
		/// </summary>
		/// <param name="sourceDirectory">The source directory.</param>
		/// <param name="manifestEntries">The manifest entries.</param>
		/// <param name="sourceProjects">The source project list.</param>
		/// <param name="testProjects">The test project list.</param>
		/// <param name="includeTypeInventory">Whether to include type inventories.</param>
		/// <returns>The scanned modules.</returns>
		public static List<ModuleInfo> Scan( string sourceDirectory, List<SubsystemManifestEntry> manifestEntries, List<ProjectInfo> sourceProjects, List<ProjectInfo> testProjects, bool includeTypeInventory ) {
			if ( !Directory.Exists( sourceDirectory ) ) {
				return new List<ModuleInfo>();
			}

			Dictionary<string, SubsystemManifestEntry> manifestByName = manifestEntries
				.GroupBy( static entry => entry.Name ?? String.Empty, StringComparer.OrdinalIgnoreCase )
				.Where( static group => !String.IsNullOrWhiteSpace( group.Key ) )
				.ToDictionary( static group => group.Key, static group => group.First(), StringComparer.OrdinalIgnoreCase );

			Dictionary<string, ProjectInfo> projectByName = sourceProjects.ToDictionary( static project => project.Name, StringComparer.OrdinalIgnoreCase );
			Dictionary<string, ProjectInfo> testProjectByName = testProjects.ToDictionary( static project => project.Name, StringComparer.OrdinalIgnoreCase );

			string[] moduleDirectories = Directory.GetDirectories( sourceDirectory, "Nomad.*", SearchOption.TopDirectoryOnly );
			List<ModuleInfo> modules = new List<ModuleInfo>( moduleDirectories.Length );

			for ( int i = 0; i < moduleDirectories.Length; i++ ) {
				string moduleDirectory = moduleDirectories[i];
				string moduleName = Path.GetFileName( moduleDirectory );
				manifestByName.TryGetValue( moduleName, out SubsystemManifestEntry? manifest );
				projectByName.TryGetValue( moduleName, out ProjectInfo? project );
				testProjectByName.TryGetValue( $"{moduleName}.Tests", out ProjectInfo? testProject );

				string publicDirectory = Path.Combine( moduleDirectory, "Public" );
				string privateDirectory = Path.Combine( moduleDirectory, "Private" );
				string propertiesDirectory = Path.Combine( moduleDirectory, "Properties" );

				List<SourceTypeInfo> publicTypes = includeTypeInventory ? ScanTypes( publicDirectory, true ) : new List<SourceTypeInfo>();
				List<SourceTypeInfo> privateTypes = includeTypeInventory ? ScanTypes( privateDirectory, false ) : new List<SourceTypeInfo>();
				List<string> privateBoundaryViolations = ScanPrivateBoundaryViolations( moduleDirectory );
				List<string> internalsVisibleTo = ScanInternalsVisibleTo( propertiesDirectory );

				modules.Add( new ModuleInfo(
					moduleName,
					moduleDirectory,
					manifest,
					project,
					testProject,
					Directory.Exists( publicDirectory ),
					Directory.Exists( privateDirectory ),
					File.Exists( Path.Combine( moduleDirectory, "README.md" ) ),
					File.Exists( Path.Combine( moduleDirectory, "ROADMAP.md" ) ),
					File.Exists( Path.Combine( moduleDirectory, "CHANGELOG.md" ) ),
					publicTypes,
					privateTypes,
					privateBoundaryViolations,
					internalsVisibleTo
				) );
			}

			modules.Sort( static ( left, right ) => StringComparer.OrdinalIgnoreCase.Compare( left.Name, right.Name ) );
			return modules;
		}

		/*
		===============
		ScanTypes
		===============
		*/
		/// <summary>
		/// Scans C# types under a directory.
		/// </summary>
		/// <param name="directory">The directory to scan.</param>
		/// <param name="isPublicDirectory">Whether the directory is Public.</param>
		/// <returns>The scanned type list.</returns>
		private static List<SourceTypeInfo> ScanTypes( string directory, bool isPublicDirectory ) {
			if ( !Directory.Exists( directory ) ) {
				return new List<SourceTypeInfo>();
			}

			string[] files = Directory.GetFiles( directory, "*.cs", SearchOption.AllDirectories );
			List<SourceTypeInfo> types = new List<SourceTypeInfo>();

			for ( int i = 0; i < files.Length; i++ ) {
				string file = files[i];
				string text = File.ReadAllText( file );
				string namespaceName = NamespaceRegex.Match( text ) is Match namespaceMatch && namespaceMatch.Success
					? namespaceMatch.Groups["name"].Value
					: String.Empty;

				MatchCollection matches = TypeRegex.Matches( RemoveComments( text ) );
				for ( int j = 0; j < matches.Count; j++ ) {
					Match match = matches[j];
					string visibility = match.Groups["visibility"].Success ? match.Groups["visibility"].Value : "internal";
					string kind = match.Groups["kind"].Value;
					string name = match.Groups["name"].Value;
					types.Add( new SourceTypeInfo( name, kind, visibility, namespaceName, file, isPublicDirectory ) );
				}
			}

			types.Sort( static ( left, right ) => StringComparer.OrdinalIgnoreCase.Compare( left.Name, right.Name ) );
			return types;
		}

		/*
		===============
		ScanPrivateBoundaryViolations
		===============
		*/
		/// <summary>
		/// Finds public files that reference private namespaces.
		/// </summary>
		/// <param name="moduleDirectory">The module directory.</param>
		/// <returns>The boundary violation paths.</returns>
		private static List<string> ScanPrivateBoundaryViolations( string moduleDirectory ) {
			string publicDirectory = Path.Combine( moduleDirectory, "Public" );
			if ( !Directory.Exists( publicDirectory ) ) {
				return new List<string>();
			}

			List<string> violations = new List<string>();
			string[] files = Directory.GetFiles( publicDirectory, "*.cs", SearchOption.AllDirectories );
			for ( int i = 0; i < files.Length; i++ ) {
				string text = File.ReadAllText( files[i] );
				if ( UsingPrivateRegex.IsMatch( text ) ) {
					violations.Add( files[i] );
				}
			}
			violations.Sort( StringComparer.OrdinalIgnoreCase );
			return violations;
		}

		/*
		===============
		ScanInternalsVisibleTo
		===============
		*/
		/// <summary>
		/// Scans InternalsVisibleTo declarations.
		/// </summary>
		/// <param name="propertiesDirectory">The properties directory.</param>
		/// <returns>The visible assemblies.</returns>
		private static List<string> ScanInternalsVisibleTo( string propertiesDirectory ) {
			if ( !Directory.Exists( propertiesDirectory ) ) {
				return new List<string>();
			}

			List<string> assemblies = new List<string>();
			string[] files = Directory.GetFiles( propertiesDirectory, "*.cs", SearchOption.AllDirectories );
			for ( int i = 0; i < files.Length; i++ ) {
				string text = File.ReadAllText( files[i] );
				MatchCollection matches = InternalsVisibleToRegex.Matches( text );
				for ( int j = 0; j < matches.Count; j++ ) {
					assemblies.Add( matches[j].Groups["assembly"].Value );
				}
			}

			return assemblies.Distinct( StringComparer.OrdinalIgnoreCase ).OrderBy( static value => value, StringComparer.OrdinalIgnoreCase ).ToList();
		}

		/*
		===============
		RemoveComments
		===============
		*/
		/// <summary>
		/// Removes simple C# comments before regex scanning.
		/// </summary>
		/// <param name="text">The source text.</param>
		/// <returns>The source text without comments.</returns>
		private static string RemoveComments( string text ) {
			string noBlockComments = Regex.Replace( text, @"/\*.*?\*/", String.Empty, RegexOptions.Singleline );
			return Regex.Replace( noBlockComments, @"//.*?$", String.Empty, RegexOptions.Multiline );
		}
	};

	/*
	===================================================================================

	ArchitectureValidator

	===================================================================================
	*/
	/// <summary>
	/// Validates repository architecture hygiene.
	/// </summary>

	internal static class ArchitectureValidator {
		/*
		===============
		Validate
		===============
		*/
		/// <summary>
		/// Validates an architecture model.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <returns>The validation warnings.</returns>
		public static List<ArchitectureWarning> Validate( ArchitectureModel model ) {
			List<ArchitectureWarning> warnings = new List<ArchitectureWarning>();

			ValidateManifest( model, warnings );
			ValidateModules( model, warnings );
			ValidateProjectReferences( model, warnings );

			warnings.Sort( static ( left, right ) => StringComparer.OrdinalIgnoreCase.Compare( left.Message, right.Message ) );
			return warnings;
		}

		/*
		===============
		ValidateManifest
		===============
		*/
		/// <summary>
		/// Validates manifest consistency.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <param name="warnings">The warning collection.</param>
		private static void ValidateManifest( ArchitectureModel model, List<ArchitectureWarning> warnings ) {
			foreach ( IGrouping<string, SubsystemManifestEntry> group in model.ManifestEntries.GroupBy( static entry => entry.Name ?? String.Empty, StringComparer.OrdinalIgnoreCase ) ) {
				if ( String.IsNullOrWhiteSpace( group.Key ) ) {
					warnings.Add( ArchitectureWarning.Error( "Manifest contains a subsystem with no Name." ) );
					continue;
				}

				if ( group.Count() > 1 ) {
					warnings.Add( ArchitectureWarning.Error( $"Manifest contains duplicate subsystem '{group.Key}'." ) );
				}
			}

			HashSet<string> manifestNames = model.ManifestEntries
				.Select( static entry => entry.Name ?? String.Empty )
				.Where( static value => !String.IsNullOrWhiteSpace( value ) )
				.ToHashSet( StringComparer.OrdinalIgnoreCase );

			foreach ( SubsystemManifestEntry entry in model.ManifestEntries ) {
				if ( entry.DependsOn == null ) {
					continue;
				}

				for ( int i = 0; i < entry.DependsOn.Count; i++ ) {
					string dependency = entry.DependsOn[i];
					if ( !manifestNames.Contains( dependency ) ) {
						warnings.Add( ArchitectureWarning.Error( $"Manifest subsystem '{entry.Name}' depends on unknown subsystem '{dependency}'." ) );
					}
				}
			}
		}

		/*
		===============
		ValidateModules
		===============
		*/
		/// <summary>
		/// Validates module folder and API boundary consistency.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <param name="warnings">The warning collection.</param>
		private static void ValidateModules( ArchitectureModel model, List<ArchitectureWarning> warnings ) {
			for ( int i = 0; i < model.Modules.Count; i++ ) {
				ModuleInfo module = model.Modules[i];

				if ( module.Project == null ) {
					warnings.Add( ArchitectureWarning.Error( $"Source module '{module.Name}' has no matching .csproj." ) );
				}

				if ( module.Manifest == null ) {
					warnings.Add( ArchitectureWarning.Warning( $"Source module '{module.Name}' is not listed in NomadSubsystems.json." ) );
				}

				if ( !module.HasPublicDirectory ) {
					warnings.Add( ArchitectureWarning.Warning( $"Source module '{module.Name}' has no Public directory." ) );
				}

				if ( !module.HasPrivateDirectory ) {
					warnings.Add( ArchitectureWarning.Info( $"Source module '{module.Name}' has no Private directory." ) );
				}

				if ( !module.HasReadme ) {
					warnings.Add( ArchitectureWarning.Warning( $"Source module '{module.Name}' has no module README.md." ) );
				}

				if ( !module.HasRoadmap ) {
					warnings.Add( ArchitectureWarning.Warning( $"Source module '{module.Name}' has no module ROADMAP.md." ) );
				}

				if ( module.TestProject == null ) {
					warnings.Add( ArchitectureWarning.Warning( $"Source module '{module.Name}' has no matching test project." ) );
				}

				foreach ( SourceTypeInfo type in module.PrivateTypes ) {
					if ( StringComparer.OrdinalIgnoreCase.Equals( type.Visibility, "public" ) ) {
						warnings.Add( ArchitectureWarning.Error( $"Private type '{type.Name}' in module '{module.Name}' is public." ) );
					}
				}

				for ( int j = 0; j < module.PrivateBoundaryViolations.Count; j++ ) {
					warnings.Add( ArchitectureWarning.Error( $"Public source file references a Private namespace: '{module.PrivateBoundaryViolations[j]}'." ) );
				}
			}
		}

		/*
		===============
		ValidateProjectReferences
		===============
		*/
		/// <summary>
		/// Validates project reference hygiene.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <param name="warnings">The warning collection.</param>
		private static void ValidateProjectReferences( ArchitectureModel model, List<ArchitectureWarning> warnings ) {
			Dictionary<string, ProjectInfo> sourceProjects = model.SourceProjects.ToDictionary( static project => project.Name, StringComparer.OrdinalIgnoreCase );

			for ( int i = 0; i < model.SourceProjects.Count; i++ ) {
				ProjectInfo project = model.SourceProjects[i];
				for ( int j = 0; j < project.ProjectReferences.Count; j++ ) {
					string reference = project.ProjectReferences[j];
					string projectName = Path.GetFileNameWithoutExtension( reference );
					if ( projectName.StartsWith( "Nomad.", StringComparison.OrdinalIgnoreCase ) && !sourceProjects.ContainsKey( projectName ) ) {
						warnings.Add( ArchitectureWarning.Warning( $"Project '{project.Name}' references '{reference}', but the referenced source project was not found during scanning." ) );
					}
				}
			}
		}
	};

	/*
	===================================================================================

	ArchitectureReportWriter

	===================================================================================
	*/
	/// <summary>
	/// Writes architecture documentation reports.
	/// </summary>

	internal sealed class ArchitectureReportWriter {
		private readonly GeneratorOptions _options;

		/*
		===============
		ArchitectureReportWriter
		===============
		*/
		/// <summary>
		/// Creates a new report writer.
		/// </summary>
		/// <param name="options">The generator options.</param>
		public ArchitectureReportWriter( GeneratorOptions options ) {
			_options = options ?? throw new ArgumentNullException( nameof( options ) );
		}

		/*
		===============
		Write
		===============
		*/
		/// <summary>
		/// Writes all reports.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		public void Write( ArchitectureModel model ) {
			Directory.CreateDirectory( _options.OutputDirectory );

			WriteFile( "index.md", WriteIndex( model ) );
			WriteFile( "Modules.md", WriteModules( model ) );
			WriteFile( "DependencyGraph.md", WriteDependencyGraph( model ) );
			WriteFile( "PackageMatrix.md", WritePackageMatrix( model ) );
			WriteFile( "TestMatrix.md", WriteTestMatrix( model ) );
			WriteFile( "PublicPrivateBoundary.md", WritePublicPrivateBoundary( model ) );
			WriteFile( "ValidationReport.md", WriteValidationReport( model ) );
		}

		/*
		===============
		WriteIndex
		===============
		*/
		/// <summary>
		/// Writes the generated docs index.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <returns>The markdown document.</returns>
		private static string WriteIndex( ArchitectureModel model ) {
			MarkdownBuilder markdown = CreateHeader( "Generated Architecture Reports", model );
			markdown.Line( "These files are generated from repository metadata. Do not edit generated reports by hand." );
			markdown.Line();
			markdown.Line( "## Reports" );
			markdown.Line();
			markdown.Bullet( "[Modules](Modules.md)" );
			markdown.Bullet( "[Dependency Graph](DependencyGraph.md)" );
			markdown.Bullet( "[Package Matrix](PackageMatrix.md)" );
			markdown.Bullet( "[Test Matrix](TestMatrix.md)" );
			markdown.Bullet( "[Public / Private Boundary](PublicPrivateBoundary.md)" );
			markdown.Bullet( "[Validation Report](ValidationReport.md)" );
			return markdown.ToString();
		}

		/*
		===============
		WriteModules
		===============
		*/
		/// <summary>
		/// Writes the module report.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <returns>The markdown document.</returns>
		private static string WriteModules( ArchitectureModel model ) {
			MarkdownBuilder markdown = CreateHeader( "Modules", model );
			markdown.TableHeader( "Module", "Description", "Depends On", "Unity", "Default", "Project", "Tests" );

			for ( int i = 0; i < model.Modules.Count; i++ ) {
				ModuleInfo module = model.Modules[i];
				SubsystemManifestEntry? manifest = module.Manifest;
				markdown.TableRow(
					Escape( module.Name ),
					Escape( manifest?.Description ?? "" ),
					Escape( JoinList( manifest?.DependsOn ) ),
					FormatBoolean( manifest?.UnityCompatible ),
					FormatBoolean( manifest?.DefaultEnabled ),
					module.Project != null ? "Yes" : "No",
					module.TestProject != null ? "Yes" : "No"
				);
			}

			return markdown.ToString();
		}

		/*
		===============
		WriteDependencyGraph
		===============
		*/
		/// <summary>
		/// Writes the dependency graph report.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <returns>The markdown document.</returns>
		private static string WriteDependencyGraph( ArchitectureModel model ) {
			MarkdownBuilder markdown = CreateHeader( "Dependency Graph", model );
			markdown.Line( "## Manifest Dependency Graph" );
			markdown.Line();
			markdown.Line( "```mermaid" );
			markdown.Line( "graph TD" );

			HashSet<string> emitted = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
			for ( int i = 0; i < model.ManifestEntries.Count; i++ ) {
				SubsystemManifestEntry entry = model.ManifestEntries[i];
				if ( String.IsNullOrWhiteSpace( entry.Name ) ) {
					continue;
				}

				string source = MermaidNode( entry.Name! );
				if ( emitted.Add( source ) ) {
					markdown.Line( $"    {source}[{entry.Name}]" );
				}

				if ( entry.DependsOn == null || entry.DependsOn.Count == 0 ) {
					continue;
				}

				for ( int j = 0; j < entry.DependsOn.Count; j++ ) {
					string dependency = entry.DependsOn[j];
					string dependencyNode = MermaidNode( dependency );
					if ( emitted.Add( dependencyNode ) ) {
						markdown.Line( $"    {dependencyNode}[{dependency}]" );
					}
					markdown.Line( $"    {source} --> {dependencyNode}" );
				}
			}

			markdown.Line( "```" );
			markdown.Line();
			markdown.Line( "## Project Reference Graph" );
			markdown.Line();
			markdown.Line( "```mermaid" );
			markdown.Line( "graph TD" );

			for ( int i = 0; i < model.SourceProjects.Count; i++ ) {
				ProjectInfo project = model.SourceProjects[i];
				string source = MermaidNode( project.Name );
				markdown.Line( $"    {source}[{project.Name}]" );
				for ( int j = 0; j < project.ProjectReferences.Count; j++ ) {
					string dependency = Path.GetFileNameWithoutExtension( project.ProjectReferences[j] );
					if ( String.IsNullOrWhiteSpace( dependency ) ) {
						continue;
					}
					markdown.Line( $"    {source} --> {MermaidNode( dependency )}[{dependency}]" );
				}
			}

			markdown.Line( "```" );
			return markdown.ToString();
		}

		/*
		===============
		WritePackageMatrix
		===============
		*/
		/// <summary>
		/// Writes the package matrix report.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <returns>The markdown document.</returns>
		private static string WritePackageMatrix( ArchitectureModel model ) {
			MarkdownBuilder markdown = CreateHeader( "Package Matrix", model );
			markdown.TableHeader( "Project", "PackageId", "Version", "TFMs", "Packable", "Generate Package", "Trimmable", "Unsafe", "Project References" );

			for ( int i = 0; i < model.SourceProjects.Count; i++ ) {
				ProjectInfo project = model.SourceProjects[i];
				markdown.TableRow(
					Escape( project.Name ),
					Escape( project.PackageId ?? "" ),
					Escape( project.Version ?? "" ),
					Escape( JoinList( project.TargetFrameworks ) ),
					FormatBoolean( project.IsPackable ),
					FormatBoolean( project.GeneratePackageOnBuild ),
					FormatBoolean( project.IsTrimmable ),
					FormatBoolean( project.AllowUnsafeBlocks ),
					Escape( JoinList( project.ProjectReferences.Select( Path.GetFileNameWithoutExtension ).Where( static value => value != null ).Cast<string>().ToList() ) )
				);
			}

			return markdown.ToString();
		}

		/*
		===============
		WriteTestMatrix
		===============
		*/
		/// <summary>
		/// Writes the test matrix report.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <returns>The markdown document.</returns>
		private static string WriteTestMatrix( ArchitectureModel model ) {
			MarkdownBuilder markdown = CreateHeader( "Test Matrix", model );
			markdown.TableHeader( "Module", "Test Project", "TFMs", "InternalsVisibleTo", "Status" );

			for ( int i = 0; i < model.Modules.Count; i++ ) {
				ModuleInfo module = model.Modules[i];
				ProjectInfo? testProject = module.TestProject;
				string expectedTestAssembly = $"{module.Name}.Tests";
				bool hasInternalsVisibleTo = module.InternalsVisibleTo.Contains( expectedTestAssembly, StringComparer.OrdinalIgnoreCase );
				string status = testProject == null ? "Missing test project" : hasInternalsVisibleTo ? "OK" : "No matching InternalsVisibleTo";

				markdown.TableRow(
					Escape( module.Name ),
					testProject?.Name ?? "",
					Escape( JoinList( testProject?.TargetFrameworks ) ),
					Escape( JoinList( module.InternalsVisibleTo ) ),
					Escape( status )
				);
			}

			return markdown.ToString();
		}

		/*
		===============
		WritePublicPrivateBoundary
		===============
		*/
		/// <summary>
		/// Writes the public/private boundary report.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <returns>The markdown document.</returns>
		private static string WritePublicPrivateBoundary( ArchitectureModel model ) {
			MarkdownBuilder markdown = CreateHeader( "Public / Private Boundary", model );
			markdown.TableHeader( "Module", "Public Dir", "Private Dir", "Public Types", "Private Types", "Public Private-Refs", "Public Private Types" );

			for ( int i = 0; i < model.Modules.Count; i++ ) {
				ModuleInfo module = model.Modules[i];
				int publicPrivateTypeCount = module.PrivateTypes.Count( static type => StringComparer.OrdinalIgnoreCase.Equals( type.Visibility, "public" ) );
				markdown.TableRow(
					Escape( module.Name ),
					module.HasPublicDirectory ? "Yes" : "No",
					module.HasPrivateDirectory ? "Yes" : "No",
					module.PublicTypes.Count.ToString( CultureInfo.InvariantCulture ),
					module.PrivateTypes.Count.ToString( CultureInfo.InvariantCulture ),
					module.PrivateBoundaryViolations.Count.ToString( CultureInfo.InvariantCulture ),
					publicPrivateTypeCount.ToString( CultureInfo.InvariantCulture )
				);
			}

			for ( int i = 0; i < model.Modules.Count; i++ ) {
				ModuleInfo module = model.Modules[i];
				markdown.Line();
				markdown.Line( $"## {module.Name}" );
				markdown.Line();
				markdown.Line( $"- Public directory: {( module.HasPublicDirectory ? "Yes" : "No" )}" );
				markdown.Line( $"- Private directory: {( module.HasPrivateDirectory ? "Yes" : "No" )}" );
				markdown.Line( $"- Public types: {module.PublicTypes.Count}" );
				markdown.Line( $"- Private types: {module.PrivateTypes.Count}" );
				markdown.Line( $"- Internals visible to: {Escape( JoinList( module.InternalsVisibleTo ) )}" );

				if ( module.PrivateBoundaryViolations.Count > 0 ) {
					markdown.Line();
					markdown.Line( "### Public files referencing Private namespaces" );
					markdown.Line();
					for ( int j = 0; j < module.PrivateBoundaryViolations.Count; j++ ) {
						markdown.Bullet( Escape( module.PrivateBoundaryViolations[j] ) );
					}
				}

				List<SourceTypeInfo> publicPrivateTypes = module.PrivateTypes.Where( static type => StringComparer.OrdinalIgnoreCase.Equals( type.Visibility, "public" ) ).ToList();
				if ( publicPrivateTypes.Count > 0 ) {
					markdown.Line();
					markdown.Line( "### Private types marked public" );
					markdown.Line();
					for ( int j = 0; j < publicPrivateTypes.Count; j++ ) {
						SourceTypeInfo type = publicPrivateTypes[j];
						markdown.Bullet( $"{type.Kind} `{type.Name}` in `{type.RelativePath}`" );
					}
				}
			}

			return markdown.ToString();
		}

		/*
		===============
		WriteValidationReport
		===============
		*/
		/// <summary>
		/// Writes the validation report.
		/// </summary>
		/// <param name="model">The architecture model.</param>
		/// <returns>The markdown document.</returns>
		private static string WriteValidationReport( ArchitectureModel model ) {
			MarkdownBuilder markdown = CreateHeader( "Validation Report", model );

			if ( model.Warnings.Count == 0 ) {
				markdown.Line( "No architecture warnings found." );
				return markdown.ToString();
			}

			markdown.TableHeader( "Severity", "Message" );
			for ( int i = 0; i < model.Warnings.Count; i++ ) {
				ArchitectureWarning warning = model.Warnings[i];
				markdown.TableRow( warning.Severity.ToString(), Escape( warning.Message ) );
			}

			return markdown.ToString();
		}

		/*
		===============
		WriteFile
		===============
		*/
		/// <summary>
		/// Writes a generated report file.
		/// </summary>
		/// <param name="fileName">The file name.</param>
		/// <param name="content">The markdown content.</param>
		private void WriteFile( string fileName, string content ) {
			string path = Path.Combine( _options.OutputDirectory, fileName );
			File.WriteAllText( path, content, new UTF8Encoding( false ) );
		}

		/*
		===============
		CreateHeader
		===============
		*/
		/// <summary>
		/// Creates a common generated report header.
		/// </summary>
		/// <param name="title">The report title.</param>
		/// <param name="model">The architecture model.</param>
		/// <returns>The markdown builder.</returns>
		private static MarkdownBuilder CreateHeader( string title, ArchitectureModel model ) {
			MarkdownBuilder markdown = new MarkdownBuilder();
			markdown.Line( $"# {title}" );
			markdown.Line();
			markdown.Line( $"> Generated by `Nomad.ArchitectureDocs` on {model.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC." );
			markdown.Line();
			return markdown;
		}

		/*
		===============
		MermaidNode
		===============
		*/
		/// <summary>
		/// Converts a name into a Mermaid-safe node identifier.
		/// </summary>
		/// <param name="name">The source name.</param>
		/// <returns>The Mermaid node identifier.</returns>
		private static string MermaidNode( string name ) {
			StringBuilder builder = new StringBuilder( name.Length + 4 );
			builder.Append( 'N' );
			for ( int i = 0; i < name.Length; i++ ) {
				char character = name[i];
				builder.Append( Char.IsLetterOrDigit( character ) ? character : '_' );
			}
			return builder.ToString();
		}

		/*
		===============
		JoinList
		===============
		*/
		/// <summary>
		/// Joins a string list for markdown tables.
		/// </summary>
		/// <param name="values">The values.</param>
		/// <returns>The joined value.</returns>
		private static string JoinList( IReadOnlyList<string>? values ) {
			return values == null || values.Count == 0 ? "—" : String.Join( ", ", values );
		}

		/*
		===============
		Escape
		===============
		*/
		/// <summary>
		/// Escapes markdown table text.
		/// </summary>
		/// <param name="value">The value to escape.</param>
		/// <returns>The escaped value.</returns>
		private static string Escape( string value ) {
			return value.Replace( "|", "\\|" ).Replace( Environment.NewLine, " " ).Replace( "\n", " " ).Trim();
		}

		/*
		===============
		FormatBoolean
		===============
		*/
		/// <summary>
		/// Formats nullable booleans.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The formatted value.</returns>
		private static string FormatBoolean( bool? value ) {
			return value.HasValue ? value.Value ? "Yes" : "No" : "—";
		}
	};

	/*
	===================================================================================

	MarkdownBuilder

	===================================================================================
	*/
	/// <summary>
	/// Small helper for Markdown document construction.
	/// </summary>

	internal sealed class MarkdownBuilder {
		private readonly StringBuilder _builder = new StringBuilder();

		/*
		===============
		Line
		===============
		*/
		/// <summary>
		/// Appends a line.
		/// </summary>
		/// <param name="line">The line text.</param>
		public void Line( string line = "" ) {
			_builder.AppendLine( line );
		}

		/*
		===============
		Bullet
		===============
		*/
		/// <summary>
		/// Appends a bullet line.
		/// </summary>
		/// <param name="line">The bullet text.</param>
		public void Bullet( string line ) {
			_builder.AppendLine( $"- {line}" );
		}

		/*
		===============
		TableHeader
		===============
		*/
		/// <summary>
		/// Appends a Markdown table header.
		/// </summary>
		/// <param name="columns">The column names.</param>
		public void TableHeader( params string[] columns ) {
			_builder.Append( "| " );
			_builder.Append( String.Join( " | ", columns ) );
			_builder.AppendLine( " |" );
			_builder.Append( "| " );
			_builder.Append( String.Join( " | ", columns.Select( static _ => "---" ) ) );
			_builder.AppendLine( " |" );
		}

		/*
		===============
		TableRow
		===============
		*/
		/// <summary>
		/// Appends a Markdown table row.
		/// </summary>
		/// <param name="columns">The row values.</param>
		public void TableRow( params string[] columns ) {
			_builder.Append( "| " );
			_builder.Append( String.Join( " | ", columns ) );
			_builder.AppendLine( " |" );
		}

		/*
		===============
		ToString
		===============
		*/
		/// <summary>
		/// Converts the builder to a string.
		/// </summary>
		/// <returns>The markdown content.</returns>
		public override string ToString() {
			return _builder.ToString();
		}
	};

	/*
	===================================================================================

	ArchitectureModel

	===================================================================================
	*/
	/// <summary>
	/// Full repository architecture model.
	/// </summary>

	internal sealed class ArchitectureModel {
		public DateTimeOffset GeneratedAt { get; }
		public string RepositoryRoot { get; }
		public List<SubsystemManifestEntry> ManifestEntries { get; }
		public List<ProjectInfo> SourceProjects { get; }
		public List<ProjectInfo> TestProjects { get; }
		public List<ProjectInfo> ToolProjects { get; }
		public List<ProjectInfo> SampleProjects { get; }
		public List<ModuleInfo> Modules { get; }
		public List<ArchitectureWarning> Warnings { get; } = new List<ArchitectureWarning>();

		public ArchitectureModel( DateTimeOffset generatedAt, string repositoryRoot, List<SubsystemManifestEntry> manifestEntries, List<ProjectInfo> sourceProjects, List<ProjectInfo> testProjects, List<ProjectInfo> toolProjects, List<ProjectInfo> sampleProjects, List<ModuleInfo> modules ) {
			GeneratedAt = generatedAt;
			RepositoryRoot = repositoryRoot;
			ManifestEntries = manifestEntries;
			SourceProjects = sourceProjects;
			TestProjects = testProjects;
			ToolProjects = toolProjects;
			SampleProjects = sampleProjects;
			Modules = modules;
		}
	};

	/*
	===================================================================================

	SubsystemManifest

	===================================================================================
	*/
	/// <summary>
	/// Root subsystem manifest model.
	/// </summary>

	internal sealed class SubsystemManifest {
		[JsonPropertyName( "Subsystems" )]
		public List<SubsystemManifestEntry> Subsystems { get; set; } = new List<SubsystemManifestEntry>();
	};

	/*
	===================================================================================

	SubsystemManifestEntry

	===================================================================================
	*/
	/// <summary>
	/// Subsystem manifest entry.
	/// </summary>

	internal sealed class SubsystemManifestEntry {
		public string? Name { get; set; }
		public string? Define { get; set; }
		public string? Description { get; set; }
		public List<string>? DependsOn { get; set; }
		public bool? UnityCompatible { get; set; }
		public bool? DefaultEnabled { get; set; }
	};

	/*
	===================================================================================

	ProjectInfo

	===================================================================================
	*/
	/// <summary>
	/// .NET project metadata.
	/// </summary>

	internal sealed class ProjectInfo {
		public string Name { get; }
		public string Path { get; }
		public string Group { get; }
		public string? PackageId { get; }
		public string? Version { get; }
		public List<string> TargetFrameworks { get; }
		public bool? IsPackable { get; }
		public bool? GeneratePackageOnBuild { get; }
		public bool? IsTrimmable { get; }
		public bool? AllowUnsafeBlocks { get; }
		public string? Nullable { get; }
		public string? LangVersion { get; }
		public List<string> ProjectReferences { get; }
		public List<PackageReferenceInfo> PackageReferences { get; }

		public ProjectInfo( string name, string path, string group, string? packageId, string? version, List<string> targetFrameworks, bool? isPackable, bool? generatePackageOnBuild, bool? isTrimmable, bool? allowUnsafeBlocks, string? nullable, string? langVersion, List<string> projectReferences, List<PackageReferenceInfo> packageReferences ) {
			Name = name;
			Path = path;
			Group = group;
			PackageId = packageId;
			Version = version;
			TargetFrameworks = targetFrameworks;
			IsPackable = isPackable;
			GeneratePackageOnBuild = generatePackageOnBuild;
			IsTrimmable = isTrimmable;
			AllowUnsafeBlocks = allowUnsafeBlocks;
			Nullable = nullable;
			LangVersion = langVersion;
			ProjectReferences = projectReferences;
			PackageReferences = packageReferences;
		}
	};

	/*
	===================================================================================

	PackageReferenceInfo

	===================================================================================
	*/
	/// <summary>
	/// Package reference metadata.
	/// </summary>

	internal sealed class PackageReferenceInfo {
		public string Name { get; }
		public string Version { get; }

		public PackageReferenceInfo( string name, string version ) {
			Name = name;
			Version = version;
		}
	};

	/*
	===================================================================================

	ModuleInfo

	===================================================================================
	*/
	/// <summary>
	/// Source module metadata.
	/// </summary>

	internal sealed class ModuleInfo {
		public string Name { get; }
		public string Directory { get; }
		public SubsystemManifestEntry? Manifest { get; }
		public ProjectInfo? Project { get; }
		public ProjectInfo? TestProject { get; }
		public bool HasPublicDirectory { get; }
		public bool HasPrivateDirectory { get; }
		public bool HasReadme { get; }
		public bool HasRoadmap { get; }
		public bool HasChangelog { get; }
		public List<SourceTypeInfo> PublicTypes { get; }
		public List<SourceTypeInfo> PrivateTypes { get; }
		public List<string> PrivateBoundaryViolations { get; }
		public List<string> InternalsVisibleTo { get; }

		public ModuleInfo( string name, string directory, SubsystemManifestEntry? manifest, ProjectInfo? project, ProjectInfo? testProject, bool hasPublicDirectory, bool hasPrivateDirectory, bool hasReadme, bool hasRoadmap, bool hasChangelog, List<SourceTypeInfo> publicTypes, List<SourceTypeInfo> privateTypes, List<string> privateBoundaryViolations, List<string> internalsVisibleTo ) {
			Name = name;
			Directory = directory;
			Manifest = manifest;
			Project = project;
			TestProject = testProject;
			HasPublicDirectory = hasPublicDirectory;
			HasPrivateDirectory = hasPrivateDirectory;
			HasReadme = hasReadme;
			HasRoadmap = hasRoadmap;
			HasChangelog = hasChangelog;
			PublicTypes = publicTypes;
			PrivateTypes = privateTypes;
			PrivateBoundaryViolations = privateBoundaryViolations;
			InternalsVisibleTo = internalsVisibleTo;
		}
	};

	/*
	===================================================================================

	SourceTypeInfo

	===================================================================================
	*/
	/// <summary>
	/// C# source type metadata.
	/// </summary>

	internal sealed class SourceTypeInfo {
		public string Name { get; }
		public string Kind { get; }
		public string Visibility { get; }
		public string Namespace { get; }
		public string Path { get; }
		public bool IsPublicDirectory { get; }
		public string RelativePath => System.IO.Path.GetFileName( Path );

		public SourceTypeInfo( string name, string kind, string visibility, string namespaceName, string path, bool isPublicDirectory ) {
			Name = name;
			Kind = kind;
			Visibility = visibility;
			Namespace = namespaceName;
			Path = path;
			IsPublicDirectory = isPublicDirectory;
		}
	};

	/*
	===================================================================================

	ArchitectureWarning

	===================================================================================
	*/
	/// <summary>
	/// Architecture validation warning.
	/// </summary>

	internal sealed class ArchitectureWarning {
		public ArchitectureWarningSeverity Severity { get; }
		public string Message { get; }

		private ArchitectureWarning( ArchitectureWarningSeverity severity, string message ) {
			Severity = severity;
			Message = message;
		}

		public static ArchitectureWarning Info( string message ) {
			return new ArchitectureWarning( ArchitectureWarningSeverity.Info, message );
		}

		public static ArchitectureWarning Warning( string message ) {
			return new ArchitectureWarning( ArchitectureWarningSeverity.Warning, message );
		}

		public static ArchitectureWarning Error( string message ) {
			return new ArchitectureWarning( ArchitectureWarningSeverity.Error, message );
		}
	};

	/*
	===================================================================================

	ArchitectureWarningSeverity

	===================================================================================
	*/
	/// <summary>
	/// Architecture validation severity.
	/// </summary>

	internal enum ArchitectureWarningSeverity {
		Info,
		Warning,
		Error
	};
};
