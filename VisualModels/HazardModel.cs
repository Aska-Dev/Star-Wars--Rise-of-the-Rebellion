using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class HazardModel : Model
{
    [Export] public Array<Texture2D> HazardTextures { get; set; } = [];

    public override void _Ready()
    {
        var randomTexture = HazardTextures[GD.RandRange(0, HazardTextures.Count - 1)];
        GetNode<Sprite2D>("Sprite").Texture = randomTexture;

        RotationDegrees = GD.RandRange(0, 360);
    }
}
