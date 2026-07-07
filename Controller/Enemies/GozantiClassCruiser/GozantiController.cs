using Godot;
using System;

[GlobalClass]
public partial class GozantiController : EnemyShipController
{
    private const int shotsUntilMove = 2;
    private int _shotsFired = 0;

    private bool _firstTiesLaunched = false;
    private bool _secondTiesLaunched = false;

    public override void _Ready()
    {
        base._Ready();
        FlyInSpeed = 200f;
    }

    public override void _Process(double delta)
    { 
        Components.GetComponent<AnimationComponent>().PlayAnimationSecure("idle");

        var healthComponent = Components.GetComponent<HealthComponent>();
        if (healthComponent.Health <= healthComponent.MaxHealth / 2 && !_secondTiesLaunched)
        {
            SpawnTieEscort();
            _secondTiesLaunched = true;
        }
    }

    public override void PerformAction()
    {
        if(!_firstTiesLaunched)
        {
            GD.Print("Spawning Tie Escort");

            SpawnTieEscort();
            _firstTiesLaunched = true;
        }
        else
        {
            Components.GetComponent<EnemyWeaponComponent>().ShootAtPlayer();
            _shotsFired++;
        }

        if(_shotsFired >= shotsUntilMove)
        {
            var enemySpawningManager = this.GetGameCore().EnemySpawningManager;
            enemySpawningManager.ShuffleEnemy(this);

            _shotsFired = 0;
        }
    }

    private void SpawnTieEscort()
    {
        var gozantiShipModel = shipModel as GozantiShipModel;
        var tieAnimationPlayer = gozantiShipModel!.TieAnimationPlayer;

        if (!_firstTiesLaunched)
        {
            tieAnimationPlayer.Play("tieOneFlyingOff");       
        }
        else
        {
            tieAnimationPlayer.Play("tieTwoFlyingOff");       
        }

        void OnAnimationFinished(StringName animationName)
        {
            if(animationName == "tieOneFlyingOff" || animationName == "tieTwoFlyingOff")
            {
                tieAnimationPlayer.AnimationFinished -= OnAnimationFinished;
                SpawnFlankingTieHazard();
            }
        }

        tieAnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    private void SpawnFlankingTieHazard()
    {
        var flankingTieHazardScene = HazardRegistry.GetOrLoadScene(typeof(FlankingTieController));

        var hazardSpawningManager = this.GetGameCore().HazardSpawningManager;
        hazardSpawningManager.SpawnHazardAtPlayer(flankingTieHazardScene, GameManager.CurrentOrientation.GetPerpendicular());
        this.CreateTimer(1).Timeout += () =>
        {
            hazardSpawningManager.SpawnHazardAtPlayer(flankingTieHazardScene, GameManager.CurrentOrientation.GetPerpendicular());
        };

        void OnHazardsClearedOnce()
        {
            hazardSpawningManager.OnAllHazardsCleared -= OnHazardsClearedOnce;
            SpawnTieEnemies();
        }

        hazardSpawningManager.OnAllHazardsCleared += OnHazardsClearedOnce;
    }

    private void SpawnTieEnemies()
    {
        var tieEnemyScene = EnemyRegistry.GetOrLoadScene(typeof(TieLnController));
        var enemySpawningManager = this.GetGameCore().EnemySpawningManager;

        enemySpawningManager.SpawnEnemyRandom(tieEnemyScene);
        enemySpawningManager.SpawnEnemyRandom(tieEnemyScene);
    }
}
