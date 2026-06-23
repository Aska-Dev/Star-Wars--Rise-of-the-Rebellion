using Godot;
using System;

[GlobalClass]
public partial class HealthComponent : Component
{
    [Signal] public delegate void OnDeathEventHandler();
    [Signal] public delegate void OnHealthChangedEventHandler(int value);

    [Export] public bool HasIFramesAfterHit = false;

    public int MaxHealth { get; private set; }

    private int _health;
    public int Health
    { 
        get => _health; 
        private set
        {
            if (_health == value) return;
            _health = value;
            EmitSignal(SignalName.OnHealthChanged, _health);
        }
    }

    public void TakeDamage(int damage)
    {
        if(_onIFrames)
        {
            return;
        }

        Health -= damage;

        if (Health <= 0)
        {
            Health = 0;
            EmitSignal(SignalName.OnDeath);
        }

        if(HasIFramesAfterHit && _iframeTimer is not null)
        {
            _onIFrames = true;
            _iframeTimer.Start();
        }
    }

    public void Heal(int amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }

    private Timer? _iframeTimer;
    private bool _onIFrames = false;

    public override void _Ready()
    {
        base._Ready();

        if(HasIFramesAfterHit)
        {
            _iframeTimer = GetNode<Timer>("IFrameTimer");
            _iframeTimer.Timeout += OnIFramesFinished;
        }
    }

    private void OnIFramesFinished()
    {
        _onIFrames = false;
    }

    protected override void Initialize(ShipData shipData, ShipModel shipModel)
    {
        MaxHealth = shipData.Health;
        Health = MaxHealth;
    }
}