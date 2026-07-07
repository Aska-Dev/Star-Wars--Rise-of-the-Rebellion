using Godot;
using System;

public partial class CombatLevel : Node2D
{
	public override void _Ready()
	{
		this.GetGameCore().GameManager.StartEndlessMode();

		this.CreateTimer(1).Timeout += () =>
		{
            this.GetPlayer()!.Components.GetComponent<HealthComponent>().OnDeath += OnPlayerDestroyed;
		};
	}

	private void OnPlayerDestroyed()
	{
		MasterScene.Instance.ChangeScene("Home");
	}
}
