
using Godot;
using System;
using System.Net.Http.Headers;

[GlobalClass]
public partial class GameManager : Node
{
    [Export] public LevelManager LevelManager { get; set; } = null!;
    [Export] public EndlessModeManager EndlessModeManager { get; set; } = null!;
    
    public static GameOrientation CurrentOrientation { get; set; } = GameOrientation.Left;
        
    public void StartEndlessMode()
    {
        AudioEngine.Instance.PlayTheme(MusicThemes.SpaceBattle);
        EndlessModeManager.NextWave();
    }
    
}