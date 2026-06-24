using Godot;
using System;

public partial class EndlessModeScoreLabel : Label
{
	public override void _Ready()
	{
		GameCore.GetFrom(this).EndlessModeManager.ScoreIncreased += (int score) =>
		{
			Text = score.ToString("D5");
		};
	}
}
