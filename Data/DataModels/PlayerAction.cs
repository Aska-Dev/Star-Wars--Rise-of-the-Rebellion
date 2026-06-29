using Godot;
using System;
using System.Runtime.InteropServices.JavaScript;

[GlobalClass]
public abstract partial class PlayerAction : Resource
{
    [Export] public PackedScene IconScene { get; set; } = null!;
    [Export] public float CooldownDuration { get; set; } = 5;

    [ExportCategory("Sounds")]
    [Export] public SoundEffects ExecutionSound { get; set; }
    [Export] public SoundEffects RechargeCompletedSound { get; set; }

    public abstract bool Execute(PlayerController player);
}
