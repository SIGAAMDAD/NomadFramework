using Nomad.Events;
using Nomad.Save.Private.Services;
using Nomad.Save.Services;

namespace Nomad.Save.Tests;

public class Tests
{
    private ISaveDataProvider _dataProvider;

    [SetUp]
    public void Setup()
    {
        var logger = new MockLogger();
        var eventFactory = new GameEventRegistry( logger );
        _dataProvider = new SaveDataProvider( eventFactory, logger );
    }
    
    [TearDown]
    public void TearDown()
    {
        _dataProvider.Dispose();
    }

    [Test]
    public void Test_CreateSaveFile()
    {
        _dataProvider.Save( new( "data0.ngd" ) );
    }
}
