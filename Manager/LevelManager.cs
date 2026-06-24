using Godot;
using System;

[GlobalClass]
public partial class LevelManager : Node
{
    // --- EVENTS ---
    public event Action OnLevelComplete = null!;

    // --- EXPORTS ---
    [Export] public WaveManager WaveManager { get; set; } = null!;

    // --- FIELDS ---
    private Level _currentLevel = null!;
    private int _currentWaveIndex = 0;

    public void StartLevel(Level level)
    {
        _currentLevel = level;
        _currentWaveIndex = 0;

        WaveManager.WaveCompleted += OnWaveCompleted;
        WaveManager.PlayWave(_currentLevel.Waves[_currentWaveIndex]);
    }

    private void OnWaveCompleted()
    {
        _currentWaveIndex++;

        if (_currentLevel.Waves.Length < _currentWaveIndex)
        {
            OnLevelComplete?.Invoke();
            return;
        }

        WaveManager.PlayWave(_currentLevel.Waves[_currentWaveIndex]);
    }
}
