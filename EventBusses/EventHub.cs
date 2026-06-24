using Godot;
using System;

public partial class EventHub : Node
{
    public static EventHub Instance { get; private set; }

    // --- EVENT BUSSES ---
    public UiEventBus UiEventBus { get; private set; } = null!;
    public PlayerEventBus PlayerEventBus { get; private set; } = null!;

    public override void _Ready()
    {
        Instance = this;

        PlayerEventBus = new();
        UiEventBus = new();
    }
}
