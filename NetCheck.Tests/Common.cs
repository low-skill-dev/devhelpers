using Xunit;
using NetChecker;

namespace NetCheckTests;

public class Common
{
	[Fact]
	public async Task CanCheck()
	{
		Assert.True(await NetCheck.CheckNetwork());
	}

	[Fact]
	public async Task CanFail() // disable net for this test
	{
		Assert.False(await NetCheck.CheckNetwork());
	}
}