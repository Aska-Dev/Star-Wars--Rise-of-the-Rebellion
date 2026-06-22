using Godot;
using System;
using System.Collections.Generic;

public class FormationBuilder
{
    public List<EnemyFormationSlot> Slots { get; set; } = [];

    public FormationBuilder Add(int col, int row, PackedScene enemy, float delay = 0)
    {
        Slots.RemoveAll(s => s.Column == col && s.Row == row);
        Slots.Add(new(col, row, enemy, delay));
        return this;
    }

    public FormationBuilder FillColumn(int col, PackedScene enemy, GameOrientation orientation)
    {
        int maxRows = Helpers.GridData[orientation].MaxRows;
        for (int row = 0; row < maxRows; row++)
        {
            Add(col, row, enemy);
        }
        return this;
    }

    public FormationBuilder FillRow(int row, PackedScene enemy, GameOrientation orientation)
    {
        int maxCols = Helpers.GridData[orientation].MaxColumns;
        for (int col = 0; col < maxCols; col++)
        {
            Add(col, row, enemy);
        }
        return this;
    }

    public FormationBuilder AddBlock(int startCol, int endCol, int startRow, int endRow, PackedScene enemy)
    {
        for (int c = startCol; c <= endCol; c++)
        {
            for (int r = startRow; r <= endRow; r++)
            {
                Add(c, r, enemy);
            }
        }
        return this;
    }

    public List<EnemyFormationSlot> Build()
    {
        return Slots;
    }
}