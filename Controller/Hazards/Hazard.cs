using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;

[GlobalClass]
public abstract partial class Hazard : CharacterBody2D
{
    [Export] public PackedScene HazardWarning { get; set; } = null!;

    public float Height { get; set; } = 0;

    private const float WarningTime = 1.5f;
    private Control _warning = null!;

    public override void _Ready()
    {
        _warning = HazardWarning.Instantiate<Control>();
        MasterScene.Instance.AddToScene(_warning);
        
        SetWarningSize(_warning);

        Visible = false;

        GetTree().CreateTimer(WarningTime).Timeout += () =>
        {
            if (GodotObject.IsInstanceValid(_warning))
            {
                _warning.QueueFree();
            }
            Launch();
        };
    }

    public override void _Process(double delta)
    {
        if (!Visible && GodotObject.IsInstanceValid(_warning))
        {
            var dir = GameManager.CurrentOrientation.GetDirectionVector();
            if (Mathf.Abs(dir.X) > Mathf.Abs(dir.Y))
            {
                _warning.GlobalPosition = new Vector2(0, GlobalPosition.Y - (_warning.Size.Y / 2f));
            }
            else
            {
                _warning.GlobalPosition = new Vector2(GlobalPosition.X + (_warning.Size.Y /2f), 0);
            }
        }
    }

    public override void _ExitTree()
    {
        if(GodotObject.IsInstanceValid(_warning))
        {
            _warning.QueueFree();
        }
    }

    protected virtual void Launch()
    {
        Visible = true;
    }

    protected abstract void SetWarningSize(Control warning);
}
