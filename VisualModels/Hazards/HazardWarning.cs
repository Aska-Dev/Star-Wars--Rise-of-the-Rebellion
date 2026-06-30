using Godot;
using System;

public partial class HazardWarning : Control
{
	[Export] public AnimationPlayer AnimationPlayer { get; set; } = null!;

    public override void _Ready()
    {
        GlobalPosition = new Vector2(-1000, -1000);

        if(GameManager.CurrentOrientation == GameOrientation.Top || GameManager.CurrentOrientation == GameOrientation.Bottom)
        {
            RotationDegrees = 90;
            var icon = GetNode<TextureRect>("Icon");

            icon.RotationDegrees = -90;
            icon.Position = new Vector2(540, icon.Position.Y);
        }

        AnimationPlayer.Play("iconBlinking");
    }
}
