using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public interface IWaveHandler
{
    public event Action WaveCompleted;
    public void Handle(Wave wave);
}

public abstract class WaveHandler : IWaveHandler
{
    public event Action WaveCompleted = null!;
    public abstract void Handle(Wave wave);

    protected void CompleteWave()
    {
        WaveCompleted?.Invoke();
        GD.Print("Wave completed");
    }
}

public class CombatFormationWaveHandler : WaveHandler
{
    private EnemySpawningManager enemySpawningManager;

    private int _formationSize;
    private int _shufflesLeft;
    private float _shuffleChance;
    
    public CombatFormationWaveHandler(EnemySpawningManager _enemySpawningManager)
    {
        enemySpawningManager = _enemySpawningManager;

        enemySpawningManager.OnEnemyDefeated += CheckForShuffle;
        enemySpawningManager.OnAllEnemiesDefeated += CompleteWave;
    }

    public override void Handle(Wave wave)
    {
        if(wave is not CombatFormationWave combat)
        {
            Log.Error(nameof(CombatFormationWaveHandler), nameof(Handle), "Provided wave resource is not of type CombatWave");
            return;
        }

        var builtFormation = combat.Formation.Build();

        _formationSize = builtFormation.Count;
        _shufflesLeft = combat.Shuffles;
        _shuffleChance = combat.ShuffleChance;

        enemySpawningManager.SpawnFormation(builtFormation);
    }

    private void CheckForShuffle()
    {
        if (_shufflesLeft <= 0)
        {
            return;
        }

        var enemiesCount = enemySpawningManager.EnemyAmount;
        float alivePercentage = (float)enemiesCount / (float)_formationSize;
        GD.Print(alivePercentage);

        if(alivePercentage <= 0.7f && GD.Randf() <= _shuffleChance)
        {
            _shufflesLeft--;
            enemySpawningManager.ShuffleGrid();
        }
    }
}

public class OrientationChangeWaveHandler : WaveHandler
{
    private PlayerController _player;

    public OrientationChangeWaveHandler(PlayerEventBus playerEventBus, PlayerController player)
    {
        playerEventBus.OnPlayerOrientationChanged += CompleteWave;
        _player = player;
    }

    public override void Handle(Wave wave)
    {
        if (wave is not OrientationChangeWave orientationChangeWave)
        {
            Log.Error(nameof(OrientationChangeWaveHandler), nameof(Handle), "Provided wave resource is not of type OrientationChangeWave");
            return;
        }

        _player.ChangeOrientation(orientationChangeWave.EnemyOrientation);
    }
}

public class CombatSpawnerWaveHandler : WaveHandler
{
    private EnemySpawningManager enemySpawningManager;
    private SceneTree tree;

    private int _enemiesAmount;
    private CombatSpawnerWave _wave = null!;
    private SceneTreeTimer? _timer = null;

    public CombatSpawnerWaveHandler(EnemySpawningManager _enemySpawningManager, SceneTree _tree)
    {
        enemySpawningManager = _enemySpawningManager;
        tree = _tree;

        _enemySpawningManager.OnAllEnemiesDefeated += OnGridEmpty;
    }

    public override void Handle(Wave wave)
    {
        if (wave is not CombatSpawnerWave spawnerWave)
        {
            Log.Error(nameof(CombatSpawnerWaveHandler), nameof(Handle), $"Provided wave resource is not of type {nameof(CombatSpawnerWave)}");
            return;
        }

        _enemiesAmount = spawnerWave.EnemyAmount;
        _wave = spawnerWave;

        HandleSpawner();
    }

    private void OnGridEmpty()
    {
        if(_enemiesAmount <= 0)
        {
            CompleteWave();
        }
        else
        {
            SpawnBatch(_wave.MaximumBatchSize, _wave.MaximumBatchSize);
            
            _timer = tree.CreateTimer(_wave.SpawnInterval);
            _timer.Timeout += HandleSpawner;
        }
    }

    private void HandleSpawner()
    {
        if(_enemiesAmount <= 0)
        {
            return;
        }

        var freeSlots = enemySpawningManager.GetFreeSlots();

        if (freeSlots.Count >= _wave.MinimumBatchSize)
        {
            var batchMaxSize = Math.Min(_wave.MaximumBatchSize, Math.Min(_enemiesAmount, freeSlots.Count));
            batchMaxSize = Math.Max(_wave.MinimumBatchSize, batchMaxSize);

            SpawnBatch(_wave.MinimumBatchSize, batchMaxSize);
        }

        // START TIMER
        _timer = tree.CreateTimer(_wave.SpawnInterval);
        _timer.Timeout += HandleSpawner;
    }

    private void SpawnBatch(int minAmount, int maxAmount)
    {
        var amount = GD.RandRange(minAmount, maxAmount);
        _enemiesAmount -= amount;
        for(var i = 0; i < amount; i++)
        {
            var position = GetRandomFreeSlot();
            var enemy = GetRandomEnemy();

            enemySpawningManager.SpawnEnemy(enemy, position.Column, position.Row);
        }
    }

    private EnemyGridSlot GetRandomFreeSlot()
    {
        var freeSlots = enemySpawningManager.GetFreeSlots();

        var randomSlotIndex = GD.RandRange(0, freeSlots.Count - 1);
        return freeSlots[randomSlotIndex];
    }

    private PackedScene GetRandomEnemy()
    {
        var totalWeight = _wave.EnemyPool.Sum(e => e.Weight);
        var rollValue = GD.Randf() * totalWeight;

        foreach(var entry in _wave.EnemyPool)
        {
            rollValue -= entry.Weight;
            if(rollValue <= 0)
            {
                return entry.EnemyScene;
            }
        }

        return _wave.EnemyPool.First().EnemyScene;
    }
}