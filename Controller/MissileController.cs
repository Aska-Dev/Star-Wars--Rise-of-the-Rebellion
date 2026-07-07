using Godot;
using System;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.InteropServices.Marshalling;

[GlobalClass]
public partial class MissileController : ProjectileController
{
    [Export] public float TurnSpeed { get; set; } = 3.0f;
    [Export] public int FuseTime { get; set; } = 5;

    public override void _Ready()
    {
        base._Ready();

        var fuseTimer = GetNode<Timer>("FuseTimer");
        fuseTimer.Start(FuseTime);
        fuseTimer.Timeout += Explode;

        var sprite = GetNode<AnimatedSprite2D>("Sprite2D");
        sprite.Play();
    }

    public override void _PhysicsProcess(double delta)
    {
        var player = this.GetPlayer();

        if (player != null && IsInstanceValid(player))
        {
            Vector2 desiredDirection = (player.GlobalPosition - GlobalPosition).Normalized();
            Vector2 currentDirection = Velocity.Normalized();
            Vector2 newDirection = currentDirection.Slerp(desiredDirection, TurnSpeed * (float)delta);

            Velocity = newDirection * Speed;

            Rotation = Velocity.Angle();
        }
        else if (Velocity != Vector2.Zero)
        {
            Velocity = Velocity.Normalized() * Speed;
        }

        base._PhysicsProcess(delta);
    }
}
