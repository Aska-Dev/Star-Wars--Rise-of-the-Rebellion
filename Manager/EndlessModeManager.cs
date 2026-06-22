using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

public class EndlessModeManager
{
    public int BeatenWaves { get; private set;  } = 0;

    private WaveManager _waveManager = null!;

    private List<Wave> _history = [];
    private const int maxHistory = 10;

    private List<IWaveRule> _waveRules = [];

    public EndlessModeManager(WaveManager waveManager)
    {
        _waveManager = waveManager;
        _waveManager.WaveCompleted += OnWaveComplete;

        InitializeRules();
    }

    public void NextWave()
    {
        Wave? nextWave = null;

        foreach (var rule in _waveRules)
        {
            nextWave = rule.Evaluate(_history, BeatenWaves);
            if (nextWave is not null) break;
        }

        if(nextWave is null)
        {
            throw new Exception("");
        }

        RecordWave(nextWave);

        _waveManager.PlayWave(nextWave);
    }

    private void OnWaveComplete()
    {
        BeatenWaves++;
        NextWave();
    }

    private void RecordWave(Wave wave)
    {
        _history.Add(wave);
        if(_history.Count > maxHistory)
        {
            _history.RemoveAt(0);
        }
    }

    private void InitializeRules()
    {
        var ruleType = typeof(IWaveRule);

        var ruleTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => ruleType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in ruleTypes)
        {
            _waveRules.Add((IWaveRule)Activator.CreateInstance(type)!);
        }

        _waveRules = _waveRules.OrderByDescending(r => r.Priority).ToList();
    }
}
