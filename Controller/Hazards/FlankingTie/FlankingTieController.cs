using Godot;
using System;

[GlobalClass]
public partial class FlankingTieController : Hazard
{
    [ExportCategory("Children")]
    [Export] Area2D Hitbox { get; set; } = null!;
    [Export] Sprite2D HorizontalSpriteNode { get; set; } = null!;
    [Export] Sprite2D VerticalSpriteNode { get; set; } = null!;

    public const float Speed = 1500;

    private float _flightTime = 0f;
    private Vector2 _direction = GameManager.CurrentOrientation.GetPerpendicular().GetDirectionVector();

    public override void _Ready()
    {
        _direction = TravelDirection ?? GameManager.CurrentOrientation.GetPerpendicular().GetDirectionVector();

        base._Ready();

        Hitbox.BodyEntered += OnHitboxBodyEntered;
        SetPhysicsProcess(false);
        SetupSprite();
    }

    public override void _PhysicsProcess(double delta)
    {
        _flightTime += (float)delta;
        var rect = GetViewport().GetVisibleRect();

        if (_flightTime > 1.0f)
        {
            if (GlobalPosition.X > rect.Size.X + 100 || GlobalPosition.X < -100 ||
                GlobalPosition.Y >= rect.Size.Y + 100 || GlobalPosition.Y < -100)
            {
                QueueFree();
            }
        }

        Velocity = _direction * Speed;
        MoveAndSlide();
    }

    protected override void Launch()
    {
        base.Launch();
        SetPhysicsProcess(true);

        AudioEngine.Instance.PlaySound(SoundEffects.TieShoot);
        this.CreateTimer(0.5f).Timeout += () =>
        {
            AudioEngine.Instance.PlaySound(SoundEffects.TieFlyIn);
        };
    }

    protected override void SetWarningSize(Control warning)
    {
        var newSize = new Vector2(warning.Size.X, Height);
        warning.SetDeferred(Control.PropertyName.Size, newSize);
        warning.SetDeferred(Control.PropertyName.CustomMinimumSize, newSize);
    }

    private void OnHitboxBodyEntered(Node2D body)
    {
        var health = body.GetNodeOrNull<HealthComponent>("HealthComponent");
        if (health != null)
        {
            health.TakeDamage(9999);
        }
    }

    private void SetupSprite()
    {
        if (Mathf.Abs(_direction.X) > Mathf.Abs(_direction.Y))
        {
            HorizontalSpriteNode.Visible = true;
            VerticalSpriteNode.Visible = false;

            GetNode<Node2D>("Sprites").RotationDegrees = 0;
        }
        else
        {
            HorizontalSpriteNode.Visible = false;
            VerticalSpriteNode.Visible = true;

            GetNode<Node2D>("Sprites").RotationDegrees = 270;
        }
    }   
}

