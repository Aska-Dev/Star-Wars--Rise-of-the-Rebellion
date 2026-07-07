using Godot;
using System;

[GlobalClass]
public partial class RollComponent : Component
{
    [Signal] public delegate void OnRollFinishedEventHandler();

    private Timer _cooldownTimer = null!;
    private ShipModel _shipModel = null!;
    private bool _isCurrentlyRolling = false;

    public override void _Ready()
    {
        base._Ready();
        _cooldownTimer = GetNode<Timer>("RollCooldownTimer");
    }

    public bool TryRoll(Vector2 movementInput)
    {
        if (_cooldownTimer.TimeLeft > 0 || _isCurrentlyRolling) return false;

        _isCurrentlyRolling = true;

        if (movementInput == Vector2.Zero)
        {
            movementInput = new Vector2(-1, 0);
        }

        Vector2 driftDirection = movementInput.Normalized();
        float halfDuration = 0.2f;
        float totalDuration = halfDuration * 2;

        // 1. Die richtige Rotations-Achse anhand der Orientierung ermitteln
        string scaleProperty = GameManager.CurrentOrientation.GetOpposite() is GameOrientation.Top or GameOrientation.Bottom
            ? "scale:x"
            : "scale:y";

        // 2. Den aktuellen Scale-Wert auslesen, damit wir nichts zerschießen 
        // (z.B. falls der PlayerController das Schiff vorher per Scale invertiert hat)
        float startScale = scaleProperty == "scale:x" ? _shipModel.Scale.X : _shipModel.Scale.Y;
        float targetScale = startScale * -1.0f;

        Tween rollTween = CreateTween();
        rollTween.SetParallel(true);

        rollTween.TweenMethod(Callable.From<float>(speed =>
        {
            var shipBody = GetParent<CharacterBody2D>();
            shipBody.GlobalPosition += driftDirection * speed * (float)GetProcessDeltaTime();
        }), 400f, 0f, totalDuration)
        .SetTrans(Tween.TransitionType.Quad)
        .SetEase(Tween.EaseType.Out);

        // 3. Dynamische Achse tweenen
        rollTween.TweenProperty(_shipModel, scaleProperty, targetScale, halfDuration)
                 .SetTrans(Tween.TransitionType.Sine)
                 .SetEase(Tween.EaseType.InOut);

        rollTween.TweenProperty(_shipModel, scaleProperty, startScale, halfDuration)
                 .SetTrans(Tween.TransitionType.Sine)
                 .SetEase(Tween.EaseType.InOut)
                 .SetDelay(halfDuration);

        float ghostInterval = 0.05f;
        for (float time = 0f; time <= totalDuration; time += ghostInterval)
        {
            rollTween.TweenCallback(Callable.From(SpawnGhostTrail)).SetDelay(time);
        }

        rollTween.Finished += () =>
        {
            _isCurrentlyRolling = false;
            EmitSignal(SignalName.OnRollFinished);
            _cooldownTimer.Start();
        };

        return true;
    }

    private void SpawnGhostTrail()
    {
        var player = this.GetPlayer();

        // Greift dynamisch auf das "Sprite"-Node der aktuell aktiven Orientierung zu!
        var currentSprite = _shipModel.ActiveOrientation?.GetNodeOrNull<Sprite2D>("Sprite");
        if (currentSprite == null) return;

        Sprite2D ghost = new Sprite2D();
        ghost.Texture = currentSprite.Texture;
        ghost.GlobalPosition = currentSprite.GlobalPosition;
        ghost.GlobalRotation = currentSprite.GlobalRotation;

        // Da wir das _shipModel skalieren, übernimmt das Ghosting automatisch die gestauchte Scale
        ghost.GlobalScale = currentSprite.GlobalScale;
        ghost.Modulate = new Color(1, 1, 1, 0.4f);

        player.GetParent().AddChild(ghost);

        Tween fadeTween = CreateTween();
        fadeTween.TweenProperty(ghost, "modulate:a", 0.0f, 0.25f);
        fadeTween.Finished += () => ghost.QueueFree();
    }

    protected override void Initialize(ShipData shipData, ShipModel shipModel)
    {
        _shipModel = shipModel;
    }
}