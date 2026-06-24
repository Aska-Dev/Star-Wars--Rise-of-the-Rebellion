using Godot;
using System;
using System.Runtime.Intrinsics;

public abstract partial class EnemyShipController : ShipController
{
    [Export] public SoundEffects FlyInSound { get; set; }
    [Export] public int Score { get; set; } = 10;

    private const float flyInSpeed = 600f;
    private const float flyInActionDelay = 1f;

    private Vector2 _targetPosition;
    private bool _isFlying = false;
    private Timer _actionTimer = null!;

    public override void _Ready()
    {
        base._Ready();

        _actionTimer = GetNode<Timer>("ActionTimer");
        _actionTimer.Timeout += PerformAction;
    }

    public void FlyToPosition(Vector2 position)
    {
        _actionTimer.Stop();

        _targetPosition = position;
        _isFlying = true;

        AudioEngine.Instance.PlaySound(FlyInSound, true);
    }

    public abstract void PerformAction();

    public override void _PhysicsProcess(double delta)
    {
        if(_isFlying)
        {
            HandleFlying(delta);
        }
    }

    public override void Destroy()
    {
        if (_isDestroyed)
        {
            return;
        }

        GameCore.GetFrom(this).EndlessModeManager.IncreaseScore(Score);
        base.Destroy();
    }

    private void HandleFlying(double delta)
    {
        float distanceToTarget = GlobalPosition.DistanceTo(_targetPosition);
        float stepDistance = flyInSpeed * (float)delta;

        if (distanceToTarget <= stepDistance)
        {
            GlobalPosition = _targetPosition;
            Velocity = Vector2.Zero;

            _isFlying = false;

            GetTree().CreateTimer(flyInActionDelay).Timeout += () =>
            {
                PerformAction();
                _actionTimer.Start();
            };
        }
        else
        {
            Vector2 direction = GlobalPosition.DirectionTo(_targetPosition);
            Velocity = direction * flyInSpeed;
            MoveAndSlide();
        }
    }
}
