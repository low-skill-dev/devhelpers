using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMonitor.Implementations;

public static class StaticNetworkChecker
{
	private static readonly string[] _servers =
	{
		// Most common
		"208.67.222.222",	// 0 OpenDNS
		"77.88.8.88",		// 1 Yandex
		"8.8.8.8",			// 2 Google

		// Others
		"1.1.1.1",			// 3 Cloudflare
		"9.9.9.9",			// 4 Quad9
		"64.6.64.6",		// 5 Verisign	
		"8.26.56.26",		// 6 Comodo
		"76.76.19.19",		// 7 Alternate  
		"94.140.14.14",		// 8 AdGuard
		"185.228.168.9",	// 9 CleanBrowsing 
	};

	/// <summary>
	/// Checks network using the most common DNS servers.
	/// </summary>
	public static async Task<bool> CheckNetwork(CancellationToken ct = default)
	{
		var ping = new Ping();
		foreach(var server in _servers)
		{
			try
			{
				if((await ping.SendPingAsync(
					server, new TimeSpan(int.MaxValue),
					cancellationToken: ct)).Status == 0) return true;
			}
			catch(Exception e) when(e is not OperationCanceledException)
			{

			}
		}

		return false;
	}
}
