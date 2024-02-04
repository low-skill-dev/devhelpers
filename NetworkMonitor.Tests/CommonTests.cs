using NetworkMonitor.Implementations;
using Xunit;

namespace NetworkMonitor.Tests;

public class CommonTests
{
    PingNetworkChecker _pingChecker;
	SystemNetworkChecker _systemChecker;

	public CommonTests()
    {
        _pingChecker = new();
        _systemChecker = new();
	}

	[Fact]
    public void CanCreate()
    {
        Assert.NotNull(_pingChecker);
        Assert.NotNull(_systemChecker);
	}

    [Fact]
    public async Task CanCheckNetwork()
    {
        Assert.True(await _pingChecker.CheckNetwork());
		Assert.True(await _systemChecker.CheckNetwork());
	}
}