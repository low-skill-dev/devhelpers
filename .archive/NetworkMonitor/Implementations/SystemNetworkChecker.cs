using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMonitor.Implementations;

/// <summary>
/// Uses the system network switch to check for network availability.
/// </summary>
[Obsolete($"Use {nameof(NetworkMonitor.Implementations.PingNetworkChecker)} instead.")]
public class SystemNetworkChecker : ANetworkCheckerEvents, INetworkChecker
{
    public SystemNetworkChecker()
    {

    }

    public async Task<bool> CheckNetwork(CancellationToken cToken = default)
    {
        if(OperatingSystem.IsWindows()) return await Task.FromResult(
            NetworkInterface.GetIsNetworkAvailable());

        if(OperatingSystem.IsLinux()) return await Task.FromResult(
            NetworkInterface.GetAllNetworkInterfaces().Any(x =>
                x.OperationalStatus == OperationalStatus.Up && x.Speed > 0 &&
                x.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                x.NetworkInterfaceType != NetworkInterfaceType.Tunnel));

        throw new PlatformNotSupportedException();
    }
}
