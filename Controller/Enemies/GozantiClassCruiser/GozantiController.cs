using Godot;
using System;

[GlobalClass]
public partial class GozantiController : EnemyShipController
{
    public override void _Process(double delta)
    {
        Components.GetComponent<AnimationComponent>().PlayAnimation("idle");
    }

    public override void PerformAction()
    {
        Components.GetComponent<EnemyWeaponComponent>().ShootAtPlayer();
    }
}
