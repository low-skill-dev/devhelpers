using Xunit.Abstractions;
using Microsoft.Extensions.Logging;

namespace TestHelper;

// https://stackoverflow.com/a/47713709/11325184
public sealed class XUnitLogger<T> : ILogger<T>, IDisposable
{
	private readonly ITestOutputHelper _output;

	public XUnitLogger(ITestOutputHelper output)
	{
		_output = output;
	}

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
	{
		_output.WriteLine(state?.ToString());
	}

	public bool IsEnabled(LogLevel logLevel)
	{
		return true;
	}

	IDisposable ILogger.BeginScope<TState>(TState state)
	{
		return this;
	}

	public void Dispose()
	{
		return;
	}
}