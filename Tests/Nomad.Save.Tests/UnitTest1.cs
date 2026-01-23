using System;
using Nomad.Events;
using Nomad.Save.Data;
using Nomad.Save.Events;
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

        var saveBegin = eventFactory.GetEvent<SaveBeginEventArgs>( EventNames.NAMESPACE, EventNames.SAVE_BEGIN_EVENT );
        saveBegin.Subscribe( this, OnSaveBegin );
    }
    
    [TearDown]
    public void TearDown()
    {
        _dataProvider.Dispose();
    }

    [Test, Order( 1 )]
    public void Test1_CreateSaveFile()
    {
        _dataProvider.Save( new( "data0.ngd" ) ).Wait();
    }

    [Test, Order( 2 )]
    public void Test2_LoadSaveFile()
    {
        _dataProvider.Load( new( "data0.ngd" ) );
    }

    private void OnSaveBegin( in SaveBeginEventArgs args )
    {
        var section = args.Writer.AddSection( "TestSection" );
        section.AddField( "TestFloat", 0.0f );
        section.AddField( "TestInt", (int)0 );
        section.AddField( "TestUInt", (uint)0 );
        section.AddField( "TestString", "testing" );
    }
}
