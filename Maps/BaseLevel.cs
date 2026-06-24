using Godot;
using System;

public partial class BaseLevel : Node2D
{
	public override void _Ready()
	{
		GameCore.Instance.GameManager.Test();

		GetTree().CreateTimer(1).Timeout += () =>
		{
			GameCore.Instance.Player.Components.GetComponent<HealthComponent>().OnDeath += OnPlayerDestroyed;
		};
	}

	private void OnPlayerDestroyed()
	{
	}
}
