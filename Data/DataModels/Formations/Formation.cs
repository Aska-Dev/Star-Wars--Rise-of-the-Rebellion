using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;

[GlobalClass]
public abstract partial class Formation : Resource
{
    public int RequiredEnemyCount => GetEnemyProperties().Count;

    public abstract List<EnemyFormationSlot> Build();
    public void InjectEnemyTypes(List<PackedScene> enemyTypes)
    {
        var properties = GetEnemyProperties();
        for (int i = 0; i < Math.Min(properties.Count, enemyTypes.Count); i++)
        {
            properties[i].SetValue(this, enemyTypes[i]);
        }
    }

    private List<PropertyInfo> GetEnemyProperties()
    {
        return [.. GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType == typeof(PackedScene) && p.GetCustomAttribute<ExportAttribute>() != null)];
    }

}
