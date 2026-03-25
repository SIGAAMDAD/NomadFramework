using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Input;
using Nomad.Core.Memory.Buffers;
using Nomad.Events;
using Nomad.Input.Private.Repositories;
using Nomad.Input.Private.Services;
using Nomad.Input.Private.ValueObjects;

namespace Nomad.Input.Tests {
	internal sealed class InputFileSystemFixture {
		private readonly Dictionary<string, string> _files = new( StringComparer.OrdinalIgnoreCase );

		public Mock<IFileSystem> Mock { get; }
		public IFileSystem Object => Mock.Object;

		public InputFileSystemFixture( params (string Path, string Content)[] files ) {
			Mock = new Mock<IFileSystem>( MockBehavior.Strict );

			for ( int i = 0; i < files.Length; i++ ) {
				SetFile( files[i].Path, files[i].Content );
			}

			Mock.Setup( fileSystem => fileSystem.Dispose() );
			Mock.Setup( fileSystem => fileSystem.FileExists( It.IsAny<string>() ) )
				.Returns( (string path) => _files.ContainsKey( NormalizePath( path ) ) );
			Mock.Setup( fileSystem => fileSystem.LoadFile( It.IsAny<string>() ) )
				.Returns( (string path) => LoadBuffer( path ) );
			Mock.Setup( fileSystem => fileSystem.DirectoryExists( It.IsAny<string>() ) )
				.Returns( (string path) => DirectoryHasFiles( path ) );
			Mock.Setup( fileSystem => fileSystem.GetFiles( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>() ) )
				.Returns( (string path, string pattern, bool recursive) => GetFiles( path, pattern, recursive ) );
		}

		public void SetFile( string path, string content ) {
			_files[NormalizePath( path )] = content;
		}

		private IBufferHandle? LoadBuffer( string path ) {
			if ( !_files.TryGetValue( NormalizePath( path ), out string? content ) ) {
				return null;
			}

			byte[] bytes = Encoding.UTF8.GetBytes( content );
			var buffer = new StandardBufferHandle( bytes.Length );
			bytes.CopyTo( buffer.Span );
			return buffer;
		}

		private bool DirectoryHasFiles( string path ) {
			string normalizedDirectory = NormalizeDirectory( path );
			return _files.Keys.Any( file => file.StartsWith( normalizedDirectory, StringComparison.OrdinalIgnoreCase ) );
		}

		private IReadOnlyList<string> GetFiles( string path, string pattern, bool recursive ) {
			string normalizedDirectory = NormalizeDirectory( path );

			return _files.Keys
				.Where( file => file.StartsWith( normalizedDirectory, StringComparison.OrdinalIgnoreCase ) )
				.Where( file => MatchesPattern( file, pattern ) )
				.OrderBy( file => file, StringComparer.OrdinalIgnoreCase )
				.ToArray();
		}

		private static bool MatchesPattern( string path, string pattern ) {
			return pattern switch {
				"*.json" => path.EndsWith( ".json", StringComparison.OrdinalIgnoreCase ),
				_ => Path.GetFileName( path ).Equals( pattern, StringComparison.OrdinalIgnoreCase )
			};
		}

		private static string NormalizeDirectory( string path ) {
			string normalized = NormalizePath( path );
			return normalized.EndsWith( "/", StringComparison.Ordinal ) ? normalized : normalized + "/";
		}

		private static string NormalizePath( string path ) {
			return path.Replace( '\\', '/' ).Trim();
		}
	}

	internal static class InputTestHelpers {
		public static GameEventRegistry CreateEventRegistry( out MockLogger logger ) {
			logger = new MockLogger();
			return new GameEventRegistry( logger );
		}

		public static MockCVarSystem CreateCVarSystem( IGameEventRegistryService eventRegistry, string defaultsPath ) {
			var cvarSystem = new MockCVarSystem( eventRegistry );
			cvarSystem.SetCVar( Nomad.Input.Private.Constants.CVars.DEFAULTS_PATH, defaultsPath );
			cvarSystem.SetCVar( Nomad.Input.Private.Constants.CVars.INPUT_DELAY_MS, 0 );
			return cvarSystem;
		}

		public static InputActionDefinition Action( string name, InputValueType valueType, params InputBindingDefinition[] bindings ) {
			return new InputActionDefinition( name, valueType, bindings.ToImmutableArray() );
		}

		public static InputBindingDefinition Button( InputScheme scheme, InputDeviceSlot device, InputControlId control, params InputControlId[] modifiers ) {
			return new InputBindingDefinition {
				Scheme = scheme,
				Kind = InputBindingKind.Button,
				Button = new ButtonBinding {
					DeviceId = device,
					ControlId = control,
					Modifiers = modifiers.ToImmutableArray()
				}
			};
		}

		public static InputBindingDefinition Axis1D( InputScheme scheme, InputDeviceSlot device, InputControlId control, float deadzone = 0.0f, float sensitivity = 1.0f, float scale = 1.0f, bool invert = false ) {
			return new InputBindingDefinition {
				Scheme = scheme,
				Kind = InputBindingKind.Axis1D,
				Axis1D = new Axis1DBinding {
					DeviceId = device,
					ControlId = control,
					Deadzone = deadzone,
					Sensitivity = sensitivity,
					Scale = scale,
					Invert = invert
				}
			};
		}

		public static InputBindingDefinition Axis2D( InputScheme scheme, InputDeviceSlot device, InputControlId control, float deadzone = 0.0f, float sensitivity = 1.0f, float scaleX = 1.0f, float scaleY = 1.0f, bool invertX = false, bool invertY = false ) {
			return new InputBindingDefinition {
				Scheme = scheme,
				Kind = InputBindingKind.Axis2D,
				Axis2D = new Axis2DBinding {
					DeviceId = device,
					ControlId = control,
					Deadzone = deadzone,
					Sensitivity = sensitivity,
					ScaleX = scaleX,
					ScaleY = scaleY,
					InvertX = invertX,
					InvertY = invertY
				}
			};
		}

		public static InputBindingDefinition Delta2D( InputScheme scheme, InputDeviceSlot device, InputControlId control, float sensitivity = 1.0f, float scaleX = 1.0f, float scaleY = 1.0f, bool invertX = false, bool invertY = false ) {
			return new InputBindingDefinition {
				Scheme = scheme,
				Kind = InputBindingKind.Delta2D,
				Delta2D = new Delta2DBinding {
					DeviceId = device,
					ControlId = control,
					Sensitivity = sensitivity,
					ScaleX = scaleX,
					ScaleY = scaleY,
					InvertX = invertX,
					InvertY = invertY
				}
			};
		}

		public static InputBindingDefinition Composite1D( InputScheme scheme, InputControlId negative, InputControlId positive, float scale = 1.0f, bool normalize = true ) {
			return new InputBindingDefinition {
				Scheme = scheme,
				Kind = InputBindingKind.Axis1DComposite,
				Axis1DComposite = new Axis1DCompositeBinding {
					Negative = negative,
					Positive = positive,
					Scale = scale,
					Normalize = normalize
				}
			};
		}

		public static InputBindingDefinition Composite2D( InputScheme scheme, InputControlId up, InputControlId down, InputControlId left, InputControlId right, float scaleX = 1.0f, float scaleY = 1.0f, bool normalize = true ) {
			return new InputBindingDefinition {
				Scheme = scheme,
				Kind = InputBindingKind.Axis2DComposite,
				Axis2DComposite = new Axis2DCompositeBinding {
					Up = up,
					Down = down,
					Left = left,
					Right = right,
					ScaleX = scaleX,
					ScaleY = scaleY,
					Normalize = normalize
				}
			};
		}

		public static CompiledBindingRepository CompileToRepository( params InputActionDefinition[] actions ) {
			var repository = new CompiledBindingRepository();
			var compiler = new BindingCompilerService( repository );
			compiler.CompileIntoRepository( actions.ToImmutableArray() );
			return repository;
		}
	}
}
