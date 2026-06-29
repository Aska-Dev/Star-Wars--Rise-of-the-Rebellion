using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics;

[GlobalClass]
public abstract partial class ProjectileController : CharacterBody2D, IController
{
	[Export] public int Damage { get; set; }
	[Export] public int Speed { get; set; }

    public List<Component> Components { get; set; } = [];

    private AnimatedSprite2D _hitAnimation = null!;
    private bool _hasDealtDamage = false;

    public override void _Ready()
    {
        _hitAnimation = GetNode<AnimatedSprite2D>("HitAnimation");
        _hitAnimation.AnimationFinished += KillInstance;

        MasterScene.Instance.SceneChanged += KillInstance   ;
    }

    public void Launch(Vector2 direction, uint collisionMask)
    {
        CollisionMask = collisionMask;

        Velocity = direction.Normalized() * Speed;
        GlobalRotation = Velocity.Angle();
    }

    public override void _Process(double delta)
    {
        var rect = GetViewport().GetVisibleRect();
        if (GlobalPosition.X > rect.Size.X || GlobalPosition.Y >= rect.Size.Y)
        {
            KillInstance();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if(_hitAnimation.IsPlaying())
        {
            return;
        }

        var collision = MoveAndCollide(Velocity * (float)delta);

        if(collision != null && !_hasDealtDamage)
        {
            var collider = collision.GetCollider();
            if (collider is ShipController ship)
            {
                ship.TakeDamage(Damage);
                _hasDealtDamage = true;
                Explode();
            }
        }
    }

    protected void Explode()
    {
        _hitAnimation.Play();
        Velocity = Vector2.Zero;
        GetNode<Node2D>("Sprite2D").Visible = false;
    }

    private void KillInstance()
    {
        if (!IsQueuedForDeletion())
        {
            QueueFree();
        }
    }
}
