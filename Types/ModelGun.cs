using Godot;
using System;

[GlobalClass]
public partial class ModelGun : Marker2D
{
    [Export] public bool IsAlternativeGun { get; set; } = false;
}
