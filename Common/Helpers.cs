using Godot;
using System;
using System.Collections.Generic;

public static class Helpers
{
    public static readonly Dictionary<GameOrientation, OrientationGridData> GridData = new()
    {
        [GameOrientation.Left] = new() { MaxRows = 9, MaxColumns = 3 },
        [GameOrientation.Bottom] = new() { MaxRows = 11, MaxColumns = 2 },
    };
}

public static class Extensions
{
    public static T GetComponent<T>(this List<Component> components, string? name = null) where T : Component
    {
        foreach (var component in components)
        {
            if (component is T Tcomponent && (name == null || component.Name == name))
            {
                return Tcomponent;
            }
        }

        throw new Exception($"Component of type {typeof(T)} with name '{name}' not found.");
    }

    public static void Initialize(this List<Component> components, Node node)
    {
        var children = node.GetChildren();

        foreach (var child in children)
        {
            if (child is Component component)
            {
                components.Add(component);
            }
        }
    }

    public static T? FirstOrDefault<T>(this Godot.Collections.Array<Node> nodes) where T : Node
    {
        foreach (Node child in nodes)
        {
            if (child is T typedChild)
            {
                return typedChild;
            }
        }

        return null;
    }

    public static SceneTreeTimer CreateTimer(this Node caller, double time)
    {
        return caller.GetTree().CreateTimer(time, false);
    }

    public static GameCore GetGameCore(this Node caller)
    {
        var core = caller.GetTree().GetFirstNodeInGroup(GameCore.GroupName) as GameCore;
        if (core is null)
        {
            throw new NullReferenceException(nameof(core));
        }

        return core;
    }

    public static PlayerController GetPlayer(this Node caller)
    {
        var player = caller.GetTree().GetFirstNodeInGroup(PlayerController.GroupName) as PlayerController;
        if (player is null)
        {
            throw new NullReferenceException(nameof(player));
        }

        return player;
    }

    public static GameOrientation GetOpposite(this GameOrientation orientation)
    {
        return orientation switch
        {
            GameOrientation.Top => GameOrientation.Bottom,
            GameOrientation.Bottom => GameOrientation.Top,

            GameOrientation.Left => GameOrientation.Right,
            GameOrientation.Right => GameOrientation.Left,

            GameOrientation.TopLeft => GameOrientation.BottomRight,
            GameOrientation.BottomRight => GameOrientation.TopLeft,

            GameOrientation.TopRight => GameOrientation.BottomLeft,
            GameOrientation.BottomLeft => GameOrientation.TopRight,

            _ => orientation
        };
    }

    public static GameOrientation GetPerpendicular(this GameOrientation orientation)
    {
        return orientation switch
        {
            GameOrientation.Top => GameOrientation.Right,
            GameOrientation.Bottom => GameOrientation.Right,
            GameOrientation.Left => GameOrientation.Top,
            GameOrientation.Right => GameOrientation.Top,
            GameOrientation.TopLeft => GameOrientation.BottomRight,
            GameOrientation.TopRight => GameOrientation.BottomLeft,
            GameOrientation.BottomLeft => GameOrientation.TopRight,
            GameOrientation.BottomRight => GameOrientation.TopLeft,
            _ => orientation
        };
    }

public static Vector2 GetDirectionVector(this GameOrientation orientation)
    {
        return orientation switch
        {
            GameOrientation.Top => new Vector2(0, -1),
            GameOrientation.Bottom => new Vector2(0, 1),

            GameOrientation.Left => new Vector2(-1, 0),
            GameOrientation.Right => new Vector2(1, 0),

            GameOrientation.TopLeft => new Vector2(-1, -1).Normalized(),
            GameOrientation.TopRight => new Vector2(1, -1).Normalized(),
            GameOrientation.BottomLeft => new Vector2(-1, 1).Normalized(),
            GameOrientation.BottomRight => new Vector2(1, 1).Normalized(),

            _ => new Vector2(-1, 0)
        };
    }
}
