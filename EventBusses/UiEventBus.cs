using Godot;
using System;

public class UiEventBus
{
    public event Action<int> OnPlayerHealthChanged = null!;
    public void InvokePlayerHealthChanged(int health) => OnPlayerHealthChanged?.Invoke(health);
}