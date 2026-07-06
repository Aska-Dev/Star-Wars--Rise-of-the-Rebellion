using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class AsteroidStormWave : Wave
{
    [Export] public int AstroidWaves = 3;

    public const int MinAmountPerWave = 3;
    public const int MaxAmountPerWave = 8;

    public const float MinTimePerWave = 1.6f;
    public const float MaxTimePerWave = 2.5f;
}
