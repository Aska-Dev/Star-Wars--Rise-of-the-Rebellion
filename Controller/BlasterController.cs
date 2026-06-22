using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BlasterController : ProjectileController
{
    public override void _PhysicsProcess(double delta)
    {
        Velocity = Velocity.Normalized() * Speed;

        base._PhysicsProcess(delta);
    }
}
