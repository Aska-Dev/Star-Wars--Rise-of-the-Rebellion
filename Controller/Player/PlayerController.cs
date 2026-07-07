using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

[GlobalClass]
public partial class PlayerController : ShipController
{
    public AnimationComponent AnimationComponent => Components.GetComponent<AnimationComponent>();
    public const string GroupName = nameof(PlayerController);

    private bool _actionsLocked = false;

    public override void _Ready()
    {
        GD.Print(CollisionLayer);
        GameManager.CurrentOrientation = GameOrientation.Left;

        base._Ready();
        AddToGroup(GroupName);

        Components.GetComponent<HealthComponent>().OnHealthChanged += EventHub.Instance.UiEventBus.InvokePlayerHealthChanged;
        Components.GetComponent<RollComponent>().OnRollFinished += () => { _actionsLocked = false; CollisionLayer = 33; };
        shipModel.OrientationChanged += OnShipModelOrienationChanged;

        shipModel.SetOrientation(GameManager.CurrentOrientation.GetOpposite());
        SyncCollisionShapeDeferred();
    }

    public override void _Process(double delta)
    {
        // --- SHOOTING ---
        if (Input.IsActionPressed("shoot") && !_actionsLocked)
        {
            Components.GetComponent<WeaponsComponent>().Shoot();
        }

        // --- ACTIONS ---
        if(Input.IsActionPressed("action1"))
        {
            Components.GetComponent<ActionComponent>().TryExecuteAction(0, this);
        }
    }

    // --- MOVEMENT ---
    public override void _PhysicsProcess(double delta)
    {
        Velocity = Components.GetComponent<PlayerMovementComponent>().GetMovementVector(Velocity);

        // --- ROLLING ---
        if (Input.IsActionPressed("roll") && !_actionsLocked)
        {
            if(Components.GetComponent<RollComponent>().TryRoll(Velocity))
            {
                CollisionLayer = 32;
                _actionsLocked = true;
            }
        }

        if (Velocity.X != 0 || Velocity.Y != 0 )
        {
            AnimationComponent.PlayAnimationSecure("moving");
        }
        else
        {
            AnimationComponent.PlayAnimationSecure("idle");
        }

        MoveAndSlide();
    }

    public override void _ExitTree()
    {
        if (IsInGroup(GroupName))
        {
            RemoveFromGroup(GroupName);
        }
    }

    public void ChangeOrientation(GameOrientation newOrientation)
    {
        var flippedOrientation = newOrientation.GetOpposite();

        _actionsLocked = true;
        shipModel.ChangeOrientation(GameManager.CurrentOrientation.GetOpposite(), flippedOrientation);
    }

    private void OnShipModelOrienationChanged(GameOrientation newOrientation)
    {
        _actionsLocked = false;
        GameManager.CurrentOrientation = newOrientation.GetOpposite();
        EventHub.Instance.PlayerEventBus.InvokePlayerOrientationChanged();

        SyncCollisionShapeDeferred();
    }

    private async void SyncCollisionShapeDeferred()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        UpdateCollisionShape();
    }
}
