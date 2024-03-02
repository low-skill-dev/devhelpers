namespace NetworkMonitor;

public interface INetworkChecker
{
    /// <summary>
    /// Checks for the network connection.
    /// </summary>
    public Task<bool> CheckNetwork(CancellationToken cToken);
}
