using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

[GlobalClass]
public abstract partial class ShipController : CharacterBody2D, IController
{
    [Signal] public delegate void OnLoadShipEventHandler(ShipData shipData, ShipModel shipModel);
    [Signal] public delegate void OnDestroyEventHandler();

    [Export] public ShipData InitShipData { get; set; } = null!;
    [Export] public PackedScene InitShipModelScene { get; set; } = null!;

    public List<Component> Components { get; set; } = [];

    // Instantiated ShipModelScene
    protected ShipModel shipModel = null!;

    protected bool _isDestroyed = false;

    public override void _Ready()
    {
        // Initialize components
        Components.Initialize(this);

        // Load the initial ship model
        LoadShip(InitShipData, InitShipModelScene);
    }

    public void TakeDamage(int damage)
    {
        Components.GetComponent<HealthComponent>().TakeDamage(damage);
    }

    public virtual void Destroy()
    {
        if(_isDestroyed)
        {
            return;
        }

        EmitSignal(SignalName.OnDestroy);

        _isDestroyed = true;

        shipModel.ActiveOrientation.Explode();
        AudioEngine.Instance.PlaySound(SoundEffects.ShipExplosion, true);
        QueueFree();
    }

    protected virtual void LoadShip(ShipData shipData, PackedScene shipModelScene)
    {
        // Re-instantiate the ship model with the new scene
        if (shipModel != null)
        {
            RemoveChild(shipModel);
            shipModel.QueueFree();
        }

        // Instantiate the new ship model
        shipModel = shipModelScene.Instantiate<ShipModel>();
        AddChild(shipModel);
        UpdateCollisionShape();

        EmitSignal(SignalName.OnLoadShip, shipData, shipModel);
    }

    protected void UpdateCollisionShape()
    {
        var modelCollisionShape = shipModel.ActiveOrientation.GetNode<CollisionPolygon2D>("Collision");
        var collisionShape = GetNode<CollisionPolygon2D>("CollisionPolygon2D");
        collisionShape.Polygon = modelCollisionShape.Polygon;
    }

}