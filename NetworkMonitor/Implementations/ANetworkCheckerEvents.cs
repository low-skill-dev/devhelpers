namespace NetworkMonitor;

public abstract class ANetworkCheckerEvents : INetworkCheckerEvents
{
	/// <summary>
	/// Fired when network was down and now it is up.
	/// </summary>
	public event Action? NetworkUp;

	/// <summary>
	/// Fired when network was up and now it is down.
	/// </summary>
	public event Action? NetworkDown;

	/// <summary>
	/// Fired when network availability check is started.
	/// </summary>
	public event Action? NetworkCheckStarted;

	/// <summary>
	/// Fired when network availability check is finished.
	/// This event is fired after all <seealso cref="NetworkUp"/>,
	/// <seealso cref="NetworkDown"/> and <seealso cref="NetworkStateChanged"/>.
	/// </summary>
	public event Action? NetworkCheckFinished;

	/// <summary>
	/// Fired when network state changed.
	/// The argument display whether the network is available.
	/// </summary>
	public event Action<bool>? NetworkStateChanged;


	protected internal void FireNetworkUp()
		=> NetworkUp?.Invoke();
	protected internal void FireNetworkDown()
		=> NetworkDown?.Invoke();
	protected internal void FireNetworkCheckStarted()
		=> NetworkCheckStarted?.Invoke();
	protected internal void FireNetworkCheckFinished()
		=> NetworkCheckFinished?.Invoke();
	protected internal void FireNetworkStateChanged(bool isAvailable)
		=> NetworkStateChanged?.Invoke(isAvailable);
}