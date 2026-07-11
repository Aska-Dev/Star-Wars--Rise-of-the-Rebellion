using Godot;
using System;

[GlobalClass]
public partial class TieDefenderController : EnemyShipController
{
    public EnemySpawningManager EnemySpawningManager { get; private set; } = null!;
    public HazardSpawningManager HazardSpawningManager { get; private set; } = null!;

    private bool _hasMoved = false;

    public override void _Ready()
    {
        base._Ready();

        var core = this.GetGameCore();
        EnemySpawningManager = core.EnemySpawningManager;
        HazardSpawningManager = core.HazardSpawningManager;
    }

    public override void Init(Vector2 spawnPosition)
    {
        var flankingHazardScene = HazardRegistry.GetOrLoadScene(typeof(FlankingTieDefenderController)) ?? throw new Exception("FlankingTieDefenderController scene could not be loaded");
        var token = HazardSpawningManager.SpawnHazardAtPlayer(flankingHazardScene, GameManager.CurrentOrientation.GetPerpendicular());

        token.Completed += () =>
        {
            base.Init(spawnPosition);
        };
    }

    public override void _Process(double delta)
    {
        GD.Print("TieDefenderController _Process");
        Components.GetComponent<AnimationComponent>().PlayAnimation("idle");
    }

    public override void PerformAction()
    {
        if(!_hasMoved)
        {
            _hasMoved = true;
            EnemySpawningManager.ShuffleEnemy(this);
            Components.GetComponent<EnemyWeaponComponent>().ShootAsyncSalve();
        }
        else
        {
            // Shoot with alternative guns
            _hasMoved = false;
            Components.GetComponent<EnemyWeaponComponent>().Shoot(true);
        }
    }
}