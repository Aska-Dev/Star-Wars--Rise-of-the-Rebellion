using Godot;
using System;

[GlobalClass]
public partial class ShipData : Resource
{
    [ExportCategory("General")]
    [Export] public required string Name { get; set; }

    [ExportCategory("Movement")]
    [Export] public required int XSpeed { get; set; }    
    [Export] public required int YSpeed { get; set; }

    [ExportCategory("Combat")]
    [Export] public required int Health { get; set; }
    [Export] public required double FireRate { get; set; } = 1;
    [Export] public SoundEffects ShootSound { get; set;  }
    [Export] public required PackedScene ProjectileScene { get; set; }
    [Export] public required double AltFireRate { get; set; } = 1;
    [Export] public SoundEffects AltShootSound { get; set; }
    [Export] public required PackedScene AltProjectileScene { get; set; }
}
