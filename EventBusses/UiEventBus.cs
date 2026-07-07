using Godot;
using System;

public class UiEventBus
{
    // --- PLAYER HEALTH CHANGED ---
    public event Action<int> OnPlayerHealthChanged = null!;
    public void InvokePlayerHealthChanged(int health) => OnPlayerHealthChanged?.Invoke(health);

    // --- OPEN PAUSE MENU---
    public event Action OnGamePaused = null!;
    public void InvokeGamePaused() => OnGamePaused?.Invoke();
}