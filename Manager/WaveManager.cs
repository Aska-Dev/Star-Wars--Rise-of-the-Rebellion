using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public class WaveManager
{
    public event Action WaveCompleted = null!;

    private Dictionary<Type, IWaveHandler> _handlers = [];

    public WaveManager(EnemySpawningManager spawningManager, PlayerEventBus playerEventBus, SceneTree tree)
    {
        // Register wave handlers
        _handlers[typeof(CombatFormationWave)] = new CombatFormationWaveHandler(spawningManager);
        _handlers[typeof(OrientationChangeWave)] = new OrientationChangeWaveHandler(playerEventBus);
        _handlers[typeof(CombatSpawnerWave)] = new CombatSpawnerWaveHandler(spawningManager, tree);
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
