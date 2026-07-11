using System;

/// <summary>
/// Returned by HazardSpawningManager spawn methods.
/// Subscribe to <see cref="Completed"/> to be notified when that specific hazard is gone,
/// regardless of whether it spawned immediately or was queued.
/// </summary>
public class HazardSpawnToken
{
    public event Action? Completed;

    internal void TriggerCompleted() => Completed?.Invoke();
}
