using Godot;
using System;

[GlobalClass]
public partial class GozantiController : EnemyShipController
{
    public override void PerformAction()
    {
        Components.GetComponent<EnemyWeaponComponent>().ShootSalve();
    }
}
