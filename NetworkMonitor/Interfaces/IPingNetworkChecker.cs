namespace NetworkMonitor;

public interface IPingNetworkChecker : INetworkChecker
{
    public int SizeOfHostsGroup { get; set; }
}
