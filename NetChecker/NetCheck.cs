using System.Net.NetworkInformation;

namespace NetChecker;

public class NetCheck
{
	private static readonly string[] _servers =
	{
		// Most common
		"77.88.8.88",		// 0 Yandex
		"8.8.8.8",			// 1 Google
		"1.1.1.1",			// 2 Cloudflare

		// Others
		"9.9.9.9",			// 3 Quad9
		"76.76.2.0",		// 4 ControlD
		"64.6.64.6",		// 5 Verisign	
		"8.26.56.26",		// 6 Comodo
		"76.76.19.19",		// 7 Alternate  
		"94.140.14.14",		// 8 AdGuard
		"208.67.222.222",	// 9 OpenDNS
	};

	/// <summary>
	/// Checks network availability using the most 
	/// common DNS servers in the next order:
	/// <code>
	/// 0. Yandex		<i>(77.88.8.88)</i>,
	/// 1. Google		<i>(8.8.8.8)</i>,
	///	2. Cloudflare		<i>(1.1.1.1)</i>,
	///	3. Quad9		<i>(9.9.9.9)</i>,
	///	4. ControlD		<i>(76.76.2.0)</i>,
	///	5. Verisign		<i>(64.6.64.6)</i>,
	///	6. Comodo		<i>(8.26.56.26)</i>,
	///	7. Alternate		<i>(76.76.19.19)</i>,
	///	8. AdGuard		<i>(94.140.14.14)</i>,
	///	9. OpenDNS		<i>(208.67.222.222)</i>, 
	/// </code> 
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
			catch(Exception e) when(e is not OperationCanceledException) { }
		}

		return false;
	}
}