using Godot;
using System;

[GlobalClass]
public partial class AstromechRepairAction : PlayerAction
{
    [Export] public int HealAmount { get; set; } = 1;

    public override bool Execute(PlayerController player)
    {
        var healthComponent = player.Components.GetComponent<HealthComponent>();

        if (healthComponent == null || healthComponent.Health == healthComponent.MaxHealth)
        {
            return false;
        }

        healthComponent.Heal(HealAmount);
        AudioEngine.Instance.PlaySound(SoundEffects.AstromechActivation);

        return true; 
    }
}
