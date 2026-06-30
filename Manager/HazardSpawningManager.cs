using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.TextServer;

[GlobalClass]
public partial class HazardSpawningManager : Node
{
    [Signal]
    public delegate void OnAllHazardsClearedEventHandler();

    private readonly List<Hazard> _activeHazards = [];
    private SpawnGridModule _spawnGrid;
    private const int MaxAttempts = 15;

    public override void _Ready()
    {
        _spawnGrid = new SpawnGridModule(GetViewport().GetVisibleRect().Size);
    }

    public void SpawnRandomHazard(PackedScene hazardScene)
    {
        // Instantiate hazard
        var hazard = hazardScene.Instantiate<Hazard>();
        MasterScene.Instance.AddToScene(hazard);

        var orientation = GameManager.CurrentOrientation;
        var direction = orientation.GetDirectionVector();
        float halfSize = hazard.Height / 2f;

        Vector2? spawnPos = Mathf.Abs(direction.X) > Mathf.Abs(direction.Y)
            ? GetHorizontalSpawnPos(hazard, orientation, halfSize, direction.X)
            : GetVerticalSpawnPos(hazard, orientation, halfSize, direction.Y);

        if (spawnPos == null)
        {
            hazard.QueueFree();
            return;
        }

        hazard.GlobalPosition = spawnPos.Value;
        _activeHazards.Add(hazard);

        hazard.TreeExited += () =>
        {
            _activeHazards.Remove(hazard);
            if (_activeHazards.Count == 0) EmitSignal(SignalName.OnAllHazardsCleared);
        };
    }

    private bool HasEnoughDodgeSpace(float newHazardHeight, GameOrientation orient, Vector2 direction)
    {
        float totalSpace = Mathf.Abs(direction.X) > Mathf.Abs(direction.Y)
            ? _spawnGrid.GetWorldPosition(0, Helpers.GridData[orient].MaxRows - 1, orient).Y - _spawnGrid.GetWorldPosition(0, 0, orient).Y
            : _spawnGrid.GetWorldPosition(0, Helpers.GridData[orient].MaxRows - 1, orient).X - _spawnGrid.GetWorldPosition(0, 0, orient).X;

        float occupiedSpace = _activeHazards.Sum(h => h.Height);
        float requiredDodgeSpace = GetRequiredDodgeSpace(orient);

        return (totalSpace - occupiedSpace - newHazardHeight) >= requiredDodgeSpace;
    }

    private float GetRequiredDodgeSpace(GameOrientation orient)
    {
        var playerSize = 70f;
        var player = PlayerController.GetFrom(this);

        if (player != null)
        {
            var sprite = player.GetNodeOrNull<Sprite2D>("Sprite");
            if (sprite != null && sprite.Texture != null)
            {
                playerSize = Mathf.Abs(orient.GetDirectionVector().X) > Mathf.Abs(orient.GetDirectionVector().Y)
                    ? sprite.Texture.GetSize().Y * sprite.Scale.Y
                    : sprite.Texture.GetSize().X * sprite.Scale.X;
            }
        }

        return playerSize * 1.5f;
    }

    private Vector2? GetHorizontalSpawnPos(Hazard hazard, GameOrientation orient, float halfSize, float dirX)
    {
        float minY = _spawnGrid.GetWorldPosition(0, 0, orient).Y;
        float maxY = _spawnGrid.GetWorldPosition(0, Helpers.GridData[orient].MaxRows - 1, orient).Y;
        float y = FindRandomCoord(minY + halfSize, maxY - halfSize, halfSize, true);

        if (y < 0) return null;

        float x = dirX < 0 ? GetViewport().GetVisibleRect().Size.X + 500f : -500f;
        return new Vector2(x, y);
    }

    private Vector2? GetVerticalSpawnPos(Hazard hazard, GameOrientation orient, float halfSize, float dirY)
    {
        float minX = _spawnGrid.GetWorldPosition(0, 0, orient).X;
        float maxX = _spawnGrid.GetWorldPosition(0, Helpers.GridData[orient].MaxRows - 1, orient).X;

        float x = FindRandomCoord(minX + halfSize, maxX - halfSize, halfSize, false);

        if (x < 0) return null;

        float y = dirY < 0 ? GetViewport().GetVisibleRect().Size.Y + 500f : -500f;
        return new Vector2(x, y);
    }

    private float FindRandomCoord(float min, float max, float halfSize, bool checkY)
    {
        for (int i = 0; i < MaxAttempts; i++)
        {
            float coord = (float)GD.RandRange(min, max);
            bool overlapping = _activeHazards.Any(h =>
            {
                float pos = checkY ? h.GlobalPosition.Y : h.GlobalPosition.X;
                return Mathf.Abs(pos - coord) < (halfSize + (h.Height / 2f) + 20f);
            });

            if (!overlapping) return coord;
        }
        return -1f;
    }
}