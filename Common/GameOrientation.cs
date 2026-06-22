using Godot;
using System;
using System.Collections.Generic;

public enum GameOrientation
{
    TopLeft,
    TopRight,
    Top,
    Left,
    Right,
    Bottom,
    BottomLeft,
    BottomRight,
}

public struct OrientationGridData
{
    public int MaxRows;
    public int MaxColumns;
}
