using Godot;
using System;

public partial class EndlessModeScoreLabel : Label
{
	public override void _Ready()
	{
		Callable.From(Setup).CallDeferred();
	}

	private void Setup()
	{
        GameCore.GetFrom(this).EndlessModeManager.ScoreIncreased += (int score) =>
        {
            Text = score.ToString("D5");
        };
    }
}
