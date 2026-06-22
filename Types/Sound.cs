using Godot;
using System;

[GlobalClass]
public partial class Sound : Resource
{
    [Export] public SoundEffect EffectName { get; set; }

    [Export] public required AudioStream PrimaryStream { get; set; }
    [Export] public AudioStream? AlternateStream { get; set; }
}
