using Godot;
using System;
using System.Runtime.InteropServices.Marshalling;

[GlobalClass]
public partial class GozantiShipModel : ShipModel
{
	[Export] public AnimationPlayer TieAnimationPlayer { get; set; } = null!;
}
