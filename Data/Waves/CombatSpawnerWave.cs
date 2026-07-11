using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class CombatSpawnerWave : Wave
{
    [Export] public int EnemyAmount { get; set; } = 1;
    [Export] public Godot.Collections.Array<EnemyPoolEntry> EnemyPool { get; set; } = [];

    public int MinimumBatchSize { get; init; } = 2;    
    public int MaximumBatchSize { get; init; } = 4;
    public float SpawnInterval { get; init; } = 8f;
}
    