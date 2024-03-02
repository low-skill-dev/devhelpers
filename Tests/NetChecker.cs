using Xunit;
using NetChecker;

namespace NetCheckTests;

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