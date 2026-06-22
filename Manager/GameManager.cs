
using Godot;
using System;
using System.Net.Http.Headers;

public class GameManager(LevelManager levelManager, EndlessModeManager endlessModeManager)
{
    public static GameOrientation CurrentOrientation { get; set; } = GameOrientation.Left;
    
    public LevelManager LevelManager = levelManager;
    public EndlessModeManager EndlessModeManager = endlessModeManager;
    
    public void Test()
    {
        AudioEngine.Instance.PlayTheme(MusicThemes.SpaceBattle);
        EndlessModeManager.NextWave();
    }
    
}