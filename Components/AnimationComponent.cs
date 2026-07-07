using Godot;
using System;

[GlobalClass]
public partial class AnimationComponent : Component
{
    private ShipModel _shipModel = null!;

    public void PlayAnimationSecure(StringName animationName)
    {
        var animationPlayer = _shipModel.AnimationPlayer;

        if (animationPlayer == null)
        {
            return;
        }

        if (animationPlayer.IsPlaying())
        {
            PlayAnimation(animationName);
        }
    }

    public void PlayAnimation(StringName animationName, Action? onAnimationComplete = null)
    {
        var animationPlayer = _shipModel.AnimationPlayer;
        if (animationPlayer == null)
        {
            return;
        }

        if (animationPlayer.HasAnimation(animationName))
        {
            void HandleAnimationFinished(StringName finishedAnimation)
            {
                if (finishedAnimation != animationName)
                {
                    return;
                }

                animationPlayer.AnimationFinished -= HandleAnimationFinished;
                onAnimationComplete?.Invoke();
            }

            if (onAnimationComplete != null)
            {
                animationPlayer.AnimationFinished += HandleAnimationFinished;
            }

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
