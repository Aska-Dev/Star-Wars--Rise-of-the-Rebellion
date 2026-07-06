using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum HazardTypes
{
    Asteroid
}

public static class HazardRegistry
{
    private static readonly Dictionary<Type, (HazardTypes Type, string Path)> _database = new()
    {
        { typeof(Asteroid), (HazardTypes.Asteroid, "res://Controller/Hazards/Asteroid/AsteroidController.tscn") }
    };

    private static readonly Dictionary<Type, PackedScene> _cache = new();

    public static PackedScene GetOrLoadScene(Type hazardClassType)
    {
        if (_cache.TryGetValue(hazardClassType, out var cachedScene))
        {
            return cachedScene;
        }

        if (_database.TryGetValue(hazardClassType, out var data))
        {
            var loadedScene = GD.Load<PackedScene>(data.Path);
            _cache[hazardClassType] = loadedScene;
            return loadedScene;
        }

        Log.Error(nameof(EnemyRegistry), nameof(GetOrLoadScene), $"Hazard-Klasse {hazardClassType.Name} ist nicht in der Registry eingetragen!");
        throw new Exception($"Hazard-Klasse {hazardClassType.Name} ist nicht in der Registry eingetragen!");
    }
}