using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

[GlobalClass]
public partial class WaveManager : Node
{
    // --- EVENT ---
    public event Action WaveCompleted = null!;

    // --- EXPORTS
    [Export] public EnemySpawningManager EnemySpawningManager { get; set; } = null!;
    [Export] public HazardSpawningManager HazardSpawningManager { get; set; } = null!;
    [Export] public PlayerController PlayerController { get; set; } = null!;

    // -- FIELDS ---
    private Dictionary<Type, IWaveHandler> _handlers = [];

    public override void _Ready()
    {
        // Register wave handlers
        _handlers[typeof(CombatFormationWave)] = new CombatFormationWaveHandler(EnemySpawningManager);
        _handlers[typeof(OrientationChangeWave)] = new OrientationChangeWaveHandler(EventHub.Instance.PlayerEventBus, PlayerController);
        _handlers[typeof(CombatSpawnerWave)] = new CombatSpawnerWaveHandler(EnemySpawningManager, GetTree());
        _handlers[typeof(AsteroidStormWave)] = new AsteroidStormWaveHandler(GetTree(), HazardSpawningManager);
        _handlers[typeof(GozantiCruiserWave)] = new GozantiCruiserWaveHandler(EnemySpawningManager);
    }
     
    public void PlayWave(Wave wave)
    {
        var waveType = wave.GetType();

        if (_handlers.TryGetValue(waveType, out var handler))
        {
            void onComplete()
            {
                handler.WaveCompleted -= onComplete;
                WaveCompleted?.Invoke();
            }

            handler.WaveCompleted += onComplete;
            handler.Handle(wave);
        }
    }
}
