using System;
using Nomad.Core.EngineUtils;
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
}