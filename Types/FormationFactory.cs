using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class FormationFactory
{
    private const float _enemyTypeDowngradeChance = 0.3f;
    private readonly List<Type> _formations = [];

    public FormationFactory()
    {
        var baseType = typeof(Formation);
        _formations = [.. Assembly.GetExecutingAssembly().GetTypes().Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && t != baseType)];
    }

    public Formation GenerateRandom()
    {
        Type randomFormation = _formations[GD.RandRange(0, _formations.Count - 1)];
        var formation = Activator.CreateInstance(randomFormation) as Formation;

        var enemyTypes = GetEnemyScenes(formation!.RequiredEnemyCount);
        formation.InjectEnemyTypes(enemyTypes);

        return formation; 
    }

    private List<PackedScene> GetEnemyScenes(int amount)
    {
        List<PackedScene> enemyTypes = [];

        for(int i = 0; i < amount; i++)
        {
            var tier = Math.Clamp(i, 0, Enum.GetValues(typeof(EnemyTier)).Length-1);

            if(tier > 0 && GD.Randf() <= _enemyTypeDowngradeChance)
            {
                tier--;
            }

            var randomEnemy = EnemyRegistry.GetRandomOfTier((EnemyTier)tier);
            enemyTypes.Add(randomEnemy);
        }

        return enemyTypes;
    }
}
