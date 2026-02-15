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

#if !UNITY_64
using System;
using Nomad.Core.EngineUtils;
using Nomad.Core.ResourceCache;
using Nomad.Core.Util;

public class MockEngineService : IEngineService
{
	public IConsoleObject CreateConsoleObject()
	{
		throw new NotImplementedException();
	}

	public IDisposable CreateImageRGBA(byte[] image, int width, int height)
	{
		throw new NotImplementedException();
	}

	public string GetApplicationVersion()
	{
		throw new NotImplementedException();
	}

	public string GetEngineVersion()
	{
		throw new NotImplementedException();
	}

	public string GetLocalPath(string ospath)
	{
		throw new NotImplementedException();
	}

	public string GetOSPath(string localpath)
	{
		throw new NotImplementedException();
	}

	public void GetScreenResolution(out int width, out int height)
	{
		throw new NotImplementedException();
	}

	public string GetStoragePath(StorageScope scope)
	{
		return System.IO.Directory.GetCurrentDirectory();
	}

	public string GetStoragePath(string relativePath, StorageScope scope)
	{
		return $"{System.IO.Directory.GetCurrentDirectory()}/{relativePath}";
	}

	public string GetSystemRegion()
	{
		throw new NotImplementedException();
	}

	public void Quit(int exitCode = 0)
	{
		throw new NotImplementedException();
	}

	public void SetScreenResolution(int width, int height)
	{
		throw new NotImplementedException();
	}

	public string Translate(InternString key)
	{
		throw new NotImplementedException();
	}

	public IResourceLoader GetResourceLoader()
	{
		throw new NotImplementedException();
	}
}
#endif
