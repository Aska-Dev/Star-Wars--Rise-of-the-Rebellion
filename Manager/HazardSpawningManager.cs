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
    private readonly Stack<(PackedScene HazardScene, GameOrientation FromOrientation, HazardSpawnToken Token)> _pendingAtPlayerSpawns = [];
    private bool _atPlayerCooldownActive;
    private SpawnGridModule _spawnGrid;
    private const int MaxAttempts = 15;
    private const float AtPlayerCooldown = 1f;

    public override void _Ready()
    {
        _spawnGrid = new SpawnGridModule(GetViewport().GetVisibleRect().Size);
    }

    public HazardSpawnToken SpawnHazardRandom(PackedScene hazardScene)
    {
        var token = new HazardSpawnToken();

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
            return token;
        }

        hazard.GlobalPosition = spawnPos.Value;
        _activeHazards.Add(hazard);

        hazard.TreeExited += () =>
        {
            _activeHazards.Remove(hazard);
            if (_activeHazards.Count == 0) EmitSignal(SignalName.OnAllHazardsCleared);
            token.TriggerCompleted();
        };

        return token;
    }

    public HazardSpawnToken SpawnHazardAtPlayer(PackedScene hazardScene, GameOrientation fromOrientation)
    {
        var token = new HazardSpawnToken();

        if (_atPlayerCooldownActive)
        {
            _pendingAtPlayerSpawns.Push((hazardScene, fromOrientation, token));
            return token;
        }

        ExecuteAtPlayerSpawn(hazardScene, fromOrientation, token);
        return token;
    }

    private void ExecuteAtPlayerSpawn(PackedScene hazardScene, GameOrientation fromOrientation, HazardSpawnToken token)
    {
        var hazard = hazardScene.Instantiate<Hazard>();

        var player = this.GetPlayer();
        if (player == null)
        {
            hazard.QueueFree();
            token.TriggerCompleted();
            StartAtPlayerCooldown();
            return;
        }

        var playerPos = player.GlobalPosition;
        var sourceDirection = fromOrientation.GetDirectionVector();
        var travelDirection = -sourceDirection;
        Vector2 spawnPos;

        if (Mathf.Abs(sourceDirection.X) > Mathf.Abs(sourceDirection.Y))
        {
            spawnPos = new Vector2(
                sourceDirection.X < 0 ? -500f : GetViewport().GetVisibleRect().Size.X + 500f,
                playerPos.Y
            );
        }
        else
        {
            spawnPos = new Vector2(
                playerPos.X,
                sourceDirection.Y < 0 ? -500f : GetViewport().GetVisibleRect().Size.Y + 500f
            );
        }

        hazard.TravelDirection = travelDirection;
        hazard.GlobalPosition = spawnPos;
        MasterScene.Instance.AddToScene(hazard);
        _activeHazards.Add(hazard);

        hazard.TreeExited += () =>
        {
            _activeHazards.Remove(hazard);
            if (_activeHazards.Count == 0) EmitSignal(SignalName.OnAllHazardsCleared);
            token.TriggerCompleted();
        };

        StartAtPlayerCooldown();
    }

    private void StartAtPlayerCooldown()
    {
        _atPlayerCooldownActive = true;
        var timer = GetTree().CreateTimer(AtPlayerCooldown);
        timer.Timeout += OnAtPlayerCooldownExpired;
    }

    private void OnAtPlayerCooldownExpired()
    {
        _atPlayerCooldownActive = false;

        if (_pendingAtPlayerSpawns.TryPop(out var pending))
            ExecuteAtPlayerSpawn(pending.HazardScene, pending.FromOrientation, pending.Token);
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
        var player = this.GetPlayer();

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

        return playerSize * 2f;
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
        float minX = 0f; //_spawnGrid.GetWorldPosition(0, 0, orient).X;
        float maxX = GetViewport().GetVisibleRect().Size.X;

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