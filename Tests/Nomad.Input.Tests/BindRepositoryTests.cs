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
using System.Linq;
using NUnit.Framework;
using Nomad.Events;
using Nomad.Input.Private.Repositories;
using System.Collections.Immutable;

namespace Nomad.Input.Tests
{
	[TestFixture]
	public class BindRepositoryTests
	{
		private GameEventRegistry _eventRegistry;
		private MockLogger _logger;

		[SetUp]
		public void SetUp()
		{
			_eventRegistry = InputTestHelpers.CreateEventRegistry(out _logger);
		}

		[TearDown]
		public void TearDown()
		{
			_eventRegistry.Dispose();
			_logger.Dispose();
		}

		[Test]
		public void GetDefaultBindings_ReturnsBindingsLoadedFromConfiguredDefaultsFile()
		{
			const string defaultsPath = "Config/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": [
				    {
				      "Name": "Jump",
				      "ValueType": "Button",
				      "Scheme": "KeyboardAndMouse",
				      "Bindings": { "DeviceId": "Keyboard", "ControlId": "Space" }
				    }
				  ]
				}
				""")
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem(_eventRegistry, defaultsPath);

			using var repository = new BindRepository(fileSystem.Object, cvarSystem);

			var defaults = repository.GetDefaultBindings();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(defaults.Length, Is.EqualTo(1));
				Assert.That(defaults[0].Name, Is.EqualTo("Jump"));
			}
		}

		[Test]
		public void GetBindMappings_ReturnsDefaultsMergedIntoEachMapping()
		{
			const string defaultsPath = "Config/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": [
				    {
				      "Name": "Jump",
				      "ValueType": "Button",
				      "Scheme": "KeyboardAndMouse",
				      "Bindings": { "DeviceId": "Keyboard", "ControlId": "Space" }
				    }
				  ]
				}
				"""),
				("Assets/Config/Bindings/Gameplay.json", """
				{
				  "Bindings": [
				    {
				      "Name": "Move",
				      "ValueType": "Vector2",
				      "Scheme": "KeyboardAndMouse",
				      "Bindings": {
				        "Kind": "Axis2DComposite",
				        "Up": "W",
				        "Down": "S",
				        "Left": "A",
				        "Right": "D"
				      }
				    }
				  ]
				}
				""")
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem(_eventRegistry, defaultsPath);

			using var repository = new BindRepository(fileSystem.Object, cvarSystem);

			var mappings = repository.GetBindMappings();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(mappings.ContainsKey("Gameplay"), Is.True);
				Assert.That(mappings["Gameplay"].Select(action => action.Name), Is.EquivalentTo(new[] { "Jump", "Move" }));
			}
		}

		[Test]
		public void GetAllBindings_MergesActionsAcrossDefaultsAndMappingsByName()
		{
			const string defaultsPath = "Config/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": [
				    {
				      "Name": "Shoot",
				      "ValueType": "Button",
				      "Scheme": "KeyboardAndMouse",
				      "Bindings": { "DeviceId": "MouseButton", "ControlId": "Left" }
				    }
				  ]
				}
				"""),
				("Assets/Config/Bindings/Gamepad.json", """
				{
				  "Bindings": [
				    {
				      "Name": "Shoot",
				      "ValueType": "Button",
				      "Scheme": "Gamepad",
				      "Bindings": { "DeviceId": "Gamepad", "ControlId": "A" }
				    }
				  ]
				}
				""")
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem(_eventRegistry, defaultsPath);

			using var repository = new BindRepository(fileSystem.Object, cvarSystem);

			var allBindings = repository.GetAllBindings();

			using (Assert.EnterMultipleScope())
			{
				Assert.That(allBindings.Length, Is.EqualTo(1));
				Assert.That(allBindings[0].Name, Is.EqualTo("Shoot"));
				Assert.That(allBindings[0].Bindings.Length, Is.EqualTo(2));
			}
		}

		[Test]
		public void TryGetBindMapping_WhenMappingDoesNotExist_ReturnsFalse()
		{
			const string defaultsPath = "Config/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": []
				}
				""")
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem(_eventRegistry, defaultsPath);

			using var repository = new BindRepository(fileSystem.Object, cvarSystem);

			bool found = repository.TryGetBindMapping("Missing", out var bindings);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(found, Is.False);
				Assert.That(bindings, Is.Empty);
			}
		}

		[Test]
		public void Reload_RefreshesTheMergedCachesAfterFileChanges()
		{
			const string defaultsPath = "Config/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": [
				    {
				      "Name": "Jump",
				      "ValueType": "Button",
				      "Scheme": "KeyboardAndMouse",
				      "Bindings": { "DeviceId": "Keyboard", "ControlId": "Space" }
				    }
				  ]
				}
				"""),
				("Assets/Config/Bindings/Gameplay.json", """
				{
				  "Bindings": [
				    {
				      "Name": "Move",
				      "ValueType": "Vector2",
				      "Scheme": "KeyboardAndMouse",
				      "Bindings": {
				        "Kind": "Axis2DComposite",
				        "Up": "W",
				        "Down": "S",
				        "Left": "A",
				        "Right": "D"
				      }
				    }
				  ]
				}
				""")
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem(_eventRegistry, defaultsPath);

			using var repository = new BindRepository(fileSystem.Object, cvarSystem);
			fileSystem.SetFile("Assets/Config/Bindings/Gameplay.json", """
			{
			  "Bindings": [
			    {
			      "Name": "Crouch",
			      "ValueType": "Button",
			      "Scheme": "KeyboardAndMouse",
			      "Bindings": { "DeviceId": "Keyboard", "ControlId": "Ctrl" }
			    }
			  ]
			}
			""");

			repository.Reload();

			Assert.That(repository.GetBindMappings()["Gameplay"].Select(action => action.Name), Is.EquivalentTo(new[] { "Jump", "Crouch" }));
		}

		[Test]
		public void Constructor_WhenDefaultsFileIsMissing_ThrowsFileNotFoundException()
		{
			var fileSystem = new InputFileSystemFixture();
			var cvarSystem = InputTestHelpers.CreateCVarSystem(_eventRegistry, "Config/MissingDefaults.json");

			Assert.That(() => new BindRepository(fileSystem.Object, cvarSystem), Throws.TypeOf<System.IO.FileNotFoundException>());
		}

		[Test]
		public void Reload_WhenTwoMappingsShareTheSameFilename_ThrowsInvalidOperationException()
		{
			const string defaultsPath = "Config/DefaultBinds.json";
			var fileSystem = new InputFileSystemFixture(
				(defaultsPath, """
				{
				  "Bindings": []
				}
				"""),
				("Assets/Config/Bindings/A/Gameplay.json", "{ \"Bindings\": [] }"),
				("Assets/Config/Bindings/B/Gameplay.json", "{ \"Bindings\": [] }")
			);
			var cvarSystem = InputTestHelpers.CreateCVarSystem(_eventRegistry, defaultsPath);

			Assert.That(() => new BindRepository(fileSystem.Object, cvarSystem), Throws.TypeOf<InvalidOperationException>());
		}
	}
}
