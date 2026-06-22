using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

public partial class HealthIndicator : Control
{
    [Export] public bool StartStatus { get; set; } = true;


	private TextureRect onTexture = null!;
	private TextureRect offTexture = null!;


    public override void _Ready()
    {
        onTexture = GetNode<TextureRect>("On");
        offTexture = GetNode<TextureRect>("Off");

        Switch(StartStatus);
    }

    public void Switch(bool state)
    {
        onTexture.Visible = state;
        offTexture.Visible = !state;
    }
}
