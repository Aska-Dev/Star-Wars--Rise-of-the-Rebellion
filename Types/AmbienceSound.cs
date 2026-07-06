using Godot;
using System;

[GlobalClass]
public partial class AmbienceSound : Resource
{
    [Export] public required AmbienceThemes Theme { get; set; }
    [Export] public required AudioStream Stream { get; set; }
}
