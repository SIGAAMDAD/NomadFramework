using System;
using Nomad.Core.Logger;
using Nomad.Core.ServiceRegistry.Interfaces;

public class MockLogger : ILoggerService
{
	public void AddSink(ILoggerSink sink)
	{
	}

	public void Clear()
	{
		Console.Clear();
	}

	public ILoggerCategory CreateCategory(in string name, LogLevel level, bool enabled)
	{
		return null;
	}

	public void Dispose()
	{
	}

	public void InitConfig(IServiceLocator locator)
	{
		throw new System.NotImplementedException();
	}

	public void Print(in string message)
	{
		Console.WriteLine(message);
	}

	public void Print(in ILoggerCategory category, in string message)
	{
		Console.WriteLine(message);
	}

	public void PrintDebug(in string message)
	{
		Console.WriteLine($"DEBUG: {message}");
	}

	public void PrintDebug(in ILoggerCategory category, in string message)
	{
		Console.WriteLine($"DEBUG: {message}");
	}

	public void PrintError(in string message)
	{
		Console.WriteLine($"ERROR: {message}");
	}

	public void PrintError(in ILoggerCategory category, in string message)
	{
		Console.WriteLine($"ERROR: {message}");
	}

	public void PrintLine(in string message)
	{
		Console.WriteLine(message);
	}

	public void PrintLine(in ILoggerCategory category, in string message)
	{
		Console.WriteLine(message);
	}

	public void PrintWarning(in string message)
	{
		Console.WriteLine($"WARNING: {message}");
	}

	public void PrintWarning(in ILoggerCategory category, in string message)
	{
		Console.WriteLine($"WARNING: {message}");
	}
}