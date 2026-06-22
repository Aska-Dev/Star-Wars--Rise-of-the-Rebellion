using Godot;
using System;

public class PlayerEventBus
{
    public event Action OnPlayerOrientationChanged = null!;
    public void InvokePlayerOrientationChanged() => OnPlayerOrientationChanged?.Invoke();
}