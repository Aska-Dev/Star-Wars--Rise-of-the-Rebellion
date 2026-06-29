using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

[GlobalClass]
public partial class HazardController : CharacterBody2D, IController
{
    [Export] public int Damage { get; set; }
    [Export] public int Speed { get; set; }

    [ExportCategory("Dependencies")]
    [Export] public Texture2D HazardTexture { get; set; } = null!;
    [Export] public PackedScene HazardWarningScene { get; set; } = null!;


    public List<Component> Components { get; set; } = [];

    private const float WarningTime = 4f;

    private Control _hazardWarning = null!;
    private bool _isFlying = false;

    public override void _Ready()
    {
        _hazardWarning = HazardWarningScene.Instantiate<Control>();
        AddChild(_hazardWarning);
        _hazardWarning.Size = new Vector2(_hazardWarning.Size.X, HazardTexture.GetSize().Y);
        _hazardWarning.GlobalPosition = new Vector2(0, _hazardWarning.GlobalPosition.Y);
        _hazardWarning.GetNode<AnimationPlayer>("AnimationPlayer").Play("iconBlinking");

        GetNode<TextureRect>("TextureRect").Texture = HazardTexture;

        GetTree().CreateTimer(WarningTime).Timeout += SpawnHazard;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isFlying) 
        {
            return;
        }

        var rect = GetViewport().GetVisibleRect();
        if (GlobalPosition.X > rect.Size.X || GlobalPosition.Y >= rect.Size.Y)
        {
            QueueFree();
        }

        Velocity = GameManager.CurrentOrientation.GetDirectionVector() * Speed;
        MoveAndSlide();
    }

    private void SpawnHazard()
    {
        _hazardWarning.QueueFree();
        _isFlying = true;
    }
}
