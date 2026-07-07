using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;

[GlobalClass]
public partial class EnemySpawningManager : Node
{
    public SpawnGridModule SpawnGridModule { get; set; }

    // --- ACTIONS ---
    public event Action OnAllEnemiesDefeated = null!;
    public event Action OnEnemyDefeated = null!;

    // --- PROPERTIES ---
    public int EnemyAmount => _activeGrid.Count;

    // --- FIELDS ---
    private List<EnemyActiveGridSlot> _activeGrid = [];
    private Node? _enemyContainerNode;

    public override void _Ready()
    {
        SpawnGridModule = new(GetViewport().GetVisibleRect().Size);
        _enemyContainerNode = GetParent().GetParent().GetNode("EnemyContainer");
    }

    public bool IsSlotFree(int column, int row)
    {
        return !_activeGrid.Any(s => s.Column == column && s.Row == row);
    }

    public bool AreSlotsFree(int column, int row, int width, int height, EnemyShipController? ignoredEnemy = null)
    {
        var currentOrientation = GameManager.CurrentOrientation;

        int maxCols = Helpers.GridData[currentOrientation].MaxColumns;
        int maxRows = Helpers.GridData[currentOrientation].MaxRows;

        if (column < 0 || row < 0 || width <= 0 || height <= 0)
        {
            return false;
        }

        if (column + width > maxCols || row + height > maxRows)
        {
            return false;
        }

        for (int c = column; c < column + width; c++)
        {
            for (int r = row; r < row + height; r++)
            {
                if (_activeGrid.Any(s => s.Column == c && s.Row == r && s.Enemy != ignoredEnemy))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public List<EnemyGridSlot> GetFreeSlots()
    {
        var freeSlots = new List<EnemyGridSlot>();
        var currentOrientation = GameManager.CurrentOrientation;

        int maxCols = Helpers.GridData[currentOrientation].MaxColumns;
        int maxRows = Helpers.GridData[currentOrientation].MaxRows;

        for (int c = 0; c < maxCols; c++)
        {
            for (int r = 0; r < maxRows; r++)
            {
                if (IsSlotFree(c, r))
                {
                    freeSlots.Add(new(c, r));
                }
            }
        }

        return freeSlots;
    }

    private Vector2 GetCenteredWorldPosition(int column, int row, int width, int height, GameOrientation orientation)
    {
        var topLeft = SpawnGridModule.GetWorldPosition(column, row, orientation);
        var bottomRight = SpawnGridModule.GetWorldPosition(column + width - 1, row + height - 1, orientation);

        return (topLeft + bottomRight) * 0.5f;
    }

    public async void SpawnFormation(List<EnemyFormationSlot> formationSlots)
    {
        foreach(var slot in formationSlots)
        {
            if (slot.SpawnDelay > 0f)
            {
                await GetTree().Root.ToSignal(this.CreateTimer(slot.SpawnDelay, false), SceneTreeTimer.SignalName.Timeout);
            }

            SpawnEnemy(slot.Enemy, slot.Column, slot.Row);
        }
    }

    public void SpawnEnemy(EnemyFormationSlot slot)
    {
        SpawnEnemy(slot.Enemy, slot.Column, slot.Row);
    }

    public void SpawnEnemy(PackedScene enemyScene, int column, int row)
    {
        if (_enemyContainerNode is null)
        {
            throw new NullReferenceException($"{nameof(_enemyContainerNode)} in {nameof(EnemySpawningManager)}");
        }

        var enemy = enemyScene.Instantiate<EnemyShipController>();

        if (!AreSlotsFree(column, row, enemy.GridWidth, enemy.GridHeight))
        {
            Log.Error(nameof(EnemySpawningManager), nameof(SpawnEnemy), $"No free area for {enemy.GetType().Name} ({enemy.GridWidth}x{enemy.GridHeight}) at {column}/{row}");
            enemy.QueueFree();
            return;
        }

        for (int c = column; c < column + enemy.GridWidth; c++)
        {
            for (int r = row; r < row + enemy.GridHeight; r++)
            {
                _activeGrid.Add(new(c, r, enemy));
            }
        }

        var currentOrientation = GameManager.CurrentOrientation;
        var targetPosition = GetCenteredWorldPosition(column, row, enemy.GridWidth, enemy.GridHeight, currentOrientation);

        var spawnDirection = currentOrientation.GetOpposite().GetDirectionVector();
        var spawnPosition = targetPosition + (spawnDirection * 500f);

        enemy.GlobalPosition = spawnPosition;
        _enemyContainerNode.AddChild(enemy);

        enemy.FlyToPosition(targetPosition);
        enemy.OnDestroy += () => OnEnemyDestroyed(enemy);
    }

    public void ShuffleEnemy(EnemyShipController enemy)
    {
        var occupiedSlots = _activeGrid.Where(g => g.Enemy == enemy).ToList();
        if (occupiedSlots.Count <= 0)
        {
            Log.Error(nameof(EnemySpawningManager), nameof(ShuffleEnemy), "Enemy not on grid");
            return;
        }

        int currentTopLeftColumn = occupiedSlots.Min(s => s.Column);
        int currentTopLeftRow = occupiedSlots.Min(s => s.Row);

        foreach (var slot in occupiedSlots)
        {
            _activeGrid.Remove(slot);
        }

        var currentOrientation = GameManager.CurrentOrientation;
        int maxCols = Helpers.GridData[currentOrientation].MaxColumns;
        int maxRows = Helpers.GridData[currentOrientation].MaxRows;

        var possibleSlots = new List<EnemyGridSlot>();

        for (int c = 0; c < maxCols; c++)
        {
            for (int r = 0; r < maxRows; r++)
            {
                if (AreSlotsFree(c, r, enemy.GridWidth, enemy.GridHeight, enemy))
                {
                    possibleSlots.Add(new EnemyGridSlot(c, r));
                }
            }
        }

        if (possibleSlots.Count <= 0)
        {
            foreach (var slot in occupiedSlots)
            {
                _activeGrid.Add(slot);
            }

            return;
        }

        var rowChangedSlots = possibleSlots.Where(s => s.Row != currentTopLeftRow).ToList();
        var targetSlotPool = rowChangedSlots.Count > 0 ? rowChangedSlots : possibleSlots;
        var targetSlot = targetSlotPool[GD.RandRange(0, targetSlotPool.Count - 1)];

        for (int c = targetSlot.Column; c < targetSlot.Column + enemy.GridWidth; c++)
        {
            for (int r = targetSlot.Row; r < targetSlot.Row + enemy.GridHeight; r++)
            {
                _activeGrid.Add(new EnemyActiveGridSlot(c, r, enemy));
            }
        }

        var targetPos = GetCenteredWorldPosition(targetSlot.Column, targetSlot.Row, enemy.GridWidth, enemy.GridHeight, currentOrientation);
        enemy.FlyToPosition(targetPos);
    }

    public void ShuffleGrid()
    {
        var enemies = _activeGrid.Select(g => g.Enemy).Distinct().ToList();
        if (enemies.Count <= 0)
        {
            return;
        }

        var currentOrientation = GameManager.CurrentOrientation;
        int maxCols = Helpers.GridData[currentOrientation].MaxColumns;
        int maxRows = Helpers.GridData[currentOrientation].MaxRows;

        var previousGrid = _activeGrid.ToList();
        var newTopLeftSlots = new Dictionary<EnemyShipController, EnemyGridSlot>();

        _activeGrid.Clear();

        foreach (var enemy in enemies.OrderBy(_ => GD.Randf()))
        {
            var possibleSlots = new List<EnemyGridSlot>();

            for (int c = 0; c < maxCols; c++)
            {
                for (int r = 0; r < maxRows; r++)
                {
                    if (AreSlotsFree(c, r, enemy.GridWidth, enemy.GridHeight, enemy))
                    {
                        possibleSlots.Add(new EnemyGridSlot(c, r));
                    }
                }
            }

            if (possibleSlots.Count <= 0)
            {
                _activeGrid.Clear();
                _activeGrid.AddRange(previousGrid);
                return;
            }

            var targetSlot = possibleSlots[GD.RandRange(0, possibleSlots.Count - 1)];
            newTopLeftSlots[enemy] = targetSlot;

            for (int c = targetSlot.Column; c < targetSlot.Column + enemy.GridWidth; c++)
            {
                for (int r = targetSlot.Row; r < targetSlot.Row + enemy.GridHeight; r++)
                {
                    _activeGrid.Add(new EnemyActiveGridSlot(c, r, enemy));
                }
            }
        }

        foreach (var pair in newTopLeftSlots)
        {
            var enemy = pair.Key;
            var slot = pair.Value;
            var targetPos = GetCenteredWorldPosition(slot.Column, slot.Row, enemy.GridWidth, enemy.GridHeight, currentOrientation);
            enemy.FlyToPosition(targetPos);
        }
    }

    private void OnEnemyDestroyed(EnemyShipController enemy)
    {
        var slots = _activeGrid.Where(s => s.Enemy == enemy).ToList();
        var topLeftSlot = slots.OrderBy(s => s.Column).ThenBy(s => s.Row).FirstOrDefault();

        GD.Print("Enemy killed", enemy.GetType().Name, "\nSlot:", topLeftSlot?.Column, "/", topLeftSlot?.Row);

        _activeGrid.RemoveAll(s => s.Enemy == enemy);

        if (_activeGrid.Count <= 0)
        {
            OnAllEnemiesDefeated?.Invoke();
        }
        else
        {
            OnEnemyDefeated?.Invoke();
        }
    }
}
