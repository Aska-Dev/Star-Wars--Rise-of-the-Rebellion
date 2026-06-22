using Godot;
using System;

[GlobalClass]
public partial class PlayerMovementComponent : Component
{
    private int _xSpeed = 0;
    private int _ySpeed = 0;

    public Vector2 GetMovementVector(Vector2 velocity)
    {
        var originalVelocity = velocity;

        Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * _xSpeed;
            velocity.Y = direction.Y * _ySpeed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(originalVelocity.X, 0, _xSpeed);
            velocity.Y = Mathf.MoveToward(originalVelocity.Y, 0, _ySpeed);
        }

        return velocity;
    }

    protected override void Initialize(ShipData shipData, ShipModel shipModel)
    {
        _xSpeed = shipData.XSpeed;
        _ySpeed = shipData.YSpeed;
    }
}
