using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Level : Resource
{
    [Export] public string Name { get; set; } = "";
    [Export] public Wave[] Waves { get; set; } = [];
}
