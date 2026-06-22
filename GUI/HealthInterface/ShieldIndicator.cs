using Godot;
using System;

public partial class ShieldIndicator : Control
{
	private AnimationPlayer _player = null!;

	public override void _Ready()
	{
		_player = GetNode<AnimationPlayer>("AnimationPlayer");
		_player.Play("shield_on");
	}


}
