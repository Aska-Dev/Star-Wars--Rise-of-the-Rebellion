using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum EnemyTier
{
    Tier1,
    Tier2,
    Tier3
}

public static class EnemyRegistry
{
    private static readonly Dictionary<Type, (EnemyTier Tier, string Path)> _database = new()
    {
        { typeof(TieLnController), (EnemyTier.Tier1, "res://Controller/Enemies/TieLn/TieLnController.tscn") }, 
        { typeof(TieBomberController), (EnemyTier.Tier2, "res://Controller/Enemies/TieBomber/TieBomberController.tscn") }, 
        { typeof(TieInterceptorController), (EnemyTier.Tier2, "res://Controller/Enemies/TieInterceptor/TieInterceptorController.tscn") }, 
    };

    private static readonly Dictionary<Type, PackedScene> _cache = new();

    public static PackedScene GetOrLoadScene(Type enemyClassType)
    {
        if (_cache.TryGetValue(enemyClassType, out var cachedScene))
        {
            return cachedScene;
        }

        if (_database.TryGetValue(enemyClassType, out var data))
        {
            var loadedScene = GD.Load<PackedScene>(data.Path);
            _cache[enemyClassType] = loadedScene;
            return loadedScene;
        }

        Log.Error(nameof(EnemyRegistry), nameof(GetOrLoadScene), $"Gegner-Klasse {enemyClassType.Name} ist nicht in der Registry eingetragen!");
        throw new Exception($"Gegner-Klasse {enemyClassType.Name} ist nicht in der Registry eingetragen!");
    }

    public static PackedScene GetRandomOfTier(EnemyTier tier)
    {
        var matchingTypes = _database.Where(kvp => kvp.Value.Tier == tier).Select(kvp => kvp.Key).ToList();

        if (matchingTypes.Count == 0)
        {
            Log.Error(nameof(EnemyRegistry), nameof(GetRandomOfTier), $"Keine Gegner für {tier} gefunden! Fallback auf Tier1.");
            return GetRandomOfTier(EnemyTier.Tier1);
        }

        var randomIndex = GD.RandRange(0, matchingTypes.Count - 1);
        Type randomType = matchingTypes[randomIndex];

        return GetOrLoadScene(randomType);
    }
}