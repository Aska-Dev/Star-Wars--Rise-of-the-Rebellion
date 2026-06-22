using Godot;
using System;
using System.Collections.Generic;
using static Godot.TextServer;

[GlobalClass]
public partial class ShipModel : Model
{
    [Signal] public delegate void OrientationChangedEventHandler(GameOrientation orientation);

    public AnimationPlayer AnimationPlayer { get; private set; } = null!;
    public ModelOrientation ActiveOrientation { get; private set; } = null!;

    private const int AnimationStepSize = 1;
    private const float AnimationStepTime = 0.7f;

    private AnimationTree _animationTree = null!;

    public override void _Ready()
    {
        _animationTree = GetNode<AnimationTree>("AnimationTree");

        SetOrientation(GameManager.CurrentOrientation);
    }

    private readonly Dictionary<GameOrientation, Vector2> _blendParameters = new()
    {
        [GameOrientation.Top] = new Vector2(0, 1),
        [GameOrientation.TopRight] = new Vector2(1, 1),
        [GameOrientation.TopLeft] = new Vector2(-1, 1),
        [GameOrientation.Left] = new Vector2(-1, 0),
        [GameOrientation.Right] = new Vector2(1, 0),
        [GameOrientation.Bottom] = new Vector2(0, -1),
        [GameOrientation.BottomLeft] = new Vector2(-1, -1),
        [GameOrientation.BottomRight] = new Vector2(1, -1),
    };

    private readonly Dictionary<GameOrientation, string> _orientationNodes = new()
    {
        [GameOrientation.Top] = "Upwards",
        [GameOrientation.TopRight] = "UpwardsAngled",
        [GameOrientation.TopLeft] = "UpwardsAngled",
        [GameOrientation.Left] = "Sideways",
        [GameOrientation.Right] = "Sideways",
        [GameOrientation.Bottom] = "Downwards",
        [GameOrientation.BottomLeft] = "DownwardsAngeled",
        [GameOrientation.BottomRight] = "DownwardsAngeled",
    };

    public async void ChangeOrientation(GameOrientation current, GameOrientation target)
    {
        var currentPos = _blendParameters[current];
        var targetPos = _blendParameters[target];

        while (currentPos != targetPos)
        {
            currentPos = GetNextStep(currentPos, targetPos);

            _animationTree.Set("parameters/blend_position", currentPos);

            await ToSignal(GetTree().CreateTimer(AnimationStepTime), SceneTreeTimer.SignalName.Timeout);
        }

        ActivateOrientationNode(target);
        EmitSignal(SignalName.OrientationChanged, Variant.From(target));
    }

    public void SetOrientation(GameOrientation newOrientation)
    {
        ActivateOrientationNode(newOrientation);
        _animationTree.Set("parameters/blend_position", _blendParameters[newOrientation]);
    }

    private static Vector2 GetNextStep(Vector2 current, Vector2 target)
    {
        // SICHERHEITS-REGEL: Wenn wir gerade um das Zentrum herum ausweichen, dürfen wir
        // nicht frühzeitig auf die 0-Achse zurückkehren, da wir sonst im Zentrum crashen.
        bool blockYToZero = (target.Y == 0 && current.X != target.X);
        bool blockXToZero = (target.X == 0 && current.Y != target.Y);

        if (current.Y != target.Y)
        {
            float stepY = (target.Y > current.Y) ? AnimationStepSize : -AnimationStepSize;
            float nextY = current.Y + stepY;

            if (!(current.X == 0 && nextY == 0) && !(nextY == 0 && blockYToZero))
            {
                return new Vector2(current.X, nextY);
            }
        }

        if (current.X != target.X)
        {
            float stepX = (target.X > current.X) ? AnimationStepSize : -AnimationStepSize;
            float nextX = current.X + stepX;

            if (!(current.Y == 0 && nextX == 0) && !(nextX == 0 && blockXToZero))
            {
                return new Vector2(nextX, current.Y);
            }
        }

        if (current.Y == 0) return new Vector2(current.X, 1);
        if (current.X == 0) return new Vector2(1, current.Y);

        return current;
    }


    protected void ActivateOrientationNode(GameOrientation orientation)
    {
        ActiveOrientation = GetNode<ModelOrientation>(_orientationNodes[orientation]);
        AnimationPlayer = ActiveOrientation.GetNode<AnimationPlayer>("AnimationPlayer");
    }
}
