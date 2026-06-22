using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class EnemyWeaponComponent : Component
{
    [Export] public int SalveAmount { get; set; } = 3;
    [Export] public double SalveInterval { get; set; } = 0.2;

    private PackedScene _projectilScene = null!;
    private readonly List<ModelGun> _guns = [];

    private SoundEffect _shootSound;

    public void ShootSalve()
    {
        var interval = 0.0;
        for (var i = 0; i < SalveAmount; i++)
        {
            GetTree().CreateTimer(interval).Timeout += () =>
            {
                Shoot();
            };
            interval += SalveInterval;
        }
    }

    public void Shoot()
    {
        var fireDirection = GameManager.CurrentOrientation.GetDirectionVector();

        foreach (var gun in _guns)
        {
            var projectile = _projectilScene.Instantiate<ProjectileController>();

            GetTree().Root.AddChild(projectile);

            projectile.GlobalPosition = gun.GlobalPosition;
            projectile.Launch(fireDirection, 1);
        }

        AudioEngine.Instance.PlaySound(_shootSound, true);
    }

    protected override void Initialize(ShipData shipData, ShipModel shipModel)
    {
        _projectilScene = shipData.ProjectileScene;
        
        _guns.Clear();
        _guns.AddRange(shipModel.ActiveOrientation.Guns);

        _shootSound = shipData.ShootSound;
    }
}
