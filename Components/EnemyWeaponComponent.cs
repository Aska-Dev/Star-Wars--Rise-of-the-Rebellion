using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class EnemyWeaponComponent : Component
{
    [Export] public int SalveAmount { get; set; } = 3;
    [Export] public double SalveInterval { get; set; } = 0.2;

    private const float AsyncInterval = 0.02f;

    private PackedScene _projectilScene = null!;
    private readonly List<ModelGun> _guns = [];

    private SoundEffects _shootSound;

    public void ShootSalve()
    {
        var interval = 0.0;
        for (var i = 0; i < SalveAmount; i++)
        {
            this.CreateTimer(interval).Timeout += Shoot;
            interval += SalveInterval;
        }
    }

    public void ShootAsyncSalve()
    {
        float durationOfOneSalve = _guns.Count * AsyncInterval;

        var interval = durationOfOneSalve + SalveInterval;
        for (var i = 0; i < SalveAmount; i++)
        {
            this.CreateTimer(interval).Timeout += ShootAllGunsAsync;
            interval *= i;
        }
    }

    public void Shoot()
    {
        var fireDirection = GameManager.CurrentOrientation.GetDirectionVector();

        foreach (var gun in _guns)
        {
            ShootGun(gun, fireDirection);
        }

        AudioEngine.Instance.PlaySound(_shootSound, true);
    }

    public void ShootAtPlayer()
    {
        var player = this.GetPlayer();
        if (player == null) return;

        foreach (var gun in _guns)
        {
            var fireDirection = (player.GlobalPosition - gun.GlobalPosition).Normalized();
            ShootGun(gun, fireDirection);
        }

        AudioEngine.Instance.PlaySound(_shootSound, true);
    }

    public void ShootAllGunsAsync()
    {
        var fireDirection = GameManager.CurrentOrientation.GetDirectionVector();

        for(var i = 0; i < _guns.Count; i++)
        {
            var index = i;
            var interval = AsyncInterval * index;
            this.CreateTimer(interval).Timeout += () => ShootGun(_guns[index], fireDirection);
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

    private void ShootGun(ModelGun gun, Vector2 fireDirection)
    {
        if (!GodotObject.IsInstanceValid(gun)) return;

        var projectile = _projectilScene.Instantiate<ProjectileController>();

        MasterScene.Instance.AddToScene(projectile);

        projectile.GlobalPosition = gun.GlobalPosition;
        projectile.Launch(fireDirection, 1);
    }
}
