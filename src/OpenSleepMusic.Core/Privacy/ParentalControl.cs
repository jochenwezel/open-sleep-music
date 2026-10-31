namespace OpenSleepMusic.Core.Privacy;

/// <summary>Optional local child mode. Parent sessions never survive a process restart.</summary>
public sealed class ParentalControl
{
    private readonly object _sync = new();
    private readonly TimeProvider _clock;
    private CancellationTokenSource _online = new();
    // Keep retired sources alive: a request may still be linking its captured token.
    private readonly List<CancellationTokenSource> _retired = [];
    private bool _enabled;
    private bool _locked;
    private int _failedAttempts;
    private DateTimeOffset _retryAfter;
    private HashSet<string>? _allowedCollections;

    public ParentalControl(bool enabled = false, TimeProvider? clock = null,
        int failedAttempts = 0, DateTimeOffset retryAfter = default, IEnumerable<string>? allowedCollections = null)
    {
        _enabled = _locked = enabled;
        _clock = clock ?? TimeProvider.System;
        _failedAttempts = Math.Clamp(failedAttempts, 0, 4);
        _retryAfter = retryAfter;
        _allowedCollections = allowedCollections?.ToHashSet(StringComparer.Ordinal);
        if (enabled) _online.Cancel();
    }

    public bool IsEnabled { get { lock (_sync) return _enabled; } }
    public bool IsLocked { get { lock (_sync) return _locked; } }
    public int FailedAttempts { get { lock (_sync) return _failedAttempts; } }
    public DateTimeOffset RetryAfter { get { lock (_sync) return _retryAfter; } }
    public TimeSpan RetryIn => TimeSpan.FromTicks(Math.Max(0, (RetryAfter - _clock.GetUtcNow()).Ticks));
    public event EventHandler? Changed;

    public bool IsCollectionSelected(string worldId)
    {
        lock (_sync) return _allowedCollections is null || _allowedCollections.Contains(worldId);
    }

    public bool IsCollectionAllowed(string worldId)
    {
        lock (_sync) return !_locked || _allowedCollections is null || _allowedCollections.Contains(worldId);
    }

    public bool IsCollectionVisible(string worldId, bool hasDownloads) =>
        !IsLocked || hasDownloads && IsCollectionAllowed(worldId);

    public void SetAllowedCollections(IEnumerable<string> worldIds)
    {
        lock (_sync)
        {
            if (_locked) throw new InvalidOperationException("Unlock the parent area first.");
            _allowedCollections = worldIds.ToHashSet(StringComparer.Ordinal);
        }
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Enable()
    {
        lock (_sync) _enabled = true;
        Lock();
    }

    public void Lock()
    {
        CancellationTokenSource previous;
        lock (_sync)
        {
            if (!_enabled || _locked) return;
            _locked = true;
            previous = _online;
        }
        previous.Cancel();
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public bool TryUnlock(string pin, string? credential)
    {
        lock (_sync)
        {
            if (!_enabled || !_locked) return true;
            if (_clock.GetUtcNow() < _retryAfter) return false;
            if (!ParentPin.Verify(pin, credential))
            {
                if (++_failedAttempts >= 5)
                {
                    _retryAfter = _clock.GetUtcNow().AddSeconds(30);
                    _failedAttempts = 0;
                }
                return false;
            }
            _failedAttempts = 0;
            _retryAfter = default;
            _retired.Add(_online);
            _online = new CancellationTokenSource();
            _locked = false;
        }
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public void Disable()
    {
        lock (_sync)
        {
            if (_locked) throw new InvalidOperationException("Unlock the parent area first.");
            _enabled = false;
        }
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public CancellationToken OnlineToken()
    {
        lock (_sync)
        {
            if (_locked) throw new HttpRequestException("Network access is disabled in child mode.");
            return _online.Token;
        }
    }
}
