using Godot;
using System;

public partial class ActionIcon : Control
{
	public TextureProgressBar Icon { get; set; } = null!;
	public AnimationPlayer AnimationPlayer { get; set; } = null!;

	public override void _Ready()
	{
		Icon = GetNode<TextureProgressBar>("Icon");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

    public void StartCooldown(float cooldownDuration)
    {
        Icon.Value = 0;

        Tween tween = CreateTween();
		tween.TweenProperty(Icon, "value", 100.0, cooldownDuration);
		tween.TweenCallback(Callable.From(OnChargeComplete));
    }

    public void OnChargeComplete()
	{
		AnimationPlayer.Play("impulse");
    }
}
