using Xunit;
using NetChecker;

namespace Tests;

#pragma warning disable xUnit1013

public class NetChecker
{
	[Fact]
	public async Task CanCheck()
	{
		Assert.True(await NetCheck.CheckNetwork());
	}

	//[Fact]
	public async Task CanFail() // disable net for this test
	{
		Assert.False(await NetCheck.CheckNetwork());
	}
}