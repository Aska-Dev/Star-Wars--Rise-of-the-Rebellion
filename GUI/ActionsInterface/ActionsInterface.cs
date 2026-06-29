using Godot;
using System;
using System.Collections.Generic;

public partial class ActionsInterface : Control
{
    [Export] public required PackedScene ActionSlotScene;

    public GridContainer Grid { get; set; } = null!;

    private List<ActionSlot> _actionSlots = [];

    public override void _Ready()
    {
        Grid = GetNode<GridContainer>("GridContainer");

        Callable.From(SetupSlots).CallDeferred();
    }

    private void SetupSlots()
    {
        var player = PlayerController.GetFrom(this);
        if(player is null)
        {
            Log.Error(nameof(ActionsInterface), nameof(SetupSlots), "PlayerController Instance is null");
            return;
        }

        player.Components.GetComponent<ActionComponent>().ActionExecuted += StartSlotCooldown;
        var actions = player.Components.GetComponent<ActionComponent>().EquippedActions;
        
        for(var i = 0; i < actions.Count; i++)
        {
            var slot = ActionSlotScene.Instantiate<ActionSlot>();
            Grid.AddChild(slot);
            slot.Setup(i, actions[i].IconScene);

            _actionSlots.Add(slot);
        }
    }

    private void StartSlotCooldown(int slotIndex, float cooldown)
    {
        if(slotIndex >= _actionSlots.Count)
        {
            Log.Error(nameof(ActionsInterface), nameof(StartSlotCooldown), "Index out of range");
            return;
        }

        var slot = _actionSlots[slotIndex];
        slot.StartCooldown(cooldown);
    }
}
