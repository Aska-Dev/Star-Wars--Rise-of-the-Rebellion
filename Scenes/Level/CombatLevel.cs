using Godot;
using System;

public partial class CombatLevel : Node2D
{
	public override void _Ready()
	{
		GameCore.GetFrom(this).GameManager.StartEndlessMode();

		GetTree().CreateTimer(1).Timeout += () =>
		{
			PlayerController.GetFrom(this).Components.GetComponent<HealthComponent>().OnDeath += OnPlayerDestroyed;
		};
	}

	private void OnPlayerDestroyed()
	{
		MasterScene.Instance.ChangeScene("Home");
	}
}
