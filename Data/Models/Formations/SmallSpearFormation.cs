using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.Marshalling;

[GlobalClass]
public partial class SmallSpearFormation : Formation
{
    [Export] public PackedScene PrimaryEnemy { get; set; } = null!;
    [Export] public PackedScene SecondaryEnemy { get; set; } = null!;

    public override List<EnemyFormationSlot> Build()
    {
        var centerRow = Helpers.GridData[GameManager.CurrentOrientation].MaxRows / 2;

        return new FormationBuilder()
            .Add(1, centerRow, PrimaryEnemy)
            .Add(0, centerRow, SecondaryEnemy)
            .Add(0, centerRow-1, SecondaryEnemy)
            .Add(0, centerRow+1, PrimaryEnemy)
            .Build();
    }
}
