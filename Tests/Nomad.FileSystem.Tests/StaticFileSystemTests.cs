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

using Nomad.Core.FileSystem;
using NUnit.Framework;
using System;
using Nomad.FileSystem.Private.Services;
using Nomad.Core.Exceptions;

namespace Nomad.FileSystem.Tests
{
	[TestFixture]
	[Category("UnitTests")]
	public class StaticFileSystemTests
	{
		private IFileSystem _fileSystem;

		[SetUp]
		public void Setup()
		{
			_fileSystem = new FileSystemService(new MockEngineService(), new MockLogger());
		}

		[TearDown]
		public void TearDown()
		{
			_fileSystem?.Dispose();
		}

		[Test]
		public void Initialize_WithValidService_DoesNotThrow()
		{
			Assert.DoesNotThrow(() => Global.FileSystem.Initialize(_fileSystem));
		}

		[Test]
		public void Initialize_WithNullService_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => Global.FileSystem.Initialize(null!));
		}

		[Test]
		public void CallStatic_WithoutInitialization_ThrowsSubsystemNotInitializedException()
		{
			Assert.Throws<SubsystemNotInitializedException>(() => Global.FileSystem.GetConfigPath());
		}
	}
}