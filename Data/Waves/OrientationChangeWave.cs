using Godot;
using System;

[GlobalClass]
public partial class OrientationChangeWave : Wave
{
    [Export] public GameOrientation EnemyOrientation { get; set; } = GameOrientation.Top;
}
