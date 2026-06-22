using System;

public class LevelManager(WaveManager waveManager)
{
    public event Action OnLevelComplete = null!;

    private WaveManager _waveManager = waveManager;

    private Level _currentLevel = null!;
    private int _currentWaveIndex = 0;

    public void StartLevel(Level level)
    {
        _currentLevel = level;
        _currentWaveIndex = 0;

        _waveManager.WaveCompleted += OnWaveCompleted;
        _waveManager.PlayWave(_currentLevel.Waves[_currentWaveIndex]);
    }

    private void OnWaveCompleted()
    {
        _currentWaveIndex++;

        if (_currentLevel.Waves.Length < _currentWaveIndex)
        {
            OnLevelComplete?.Invoke();
            return;
        }

        _waveManager.PlayWave(_currentLevel.Waves[_currentWaveIndex]);
    }
}
