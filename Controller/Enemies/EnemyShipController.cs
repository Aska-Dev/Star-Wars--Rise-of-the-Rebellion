using Godot;
using System;
using System.Runtime.Intrinsics;

[GlobalClass]
public abstract partial class EnemyShipController : ShipController
{
    [Export] public SoundEffects FlyInSound { get; set; }
    [Export] public int Score { get; set; } = 10;
    [Export(PropertyHint.Range, "1,10,1")] public int GridWidth { get; set; } = 1;
    [Export(PropertyHint.Range, "1,10,1")] public int GridHeight { get; set; } = 1;

    public float FlyInSpeed { get; set; } = 600f;
    private const float flyInActionDelay = 1f;

    private Vector2 _targetPosition;
    private bool _isFlying = false;
    protected bool _isFlyingIn = false;
    private Timer _actionTimer = null!;

    public override void _Ready()
    {
        base._Ready();

        _isFlyingIn = true;

        _actionTimer = GetNode<Timer>("ActionTimer");
        _actionTimer.Timeout += PerformAction;
    }

    public virtual void Init(Vector2 spawnPosition)
    {
        _actionTimer.Stop();
        FlyToPosition(spawnPosition);
    }


    public void FlyToPosition(Vector2 position)
    {
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

        this.GetGameCore().EndlessModeManager.IncreaseScore(Score);
        base.Destroy();
    }

    private void HandleFlying(double delta)
    {
        float distanceToTarget = GlobalPosition.DistanceTo(_targetPosition);
        float stepDistance = FlyInSpeed * (float)delta;

        if (distanceToTarget <= stepDistance)
        {
            GlobalPosition = _targetPosition;
            Velocity = Vector2.Zero;

            _isFlying = false;

            if(_isFlyingIn)
            {
                _isFlyingIn = false;
                this.CreateTimer(flyInActionDelay).Timeout += () =>
                {
                    PerformAction();
                    _actionTimer.Start();
                };
            }
        }
        else
        {
            Vector2 direction = GlobalPosition.DirectionTo(_targetPosition);
            Velocity = direction * FlyInSpeed;
            MoveAndSlide();
        }
    }
}
