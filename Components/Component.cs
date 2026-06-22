using Godot;
using System;

[GlobalClass]
public abstract partial class Component : Node
{
    public override void _Ready()
    {
        var shipController = GetShipControllerParent();
        if (shipController != null)
        {
            shipController.OnLoadShip += Initialize;
        }
        else
        {
            GD.PrintErr($"Component {Name} is not attached to a ShipController!");
        }
    }

    protected abstract void Initialize(ShipData shipData, ShipModel shipModel);

    private ShipController? GetShipControllerParent()
    {
        var parent = GetParent();
        while (parent != null)
        {
            if (parent is ShipController shipController)
            {
                return shipController;
            }
            parent = parent.GetParent();
        }
        return null;
    }
}
