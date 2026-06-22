using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public interface IWaveFactory
{
    public Wave Build();
}

public class CombatSpawnerWaveFactory : IWaveFactory
{
    private const int minEnemyAmount = 4;
    private const int maxEnemyAmount = 12;

    public Wave Build()
    {
        var wave = new CombatSpawnerWave();

        List<EnemyPoolEntry> pool =
        [
            new EnemyPoolEntry()
            {
                EnemyScene = EnemyRegistry.GetRandomOfTier(EnemyTier.Tier1),
                Weight = 5
            },
            new EnemyPoolEntry()
            {
                EnemyScene = EnemyRegistry.GetRandomOfTier(EnemyTier.Tier2),
                Weight = 2
            }
        ];

        wave.EnemyAmount = GD.RandRange(minEnemyAmount, maxEnemyAmount);
        wave.EnemyPool.AddRange(pool);

        return wave;
    }
}

public class CombatFormationWaveFactory : IWaveFactory
{
    private FormationFactory _formationFactory = new FormationFactory();

    public Wave Build()
    {
        var wave = new CombatFormationWave
        {
            Formation = _formationFactory.GenerateRandom()
        };

        return wave;
    }
}

public class OrientationChangeWaveFactory : IWaveFactory
{
    private readonly List<GameOrientation> _orientations = [GameOrientation.Bottom, GameOrientation.Left];

    public Wave Build()
    {
        var orientations = _orientations;
        orientations.Remove(GameManager.CurrentOrientation);

        var randomIndex = GD.RandRange(0, orientations.Count - 1);

        var wave = new OrientationChangeWave
        {
            EnemyOrientation = orientations[randomIndex]
        };

        return wave;
    }
}