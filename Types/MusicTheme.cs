using Godot;
using System;

[GlobalClass]
public partial class MusicTheme : Resource
{
    [Export] public MusicThemes Theme { get; set; }
    [Export] public Godot.Collections.Array<AudioStream> Tracks { get; set; } = [];
}
