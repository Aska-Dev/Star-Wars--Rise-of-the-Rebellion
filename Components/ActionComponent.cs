using Godot;
using System;

[GlobalClass]
public partial class ActionComponent : Component
{
    [Signal] public delegate void ActionExecutedEventHandler(int index, float cooldown);

    [Export] public Godot.Collections.Array<PlayerAction> EquippedActions { get; set; } = [];
    private float[] _currentCooldowns = [];

    public override void _Ready()
    {
        _currentCooldowns = new float[EquippedActions.Count];
    }

    public override void _Process(double delta)
    {
        for (int i = 0; i < _currentCooldowns.Length; i++)
        {
            if (_currentCooldowns[i] > 0)
            {
                _currentCooldowns[i] -= (float)delta;

                if (_currentCooldowns[i] <= 0)
                {
                    _currentCooldowns[i] = 0;
                    AudioEngine.Instance.PlaySound(EquippedActions[i].RechargeCompletedSound);
                }
            }
        }
    }

    public void TryExecuteAction(int slotIndex, PlayerController player)
    {
        if (slotIndex < 0 || slotIndex >= EquippedActions.Count) return;
        if (_currentCooldowns[slotIndex] > 0) return;

        PlayerAction action = EquippedActions[slotIndex];

        if (action.Execute(player))
        {
            _currentCooldowns[slotIndex] = action.CooldownDuration;
            AudioEngine.Instance.PlaySound(action.ExecutionSound);
            EmitSignalActionExecuted(slotIndex, action.CooldownDuration);
        }
    }

    protected override void Initialize(ShipData shipData, ShipModel shipModel) { }
}
