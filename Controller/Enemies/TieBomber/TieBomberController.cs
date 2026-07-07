using Godot;
using System;

[GlobalClass]
public partial class TieBomberController : EnemyShipController
{
    public override void _Process(double delta)
    {
        Components.GetComponent<AnimationComponent>().PlayAnimationSecure("idle");
    }

    public override void PerformAction()
    {
        Components.GetComponent<EnemyWeaponComponent>().ShootSalve();
    }
}
