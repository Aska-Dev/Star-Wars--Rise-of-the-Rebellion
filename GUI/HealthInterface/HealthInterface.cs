using Godot;
using System;
using System.Collections.Generic;

public partial class HealthInterface : Control
{
	[Export] public required PackedScene HealthIndicatorScene;

	private GridContainer _grid = null!;
	private List<HealthIndicator> _healthIndicators = [];

	public override void _Ready()
	{
		_grid = GetNode<GridContainer>("GridContainer");

		GameCore.Instance.UiEventBus.OnPlayerHealthChanged += UpdateIndicators;

		Callable.From(SetupIndicators).CallDeferred();
	}

	private void SetupIndicators()
	{
		var maxHealth = GameCore.Instance.Player.Components.GetComponent<HealthComponent>().MaxHealth;

		foreach(var child in _grid.GetChildren())
		{
			child.QueueFree();
		}

		_healthIndicators.Clear();

		var amount = maxHealth;

		for(var i = 0; i < amount; i++)
		{
			var indicator = HealthIndicatorScene.Instantiate<HealthIndicator>();
			_grid.AddChild(indicator);
			_healthIndicators.Add(indicator);
		}
	}

	private void UpdateIndicators(int newHealth)
	{
        var highestActiveIndex = Mathf.CeilToInt(newHealth);
		for(var i = 0; i < highestActiveIndex; i++)
		{
			_healthIndicators[i].Switch(true);
		}

		for(var i = highestActiveIndex; i < _healthIndicators.Count; i++)
		{
			_healthIndicators[i].Switch(false);
		}
	}
}
