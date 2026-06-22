using Godot;
using System;
using System.Collections.Generic;

public struct SpawnGridModule
{
    private Dictionary<GameOrientation, EnemyGridData> _gridData = [];

    public SpawnGridModule(Vector2 viewportSize)
    {
        _gridData[GameOrientation.Left] = new()
        {
            Origin = new Vector2(viewportSize.X * 0.95f, viewportSize.Y * 0.05f),
            ColumnDirection = Vector2.Left,
            RowDirection = Vector2.Down,
            ColumnSpacing = viewportSize.X * 0.04f,
            RowSpacing = viewportSize.Y * 0.1f,
        };
        _gridData[GameOrientation.Bottom] = new()
        {
            Origin = new Vector2(viewportSize.X * 0.2f, viewportSize.Y * 0.05f),
            ColumnDirection = Vector2.Down,
            RowDirection = Vector2.Right,
            ColumnSpacing = viewportSize.X * 0.04f,
            RowSpacing = viewportSize.Y * 0.1f,
        };
    }

    public readonly Vector2 GetWorldPosition(int column, int row, GameOrientation orientation)
    {
        var config = _gridData[orientation];

        Vector2 colOffset = config.ColumnDirection * (column * config.ColumnSpacing);
        Vector2 rowOffset = config.RowDirection * (row * config.RowSpacing);

        return config.Origin + colOffset + rowOffset;
    }
}
