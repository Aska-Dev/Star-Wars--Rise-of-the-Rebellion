using Godot;
using System;

public enum ParticleEmitter
{
    AsteroidStorm
}

public partial class EventHub : Node
{
    public static EventHub Instance { get; private set; } = null!;

    // --- EVENT BUSSES ---
    public UiEventBus UiEventBus { get; private set; } = null!;
    public PlayerEventBus PlayerEventBus { get; private set; } = null!;

    public override void _Ready()
    {
        Instance = this;

        PlayerEventBus = new();
        UiEventBus = new();
    }

    public event Action<(ParticleEmitter, bool)> OnParticleEmitterStateChanged = null!;
    public void InvokeParticleEmitterStateChanged(ParticleEmitter emitter, bool isActive) => OnParticleEmitterStateChanged?.Invoke((emitter, isActive));
}
