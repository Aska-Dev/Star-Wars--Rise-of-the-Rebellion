using Godot;
using System;

[GlobalClass]
public partial class CombatFormationWave : Wave
{
    [Export] public Formation Formation { get; set; } = null!;
    [Export] public int Shuffles { get; set; } = 2;

    public float ShuffleChance { get; init; } = 0.6f;
}
