using System.Collections.Frozen;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace NetworkMonitor.Implementations;

/// <summary>
/// Uses the <see cref="Ping"/> class to check for network availability.
/// </summary>
public class PingNetworkChecker : ANetworkCheckerEvents, INetworkChecker
{
	#region protected fields

	protected internal readonly FrozenDictionary<string, StrongBox<int>> _hosts;
	protected internal List<string> _currentHostsGroup;
	protected internal int _sizeOfHostsGroup;

	protected internal TimeSpan _currentHostsGroupRegenerationTimeout;
	protected internal DateTime _currentHostsGroupRegenerated;

	protected internal int _currentHostsGroupRegenerationThreshold;
	protected internal int _currentHostsGroupUsageCount;

	protected internal bool _lastNetworkStatus;

	#endregion

	#region public properties

	/// <summary>
	/// The hosts to be used with a <seealso cref="Ping"/> class. 
	/// Provided in a constructor or taken from defaults. The
	/// value displays the number of times selected host
	/// was used for ping.
	/// </summary>
	public IReadOnlyDictionary<string, StrongBox<int>> Hosts
		=> _hosts.AsReadOnly();

	/// <summary>
	/// The group of hosts, currently used for ping.
	/// </summary>
	public IReadOnlyList<string> CurrentHostsGroup
		=> _currentHostsGroup.AsReadOnly();

	/// <summary>
	/// The number of hosts that are being placed to the hosts
	/// group and then being used for network checks until
	/// the <seealso cref="CurrentHostsGroupRegenerationThreshold"/>
	/// or <seealso cref="CurrentHostsGroupRegenerationTimeout"/> 
	/// is reached.
	/// </summary>
	public int SizeOfHostsGroup
	{
		get => _sizeOfHostsGroup;
		set
		{
			if(value < 1) throw new ArgumentOutOfRangeException(
				$"{nameof(SizeOfHostsGroup)} must be more than zero.");

			_sizeOfHostsGroup = value;
		}
	}

	/// <summary>
	/// The timeout of current hosts group to be used
	/// without regeneration.
	/// </summary>
	public TimeSpan CurrentHostsGroupRegenerationTimeout
	{
		get => _currentHostsGroupRegenerationTimeout;
		set
		{
			_currentHostsGroupRegenerationTimeout = value;
		}
	}

	/// <summary>
	/// The DateTime, when the last current hosts
	/// group was generated. At this moment the
	/// hosts with the least number requests are
	/// placed in a collection of the size defined in
	/// <seealso cref="CurrentHostsGroupSize"/> for
	/// use in farther requests.
	/// </summary>
	public DateTime CurrentHostsGroupRegenerated => _currentHostsGroupRegenerated;

	/// <summary>
	/// The number of times the current hosts group was used.
	/// When this number overcomes 
	/// <seealso cref="CurrentHostsGroupRegenerationThreshold"/>,
	/// it's being regenerated.
	/// </summary>
	public int CurrentHostsGroupUsageCount => _currentHostsGroupUsageCount;


	/// <summary>
	/// The number of network checks that may be performed
	/// before the current hosts group is being regenerated.
	/// </summary>
	public int CurrentHostsGroupRegenerationThreshold
	{
		get => _currentHostsGroupRegenerationThreshold;
		set
		{
			if(value < 0) throw new ArgumentOutOfRangeException(
				$"{nameof(SizeOfHostsGroup)} must be not negative.");

			_currentHostsGroupRegenerationThreshold = value;
		}
	}

	/// <summary>
	/// Network status after the last check.
	/// </summary>
	public bool LastNetworkStatus => _lastNetworkStatus;

	#endregion

	/// <param name="hosts">
	/// Hosts to be used with a <seealso cref="Ping"/> class.
	/// </param>
	public PingNetworkChecker(IEnumerable<string>? hosts = null)
	{
		_hosts = (hosts ?? Defaults.DefaultPingHosts)
			.ToFrozenDictionary(x => x, x => new StrongBox<int>(0));

		_currentHostsGroup = new();
		_sizeOfHostsGroup = 16;
		_currentHostsGroupRegenerationThreshold = 16;
		_currentHostsGroupRegenerationTimeout = TimeSpan.FromMinutes(3);
	}

	#region protected methods

	protected internal virtual void GenerateHostsGroup()
	{
		_currentHostsGroup = _hosts.OrderBy(x => x.Value)
			.Take(_sizeOfHostsGroup).Select(x => x.Key).ToList();
	}

	protected internal virtual void RegenerateHostsGroupIfNeeded()
	{
		var timeDiff = DateTime.UtcNow - _currentHostsGroupRegenerated;
		var count = _currentHostsGroupUsageCount;

		if(count < CurrentHostsGroupRegenerationThreshold &&
		   timeDiff < CurrentHostsGroupRegenerationTimeout &&
		   CurrentHostsGroup.Count > 0) return;

		if(_sizeOfHostsGroup >= _hosts.Count &&
		   CurrentHostsGroup.Count > 0) return;

		_currentHostsGroup = _hosts.OrderBy(x => x.Value.Value)
			.Take(_sizeOfHostsGroup).Select(x => x.Key).ToList();

		_currentHostsGroupRegenerated = DateTime.UtcNow;
		_currentHostsGroupUsageCount = 0;
	}

	protected internal virtual void OnCheckFinish(bool isAvailable)
	{
		var prev = _lastNetworkStatus;
		_lastNetworkStatus = isAvailable;

		if(prev != _lastNetworkStatus)
		{
			FireNetworkStateChanged(_lastNetworkStatus);

			if(_lastNetworkStatus) FireNetworkUp();
			else FireNetworkDown();
		}

		FireNetworkCheckFinished();
	}

	protected internal virtual void OnCheckStart()
	{
		FireNetworkCheckStarted();
		RegenerateHostsGroupIfNeeded();
		Interlocked.Increment(ref _currentHostsGroupUsageCount);
	}

	#endregion

	public async Task<bool> CheckNetwork(CancellationToken cToken = default)
	{
		List<string> hosts;
		lock(_currentHostsGroup)
		{
			OnCheckStart();
			hosts = _currentHostsGroup;
		}

		var ping = new Ping();
		foreach(var host in hosts)
		{
			var result = IPStatus.Unknown;
			try
			{
				/* If you pass 0 as the timeout parameter to the Ping.Send method 
				 * in C#, the Ping class will use a default timeout value. This 
				 * default value is typically around 4 seconds, but it can vary 
				 * slightly depending on the operating system and network 
				 * configuration.
				 */
				Interlocked.Increment(ref _hosts[host].Value);
				result = (await ping.SendPingAsync(
					host, new TimeSpan(int.MaxValue),
					cancellationToken: cToken).ConfigureAwait(false)).Status;
			}
			catch(Exception ex) when(ex is not OperationCanceledException) { }

			if(result == IPStatus.Success)
			{
				OnCheckFinish(true);
				return true;
			}
		}

		OnCheckFinish(false);
		return false;
	}
};
