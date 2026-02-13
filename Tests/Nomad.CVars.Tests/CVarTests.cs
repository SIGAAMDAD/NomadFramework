using System;
using Nomad.CVars.Private.Services;
using Nomad.Events;
using Nomad.FileSystem.Private.Services;
using NUnit.Framework;

namespace Nomad.CVars.Tests;

public class Tests
{
	private ICVarSystemService _cvarSystem;

	[SetUp]
	public void Setup()
	{
		var logger = new MockLogger();
		var engineService = new MockEngineService();
		var fileSystem = new FileSystemService(engineService, logger);
		var eventFactory = new GameEventRegistry(logger);
		_cvarSystem = new CVarSystem(eventFactory, fileSystem, logger);
	}

	[TearDown]
	public void TearDown()
	{
		_cvarSystem.Dispose();
	}

	[Test, Order( 1 )]
	public void Test1_TestCVarRegistration()
	{
		_cvarSystem.Register(
			new CVarCreateInfo<int>(
				name: "testCvar",
				defaultValue: 0,
				description: "its a test."
			)
		);
	}

	[Test, Order( 2 )]
	public void Test2_TestCVarCreationCollision()
	{
		var testCvarInt = _cvarSystem.GetCVar<int>( "testCvar" );
	}
}