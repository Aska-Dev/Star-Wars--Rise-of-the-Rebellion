using Godot;
using System;
using System.Collections.Generic;
using static Godot.TextServer;

[GlobalClass]
public partial class ArrowHeadFormation : Formation
{
    [Export] public PackedScene PrimaryEnemy { get; set; } = null!;
    [Export] public PackedScene SecondaryEnemy { get; set; } = null!;

    private const float secondaryEnemyPosition = 0.5f;

    public override List<EnemyFormationSlot> Build()
    {
        var builder = new FormationBuilder();
        var orientation = GameManager.CurrentOrientation;

        int maxRows = Helpers.GridData[orientation].MaxRows;
        int maxColumns = Helpers.GridData[orientation].MaxColumns;

        if (maxRows <= 0 || maxColumns <= 0) return builder.Build();

        // 1. Prüfen ob die Formation seitlich (Left/Right) oder vertikal (Top/Bottom) schaut
        bool isHorizontal = orientation is GameOrientation.Left or GameOrientation.Right;

        if (isHorizontal)
        {
            // SEITLICHE LOGIK (Dein ASCII-Beispiel)
            int centerRow = maxRows / 2;
            int maxDistance = Math.Max(1, centerRow);
            float step = (maxColumns - 1) / (float)maxDistance;

            // Auf exakte Distanz runden, in der der Elite-Gegner sitzen soll
            int eliteDistance = (int)Math.Round(maxDistance * secondaryEnemyPosition, MidpointRounding.AwayFromZero);
            if (eliteDistance == 0 && maxDistance > 0) eliteDistance = 1; // Spitze blockieren

            for (int row = 0; row < maxRows; row++)
            {
                int distance = Math.Abs(row - centerRow);
                int offset = (int)Math.Round(distance * step, MidpointRounding.AwayFromZero);

                // Wenn Left, ist die Spitze (0) vorne. Wenn Right, ist sie hinten.
                int col = (orientation == GameOrientation.Left) ? offset : (maxColumns - 1) - offset;
                col = Math.Clamp(col, 0, maxColumns - 1);

                PackedScene currentEnemy = (distance == eliteDistance && SecondaryEnemy != null)
                    ? SecondaryEnemy
                    : PrimaryEnemy;

                builder.Add(col, row, currentEnemy);
            }
        }
        else
        {
            // VERTIKALE LOGIK (Top/Bottom)
            int centerCol = maxColumns / 2;
            int maxDistance = Math.Max(1, centerCol);
            float step = (maxRows - 1) / (float)maxDistance;

            int eliteDistance = (int)Math.Round(maxDistance * secondaryEnemyPosition, MidpointRounding.AwayFromZero);
            if (eliteDistance == 0 && maxDistance > 0) eliteDistance = 1;

            for (int col = 0; col < maxColumns; col++)
            {
                int distance = Math.Abs(col - centerCol);
                int offset = (int)Math.Round(distance * step, MidpointRounding.AwayFromZero);

                // Wenn Top, ist die Spitze (0) oben.
                int row = (orientation == GameOrientation.Top) ? offset : (maxRows - 1) - offset;
                row = Math.Clamp(row, 0, maxRows - 1);

                PackedScene currentEnemy = (distance == eliteDistance && SecondaryEnemy != null)
                    ? SecondaryEnemy
                    : PrimaryEnemy;

                builder.Add(col, row, currentEnemy);
            }
        }

        return builder.Build();
    }
}
