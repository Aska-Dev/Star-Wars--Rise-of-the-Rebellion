using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

[GlobalClass]
public partial class PlayerController : ShipController
{
    public AnimationComponent AnimationComponent => Components.GetComponent<AnimationComponent>();

    private bool _actionsLocked = false;
    private const string GroupName = nameof(PlayerController);

    public override void _Ready()
    {
        GameManager.CurrentOrientation = GameOrientation.Left;

        base._Ready();
        AddToGroup(GroupName);

        Components.GetComponent<HealthComponent>().OnHealthChanged += EventHub.Instance.UiEventBus.InvokePlayerHealthChanged;
        Components.GetComponent<RollComponent>().OnRollFinished += () => { _actionsLocked = false; CollisionLayer = 1; };
        shipModel.OrientationChanged += OnShipModelOrienationChanged;

        shipModel.SetOrientation(GameManager.CurrentOrientation.GetOpposite());
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
                CollisionLayer = 0;
                _actionsLocked = true;
            }
        }

        if (Velocity.X != 0 || Velocity.Y != 0 )
        {
            AnimationComponent.PlayAnimation("moving");
        }
        else
        {
            AnimationComponent.PlayAnimation("idle");
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

    public static PlayerController? GetFrom(Node caller)
    {
        return caller.GetTree().GetFirstNodeInGroup(GroupName) as PlayerController;
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

        UpdateCollisionShape();
    }
}
