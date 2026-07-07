using Godot;
using System.Collections.Generic;
using System.Linq;

public interface IWaveRule
{
    int Priority { get; }
    Wave? Evaluate(IReadOnlyList<Wave> history, int beatenWaves);
}

public class OrientationChangeRule : IWaveRule
{
    public int Priority => 20;

    public Wave? Evaluate(IReadOnlyList<Wave> history, int beatenWaves)
    {
        int wavesSinceLastOrientationChange = history.Reverse()
            .TakeWhile(w => w is not OrientationChangeWave)
            .Count();

        GD.Print(wavesSinceLastOrientationChange);

        if (wavesSinceLastOrientationChange < 2) return null;

        float chance = (wavesSinceLastOrientationChange - 1) * 0.20f;

        if (GD.Randf() <= chance)
        {
            var factory = new OrientationChangeWaveFactory();
            return factory.Build();
        }

        return null;
    }
}

public class GozantiCruiserWaveRule : IWaveRule
{
    public int Priority => 30;

    public Wave? Evaluate(IReadOnlyList<Wave> history, int beatenWaves)
    {
        int wavesSinceLastBossWave = history.Reverse()
            .TakeWhile(w => w is not GozantiCruiserWave)
            .Count();

        if (wavesSinceLastBossWave >= 1)
        {
            var factory = new GozantiCruiserWaveFactory();
            return factory.Build();
        }

        return null;
    }
}

public class AsteroidStormWaveRule : IWaveRule
{
    public int Priority => 10;

    public Wave? Evaluate(IReadOnlyList<Wave> history, int beatenWaves)
    {
        var lastWave = history.Reverse().FirstOrDefault();

        if (lastWave is AsteroidStormWave) return null;

        float chance = 0.25f;

        if (GD.Randf() <= chance)
        {
            var factory = new AsteroidStormWaveFactory();
            return factory.Build();
        }

        return null;
    }
}

public class CombatWaveRule : IWaveRule
{
    public int Priority => 0;

    private List<IWaveFactory> _combatFactories = new()
    {
        new CombatFormationWaveFactory(),
        new CombatSpawnerWaveFactory()
    };

    public Wave Evaluate(IReadOnlyList<Wave> history, int beatenWaves)
    {
        var randomIndex = GD.RandRange(0, _combatFactories.Count - 1);
        return _combatFactories[randomIndex].Build();
    }
}