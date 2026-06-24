using Godot;
using System;

public partial class GameCore : Node
{
	public static GameCore Instance { get; private set; } = null!;

	// --- MANAGERS ---
	public GameManager GameManager { get; private set; } = null!;
	public EnemySpawningManager EnemySpawningManager { get; private set; } = null!;
	public LevelManager LevelManager { get; private set; } = null!;
	public WaveManager WaveManager { get; private set; } = null!;
	public EndlessModeManager EndlessModeManager { get; private set; } = null!;

	// --- EVENT BUSSES ---
	public UiEventBus UiEventBus { get; private set; } = null!;
	public PlayerEventBus PlayerEventBus { get; private set; } = null!;

	// --- GLOBAL REFERENCES ---
	public PlayerController Player { get; set; } = null!;

	public override void _Ready()
	{
		Instance = this;

		UiEventBus = new();
        PlayerEventBus = new();

		EnemySpawningManager = new(GetTree(), GetViewport());
		WaveManager = new(EnemySpawningManager, PlayerEventBus, GetTree());
		LevelManager = new(WaveManager);
		EndlessModeManager = new(WaveManager);
		GameManager = new(LevelManager, EndlessModeManager);
    }
}
