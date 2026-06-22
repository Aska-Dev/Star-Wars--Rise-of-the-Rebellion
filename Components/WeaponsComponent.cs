using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class WeaponsComponent : Component
{
    private PackedScene _projectilScene = null!;
    private ShipModel _shipModel = null!;

    private Timer _weaponCooldownTimer = null!;

    public override void _Ready()
    {
        base._Ready();

        _weaponCooldownTimer = GetNode<Timer>("WeaponCooldown");
    }

    public void Shoot()
    {
        if (_weaponCooldownTimer.TimeLeft > 0)
        {
            return;
        }

        var fireDirection = GameManager.CurrentOrientation.GetOpposite().GetDirectionVector();

        foreach (var gun in _shipModel.ActiveOrientation.Guns)
        {
            var projectile = _projectilScene.Instantiate<ProjectileController>();

            GetTree().Root.AddChild(projectile);

            projectile.GlobalPosition = gun.GlobalPosition;
            projectile.Launch(fireDirection.Rotated(gun.Rotation), 2);
        }

        _weaponCooldownTimer.Start();
    }

    protected override void Initialize(ShipData shipData, ShipModel shipModel)
    {
        // Initialize guns based on the ship model
        _shipModel = shipModel;

        _projectilScene = shipData.ProjectileScene;
        _weaponCooldownTimer.WaitTime = shipData.FireRate;
    }
}
