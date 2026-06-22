using Godot;
using System;

[GlobalClass]
public partial class EnemyPoolEntry : Resource
{
    [Export] public required PackedScene EnemyScene;
    [Export] public float Weight = 1;
}
