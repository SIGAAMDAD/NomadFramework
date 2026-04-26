/*
#if !UNITY_EDITOR
using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Nomad.Core.CVars;
using Nomad.Core.Engine.Services;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.CVars.Private.Services;
using Nomad.Events;
using Nomad.FileSystem.Private.Services;
using Nomad.Save.Data;
using Nomad.Save.Events;
using Nomad.Save.Private.Services;
using Nomad.Save.Private.ValueObjects;
using Nomad.Save.Services;
using Nomad.Save.ValueObjects;

namespace Nomad.Save.Tests;

[TestFixture]
[Category("Nomad.Save")]
[Category("Regression")]
public class SaveRegressionTests {
	private ISaveDataProvider _dataProvider;
	private ILoggerService _logger;
	private ICVarSystemService _cvarSystem;
	private IFileSystem _fileSystem;
	private IGameEventRegistryService _eventFactory;
	private Mock<IEngineService> _engineService;
	private string _testDirectory;
	private string _saveDirectory;

	private IGameEvent<SaveBeginEventArgs> _saveBegin;
	private IGameEvent<LoadBeginEventArgs> _loadBegin;

	[SetUp]
	public void Setup() {
		_testDirectory = Path.Combine( Path.GetTempPath(), "NomadSaveRegressionTests", Guid.NewGuid().ToString() );
		_saveDirectory = Path.Combine( _testDirectory, "SaveData" );
		Directory.CreateDirectory( _saveDirectory );

		_logger = new MockLogger();
		_engineService = new Mock<IEngineService>();
		_engineService.Setup( e => e.GetStoragePath( StorageScope.StreamingAssets ) ).Returns( _testDirectory );
		_engineService.Setup( e => e.GetStoragePath( StorageScope.UserData ) ).Returns( _testDirectory );
		_engineService.Setup( e => e.GetStoragePath( StorageScope.Install ) ).Returns( _testDirectory );

		_fileSystem = new FileSystemService( _engineService.Object, _logger );
		_eventFactory = new GameEventRegistry( _logger );
		_cvarSystem = new CVarSystem( _eventFactory, _logger );
		_dataProvider = new SaveDataProvider( _engineService.Object, _eventFactory, _cvarSystem, _fileSystem, _logger );

		_saveBegin = _eventFactory.GetEvent<SaveBeginEventArgs>( EventNames.SAVE_BEGIN_EVENT, EventNames.NAMESPACE );
		_loadBegin = _eventFactory.GetEvent<LoadBeginEventArgs>( EventNames.LOAD_BEGIN_EVENT, EventNames.NAMESPACE );
	}

	[TearDown]
	public void TearDown() {
		_saveBegin?.Dispose();
		_loadBegin?.Dispose();
		_dataProvider?.Dispose();
		_cvarSystem?.Dispose();
		_eventFactory?.Dispose();
		_fileSystem?.Dispose();
		_logger?.Dispose();

		try {
			if ( Directory.Exists( _testDirectory ) ) {
				Directory.Delete( _testDirectory, true );
			}
		} catch {
		}
	}

	[Test]
	public async Task Load_WithMissingFile_DoesNotCreatePhantomSaveMetadata() {
		await _dataProvider.Load( "missing_save" );

		Assert.That( _dataProvider.ListSaveFiles(), Is.Empty );
	}

	[Test]
	public async Task Load_AfterPreviousLoad_DoesNotLeakStaleSections() {
		string currentSectionName = "First";
		int currentValue = 1;
		bool firstSeenOnSecondLoad = true;
		bool secondSeenOnSecondLoad = false;
		int loadCount = 0;

		_saveBegin.Subscribe( ( in SaveBeginEventArgs args ) => {
			var section = args.Writer.AddSection( currentSectionName );
			section.AddField( "Value", currentValue );
		} );

		_loadBegin.Subscribe( ( in LoadBeginEventArgs args ) => {
			loadCount++;
			if ( loadCount != 2 ) {
				return;
			}

			firstSeenOnSecondLoad = args.Reader.FindSection( "First" ) != null;
			secondSeenOnSecondLoad = args.Reader.FindSection( "Second" ) != null;
		} );

		await _dataProvider.Save( "slot_one", default );

		currentSectionName = "Second";
		currentValue = 2;
		await _dataProvider.Save( "slot_two", default );

		await _dataProvider.Load( "slot_one" );
		await _dataProvider.Load( "slot_two" );

		using ( Assert.EnterMultipleScope() ) {
			Assert.That( firstSeenOnSecondLoad, Is.False );
			Assert.That( secondSeenOnSecondLoad, Is.True );
		}
	}

	[Test]
	public async Task Load_WithCorruptGlobalMagic_DoesNotPublishLoadBegin() {
		const string fileId = "corrupt_magic";
		bool loadBeginPublished = false;

		_saveBegin.Subscribe( ( in SaveBeginEventArgs args ) => {
			var section = args.Writer.AddSection( "Player" );
			section.AddField( "Health", 100 );
		} );
		_loadBegin.Subscribe( ( in LoadBeginEventArgs args ) => loadBeginPublished = true );

		await _dataProvider.Save( fileId, default );

		SaveFileMetadata metadata = _dataProvider.ListSaveFiles()[0];
		string filePath = Path.Combine( _saveDirectory, SaveSlot.CalculateFileName( false, metadata ) );
		byte[] fileBytes = File.ReadAllBytes( filePath );
		fileBytes[0] ^= 0xFF;
		File.WriteAllBytes( filePath, fileBytes );

		await _dataProvider.Load( fileId );

		Assert.That( loadBeginPublished, Is.False );
	}
}
#endif
*/