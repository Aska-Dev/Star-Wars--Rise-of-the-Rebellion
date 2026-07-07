using Godot;
using System;
using System.ComponentModel;

[GlobalClass]
public partial class TieInterceptorController : EnemyShipController
{
    public EnemySpawningManager EnemySpawningManager { get; private set; } = null!;

    public override void _Ready()
    {
        base._Ready();

        var core = this.GetGameCore();
        EnemySpawningManager = core.EnemySpawningManager;
    }

    public override void _Process(double delta)
    {
        Components.GetComponent<AnimationComponent>().PlayAnimationSecure("idle");
    }

    public override void PerformAction()
    {
        EnemySpawningManager.ShuffleEnemy(this);
        Components.GetComponent<EnemyWeaponComponent>().ShootSalve();
    }
}
 