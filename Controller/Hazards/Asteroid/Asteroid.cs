using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class Asteroid : Hazard
{
    [Export] public Array<Texture2D> AsteroidTexures { get; set; } = [];

    [ExportCategory("Children")]
    [Export] Sprite2D SpriteNode { get; set; } = null!;
    [Export] Area2D Hitbox { get; set; } = null!;
    [Export] AnimationPlayer AnimationPlayer = null!;

    public const float Speed = 1500;

    private double minHeight = 100;
    private double maxHeight = 200;
    private float _flightTime = 0f;

    public override void _Ready()
    {
        SetHeightRandom();
        SetupSprite();
        SetupHitbox();

        AnimationPlayer.Play("spin");

        base._Ready();

        SetPhysicsProcess(false);
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

        Velocity = GameManager.CurrentOrientation.GetDirectionVector() * Speed;
        MoveAndSlide();
    }

    protected override void Launch()
    {
        base.Launch();
        SetPhysicsProcess(true);
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

    private Texture2D GetRandomTexture()
    {
        return AsteroidTexures[GD.RandRange(0, AsteroidTexures.Count - 1)];
    }

    private void SetHeightRandom()
    {
        Height = (float)GD.RandRange(minHeight, maxHeight);
        GD.Print(Height);
    }

    private void SetupSprite()
    {
        var texture = GetRandomTexture();
        var textureSize = texture.GetSize();
        
        var largerSize = textureSize.Y >= textureSize.X ? textureSize.Y : textureSize.X;
        var scale = Height / largerSize;

        SpriteNode.Texture = texture;
        SpriteNode.Scale = new Vector2(scale, scale);
    }

    private void SetupHitbox()
    {
        Hitbox.BodyEntered += OnHitboxBodyEntered;
        Hitbox.GetNode<CollisionShape2D>("CollisionShape2D").Shape = new CircleShape2D()
        {
            Radius = (Height / 2f) * 0.9f
        };
    }
}
