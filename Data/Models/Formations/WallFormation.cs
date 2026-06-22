using Godot;
using System;
using System.Collections.Generic;
using static Godot.TextServer;

[GlobalClass]
public partial class WallFormation : Formation
{
    [Export] public PackedScene Enemy { get; set; } = null!;

    private const float delay = 0.3f;

    public override List<EnemyFormationSlot> Build()
    {
        var builder = new FormationBuilder();
        int maxRows = Helpers.GridData[GameManager.CurrentOrientation].MaxRows;

        int topRow = 0;
        int bottomRow = maxRows - 1;

        while (topRow <= bottomRow)
        {
            builder.Add(0, topRow, Enemy, delay);

            if (topRow != bottomRow)
            {
                builder.Add(0, bottomRow, Enemy, 0f);
            }

            topRow++;
            bottomRow--;
        }

        return builder.Build();
    }
}
