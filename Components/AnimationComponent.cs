using Godot;
using Godot.NativeInterop;
using System;

[GlobalClass]
public partial class AnimationComponent : Component
{
    private ShipModel _shipModel = null!;

    public void PlayAnimation(StringName animationName)
    {
        var animationPlayer = _shipModel.AnimationPlayer;

        if (animationPlayer == null)
        {
            return;
        }

        if (animationPlayer.HasAnimation(animationName))
        {
            animationPlayer.Play(animationName);
        }
        else
        {
            Log.Error(nameof(AnimationComponent), nameof(PlayAnimation), $"{animationName} not found in AnimationPlayer of {_shipModel.AnimationPlayer}");
        }
    }

    protected override void Initialize(ShipData shipData, ShipModel shipModel)
    {
        _shipModel = shipModel;
    }
}
