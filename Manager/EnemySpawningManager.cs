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

    public async void SpawnFormation(List<EnemyFormationSlot> formationSlots)
    {
        foreach(var slot in formationSlots)
        {
            if (slot.SpawnDelay > 0f)
            {
                await GetTree().Root.ToSignal(GetTree().CreateTimer(slot.SpawnDelay), SceneTreeTimer.SignalName.Timeout);
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

        var currentOrientation = GameManager.CurrentOrientation;
        var targetPosition = SpawnGridModule.GetWorldPosition(column, row, currentOrientation);

        var spawnDirection = currentOrientation.GetOpposite().GetDirectionVector();
        var spawnPosition = targetPosition + (spawnDirection * 500f);

        var enemy = enemyScene.Instantiate<EnemyShipController>();
        _activeGrid.Add(new(column, row, enemy));
        
        enemy.GlobalPosition = spawnPosition;
        _enemyContainerNode.AddChild(enemy);

        enemy.FlyToPosition(targetPosition);
        enemy.OnDestroy += () => OnEnemyDestroyed(enemy);
    }

    private void OnEnemyDestroyed(EnemyShipController enemy)
    {
        var slot = _activeGrid.FirstOrDefault(s => s.Enemy == enemy);
        if (slot is not null)
        {
            _activeGrid.Remove(slot);
        }

        if (_activeGrid.Count <= 0)
        {
            OnAllEnemiesDefeated?.Invoke();
        }
        else
        {
            OnEnemyDefeated?.Invoke();
        }
    }

    public void ShuffleGrid()
    {
        var enemies = _activeGrid.Select(g => g.Enemy).ToList();
        var freeSlots = GetFreeSlots();

        var newSlots = freeSlots.OrderBy(x => GD.Randf()).Take(enemies.Count).ToList();

        // 3. Jetzt erst das Grid leeren und mit den neuen Positionen befüllen
        _activeGrid.Clear();

        var currentOrientation = GameManager.CurrentOrientation;

        for (int i = 0; i < enemies.Count; i++)
        {
            var slot = newSlots[i];
            _activeGrid.Add(new EnemyActiveGridSlot(slot.Column, slot.Row, enemies[i]));

            var targetPos = SpawnGridModule.GetWorldPosition(slot.Column, slot.Row, currentOrientation);
            enemies[i].FlyToPosition(targetPos);
        }
    }
}
