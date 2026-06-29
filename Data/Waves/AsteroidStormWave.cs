using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class AsteroidStormWave : Wave
{
    [Export] public int AstroidWaves = 3;

    private const int minAmountPerWave = 2;
    private const int maxAmountPerWave = 5;

    private const float minTimePerWave = 6f;
    private const float maxTimePerWave = 10f;
}
