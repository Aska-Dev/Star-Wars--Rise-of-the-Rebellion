using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnemyGridSlot(int column, int row)
{
    public int Column { get; protected set; } = column;
    public int Row { get; protected set; } = row;
}

public class EnemyFormationSlot(int column, int row, PackedScene enemy, float spawnDelay = 0) : EnemyGridSlot(column, row)
{
    public PackedScene Enemy { get; set; } = enemy;
    public float SpawnDelay { get; set; } = spawnDelay;
}

public class EnemyActiveGridSlot(int column, int row, EnemyShipController enemy) : EnemyGridSlot(column, row)
{
    public EnemyShipController Enemy { get; set; } = enemy;
}
