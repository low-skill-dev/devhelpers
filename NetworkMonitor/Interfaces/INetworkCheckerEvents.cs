namespace NetworkMonitor;

public interface INetworkCheckerEvents
{
    /// <summary>
    /// Fired when network state changed.
    /// The argument display whether the network is available.
    /// </summary>
    public event Action<bool>? NetworkStateChanged;

    /// <summary>
    /// Fired when network was up and now it is down.
    /// </summary>
    public event Action? NetworkDown;

    /// <summary>
    /// Fired when network was down and now it is up.
    /// </summary>
    public event Action? NetworkUp;

    /// <summary>
    /// Fired when network availability check is started.
    /// </summary>
    public event Action? NetworkCheckStarted;

    /// <summary>
    /// Fired when network availability check is finished.
    /// </summary>
    public event Action? NetworkCheckFinished;
}
