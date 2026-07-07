using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class HazardRegistry
{
    private static readonly Dictionary<Type, string> _database = new()
    {
        { typeof(Asteroid), "res://Controller/Hazards/Asteroid/AsteroidController.tscn"},
        { typeof(FlankingTieController), "res://Controller/Hazards/FlankingTie/FlankingTieController.tscn" }
    };

    private static readonly Dictionary<Type, PackedScene> _cache = new();

    public static PackedScene GetOrLoadScene(Type hazardClassType)
    {
        if (_cache.TryGetValue(hazardClassType, out var cachedScene))
        {
            return cachedScene;
        }

        if (_database.TryGetValue(hazardClassType, out var path))
        {
            var loadedScene = GD.Load<PackedScene>(path);
            _cache[hazardClassType] = loadedScene;
            return loadedScene;
        }

        Log.Error(nameof(HazardRegistry), nameof(GetOrLoadScene), $"Hazard-Klasse {hazardClassType.Name} ist nicht in der Registry eingetragen!");
        throw new Exception($"Hazard-Klasse {hazardClassType.Name} ist nicht in der Registry eingetragen!");
    }
}