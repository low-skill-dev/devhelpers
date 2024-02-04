using NetworkMonitor.Implementations;
using System;
using Xunit;

namespace NetworkMonitor.Tests;

public class PingNetworkCheckerTests
{
	PingNetworkChecker _pingChecker;

	public PingNetworkCheckerTests()
	{
		_pingChecker = new();
	}

	[Fact]
	public async Task CanOverrideHosts()
	{
		_pingChecker = new(new string[] { "google.com" });
		await _pingChecker.CheckNetwork();

		Assert.Single(_pingChecker.Hosts);
		Assert.Single(_pingChecker.CurrentHostsGroup);
		Assert.Equal("google.com", _pingChecker.CurrentHostsGroup.Single());
	}


	[Fact]
	public async Task CanOverrideSizeOfHostsGroup()
	{
		var rand = Random.Shared.Next(2, 768);
		_pingChecker = new() { SizeOfHostsGroup = rand };

		await _pingChecker.CheckNetwork();

		Assert.Equal(rand, _pingChecker.CurrentHostsGroup.Count);
	}

	[Fact]
	public async Task CanRotateHosts()
	{
		//var rand = Random.Shared.Next(2, 16);
		var rand = 1;
		_pingChecker = new() { CurrentHostsGroupRegenerationThreshold = rand };

		await _pingChecker.CheckNetwork();
		var cHosts = _pingChecker.CurrentHostsGroup;
		for(int i = 1; i< rand; i++) await _pingChecker.CheckNetwork();
		Assert.Equal(cHosts, _pingChecker.CurrentHostsGroup);

		await _pingChecker.CheckNetwork();
		Assert.NotEqual(cHosts, _pingChecker.CurrentHostsGroup);
		Assert.False(cHosts.SequenceEqual(_pingChecker.CurrentHostsGroup));
	}
}
