using Xunit.Abstractions;

namespace TestHelper;

public abstract class ATestsWithLogging
{
	protected ITestOutputHelper _output;

	public ATestsWithLogging(ITestOutputHelper output)
	{
		_output = output;
	}

	protected XUnitLogger<T> CreateLogger<T>()
	{
		return new XUnitLogger<T>(_output);
	}
}