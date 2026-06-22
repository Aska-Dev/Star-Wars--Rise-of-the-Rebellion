using Godot;
using System;

public partial class ShipExplosion : Node2D
{
	public override void _Ready()
	{
		var explosionAnimation = GetNode<AnimatedSprite2D>("Explosion");

        explosionAnimation.Play();
        explosionAnimation.AnimationFinished += () =>
        {
            QueueFree();
        };
    }
}
