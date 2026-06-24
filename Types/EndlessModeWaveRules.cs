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
        int consecutiveCombats = history.Reverse()
            .TakeWhile(w => w is CombatFormationWave || w is CombatSpawnerWave)
            .Count();

        if (consecutiveCombats < 2) return null;

        float chance = (consecutiveCombats - 1) * 0.20f;

        if (GD.Randf() <= chance)
        {
            var factory = new OrientationChangeWaveFactory();
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