using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.InteropServices.Marshalling;

[GlobalClass]
public partial class ModelOrientation : Node2D
{
    [Export]
    public PackedScene ExplosionScene { get; set; } = null!;

    public List<ModelGun> Guns { get; private set; } = [];
    public List<ModelGun> AlternateGuns { get; private set; } = [];

    public override void _Ready()
    {
        InitGuns();
    }

    public void Explode()
    {
        var explosion = ExplosionScene.Instantiate<ShipExplosion>();
        explosion.GlobalPosition = GlobalPosition;
        MasterScene.Instance.AddToScene(explosion);
    }

    private void InitGuns()
    {
        AlternateGuns.Clear();
        Guns.Clear();
        var children = GetChildren();
        foreach (var child in children)
        {
            if(child is ModelGun gun && !gun.IsAlternativeGun)
            {
                Guns.Add(gun);
            }
            else if(child is ModelGun altGun && altGun.IsAlternativeGun)
            {
                AlternateGuns.Add(altGun);
            }
        }
    }
}
