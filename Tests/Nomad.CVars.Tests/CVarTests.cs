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

#if !UNITY_EDITOR
using System;
using Nomad.Core.CVars;
using Nomad.Core.EngineUtils;
using Nomad.Core.Events;
using Nomad.Core.FileSystem;
using Nomad.Core.Logger;
using Nomad.CVars.Private.Services;
using Nomad.Events;
using Nomad.FileSystem.Private.Services;
using NUnit.Framework;

namespace Nomad.CVars.Tests;

[TestFixture]
public class CVarTests
{
	private ICVarSystemService _cvarSystem;
	private IGameEventRegistryService _registry;
	private IEngineService _engineService;
	private IFileSystem _fileSystem;
	private ILoggerService _logger;

	[SetUp]
	public void Setup()
	{
		_logger = new MockLogger();
		_registry = new GameEventRegistry(_logger);
		_engineService = new MockEngineService();
		_fileSystem = new FileSystemService(_engineService, _logger);
		_cvarSystem = new CVarSystem(_registry, _fileSystem, _logger);
	}

	[TearDown]
	public void TearDown()
	{
		_logger?.Dispose();
		_registry?.Dispose();
		_fileSystem?.Dispose();
		_cvarSystem?.Dispose();
		_engineService?.Dispose();
	}

	#region Registration Tests

	[Test]
	[TestCase(false)]
	[TestCase("")]
	[TestCase((uint)0)]
	[TestCase(0)]
	[TestCase(0.0f)]
	public void RegisterCVar_CreatedWithExplicitType_SetsInitialValue<T>(T value)
	{
		// Arrange
		var createInfo = new CVarCreateInfo<T>(
			Name: "TestCVar",
			DefaultValue: value,
			Description: "A test cvar."
		);

		// Act
		var cvar = _cvarSystem.Register(createInfo);

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(cvar.Value, Is.EqualTo(value));
			Assert.That(cvar.DefaultValue, Is.EqualTo(value));
		}
	}

	[Test]
	[TestCase(false, "Describes a boolean CVar.")]
	[TestCase("", "Describes a string CVar.")]
	[TestCase((uint)0, "Describes a uint CVar.")]
	[TestCase(0, "Describes a int CVar.")]
	[TestCase(0.0f, "Describe a float CVar.")]
	public void RegisterCVar_CreatedWithExplicitType_HasMatchingDescription<T>(T value, string description)
	{
		// Arrange
		var createInfo = new CVarCreateInfo<T>(
			Name: "TestCVar",
			DefaultValue: value,
			Description: description
		);

		// Act
		var cvar = _cvarSystem.Register(createInfo);

		// Assert
		Assert.That(cvar.Description, Is.EqualTo(description));
	}

	[Test]
	[TestCase(false, "TestBoolean")]
	[TestCase("", "TestString")]
	[TestCase((uint)0, "TestUInt")]
	[TestCase(0, "TestInt")]
	[TestCase(0.0f, "TestFloat")]
	public void RegisterCVar_CreatedWithExplicitType_HasMatchingName<T>(T value, string name)
	{
		// Arrange
		var createInfo = new CVarCreateInfo<T>(
			Name: name,
			DefaultValue: value,
			Description: "A test cvar."
		);

		// Act
		var cvar = _cvarSystem.Register(createInfo);

		// Assert
		Assert.That(cvar.Name, Is.EqualTo(name));
	}

	[Test]
	[TestCase(false, CVarFlags.Archive | CVarFlags.Init)]
	[TestCase("", CVarFlags.Archive | CVarFlags.Init)]
	[TestCase((uint)0, CVarFlags.Archive | CVarFlags.Init)]
	[TestCase(0, CVarFlags.Archive | CVarFlags.Init)]
	[TestCase(0.0f, CVarFlags.Archive | CVarFlags.Init)]
	public void RegisterCVar_CreatedWithExplicitType_HasMatchingFlags<T>(T value, CVarFlags flags)
	{
		// Arrange
		var createInfo = new CVarCreateInfo<T>(
			Name: "TestCVar",
			DefaultValue: value,
			Description: "A test cvar.",
			Flags: flags
		);

		// Act
		var cvar = _cvarSystem.Register(createInfo);

		// Assert
		Assert.That(cvar.Flags, Is.EqualTo(flags));
	}

	#endregion

	#region Event Tests

	[Test]
	[TestCase(false, true)]
	[TestCase("", "67")]
	[TestCase((uint)0, (uint)67)]
	[TestCase(0, 67)]
	[TestCase(0.0f, 67.0f)]
	public void SetValue_WithValidValue_TriggersEvent<T>(T value, T newValue)
	{
		// Arrange
		var createInfo = new CVarCreateInfo<T>(
			Name: "TestCVar",
			DefaultValue: value,
			Description: "A test cvar."
		);
		var cvar = _cvarSystem.Register(createInfo);
		var valueChanged = cvar.ValueChanged;
		bool triggered = false;

		void OnCVarValueChanged(in CVarValueChangedEventArgs<T> args)
		{
			triggered = true;
		}

		// Act
		valueChanged.Subscribe(this, OnCVarValueChanged);
		cvar.Value = newValue;
		
		// Assert
		Assert.That(triggered, Is.True);
	}

	[Test]
	[TestCase(false, true)]
	[TestCase("", "67")]
	[TestCase((uint)0, (uint)67)]
	[TestCase(0, 67)]
	[TestCase(0.0f, 67.0f)]
	public void SetValue_WithValidValue_EventArgsHasExpectedValues<T>(T value, T expectedValue)
	{
		// Arrange
		var createInfo = new CVarCreateInfo<T>(
			Name: "TestCVar",
			DefaultValue: value,
			Description: "A test cvar."
		);
		var cvar = _cvarSystem.Register(createInfo);
		var valueChanged = cvar.ValueChanged;
		bool triggered = false;
		T receivedValue = default;
		T oldValue = default;

		void OnCVarValueChanged(in CVarValueChangedEventArgs<T> args)
		{
			triggered = true;
			oldValue = args.OldValue;
			receivedValue = args.NewValue;
		}

		// Act
		valueChanged.Subscribe(this, OnCVarValueChanged);
		cvar.Value = expectedValue;

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(triggered, Is.True);
			Assert.That(oldValue, Is.EqualTo(value));
			Assert.That(receivedValue, Is.EqualTo(expectedValue));
		}
	}

	#endregion

	#region Value Tests

	[Test]
	public void GetCVarIntegerValue_AsInteger_DoesNotThrow()
	{
		// Arrange
		var createInfo = new CVarCreateInfo<int>(
			Name: "TestCVar",
			DefaultValue: 0,
			Description: "A test cvar."
		);
		var cvar = _cvarSystem.Register(createInfo);

		// Act
		Assert.That(
			() => { cvar.GetIntegerValue(); },
			!Throws.InstanceOf<InvalidCastException>()
		);
	}

	[Test]
	public void SetCVarIntegerValue_AsInteger_DoesNotThrow()
	{
		// Arrange
		var createInfo = new CVarCreateInfo<int>(
			Name: "TestCVar",
			DefaultValue: 0,
			Description: "A test cvar."
		);
		var cvar = _cvarSystem.Register(createInfo);

		// Act
		Assert.That(
			() => cvar.SetIntegerValue( 67 ),
			!Throws.InstanceOf<InvalidCastException>()
		);
	}

	[Test]
	public void GetCVarUIntegerValue_AsUInteger_DoesNotThrow()
	{
		// Arrange
		var createInfo = new CVarCreateInfo<uint>(
			Name: "TestCVar",
			DefaultValue: 0,
			Description: "A test cvar."
		);
		var cvar = _cvarSystem.Register(createInfo);

		// Act
		Assert.That(
			() => { cvar.GetUIntegerValue(); },
			!Throws.InstanceOf<InvalidCastException>()
		);
	}

	[Test]
	public void SetCVarUIntegerValue_AsUInteger_DoesNotThrow()
	{
		// Arrange
		var createInfo = new CVarCreateInfo<uint>(
			Name: "TestCVar",
			DefaultValue: 0,
			Description: "A test cvar."
		);
		var cvar = _cvarSystem.Register(createInfo);

		// Act
		Assert.That(
			() => cvar.SetUIntegerValue( 67 ),
			!Throws.InstanceOf<InvalidCastException>()
		);
	}

	[Test]
	public void GetCVarFloatValue_AsFloat_DoesNotThrow()
	{
		// Arrange
		var createInfo = new CVarCreateInfo<float>(
			Name: "TestCVar",
			DefaultValue: 0.0f,
			Description: "A test cvar."
		);
		var cvar = _cvarSystem.Register(createInfo);

		// Act
		Assert.That(
			() => { cvar.GetDecimalValue(); },
			!Throws.InstanceOf<InvalidCastException>()
		);
	}

	[Test]
	public void SetCVarFloatValue_AsFloat_DoesNotThrow()
	{
		// Arrange
		var createInfo = new CVarCreateInfo<float>(
			Name: "TestCVar",
			DefaultValue: 0.0f,
			Description: "A test cvar."
		);
		var cvar = _cvarSystem.Register(createInfo);

		// Act
		Assert.That(
			() => cvar.SetDecimalValue( 67.0f ),
			!Throws.InstanceOf<InvalidCastException>()
		);
	}

	[Test]
	public void GetCVarStringValue_AsString_DoesNotThrow()
	{
		// Arrange
		var createInfo = new CVarCreateInfo<string>(
			Name: "TestCVar",
			DefaultValue: "TestValue",
			Description: "A test cvar."
		);
		var cvar = _cvarSystem.Register(createInfo);

		// Act
		Assert.That(
			() => { cvar.GetStringValue(); },
			!Throws.InstanceOf<InvalidCastException>()
		);
	}

	[Test]
	public void SetCVarStringValue_AsString_DoesNotThrow()
	{
		// Arrange
		var createInfo = new CVarCreateInfo<string>(
			Name: "TestCVar",
			DefaultValue: "TestValue",
			Description: "A test cvar."
		);
		var cvar = _cvarSystem.Register(createInfo);

		// Act
		Assert.That(
			() => cvar.SetStringValue( "NewValue" ),
			!Throws.InstanceOf<InvalidCastException>()
		);
	}

	#endregion
}
#endif
