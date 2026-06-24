using Godot;
using System;

[GlobalClass]
public partial class GameCore : Node
{
    // --- MANAGERS ---
    [Export] public GameManager GameManager { get; private set; } = null!;
    [Export] public EnemySpawningManager EnemySpawningManager { get; private set; } = null!;
    [Export] public LevelManager LevelManager { get; private set; } = null!;
    [Export] public WaveManager WaveManager { get; private set; } = null!;
    [Export] public EndlessModeManager EndlessModeManager { get; private set; } = null!;

	// -- FIELDS ---
	private const string GroupName = nameof(GameCore);

	public override void _Ready()
	{
        AddToGroup(GroupName);
    }

    public override void _ExitTree()
    {
        if (IsInGroup(GroupName))
        {
            RemoveFromGroup(GroupName);
        }
    }

    public static GameCore GetFrom(Node caller)
    {
        var core = caller.GetTree().GetFirstNodeInGroup(GroupName) as GameCore;
		if(core is null)
		{
			throw new NullReferenceException(nameof(core));
		}

		return core;
    }
}