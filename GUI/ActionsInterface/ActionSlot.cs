using Godot;
using System;
using System.Collections.Generic;

public partial class ActionSlot : Control
{
	public List<TextureRect> Keys { get; set; } = [];

	private ActionIcon _icon = null!;

	public override void _Ready()
	{
		var keysNode = GetNode<Control>("Keys");

		foreach(var child in keysNode.GetChildren())
		{
			if(child is TextureRect key)
			{
				Keys.Add(key);
			}
		}
	}
	
	public void Setup(int index, PackedScene iconScene)
	{
        Keys[index].Visible = true;
        _icon = iconScene.Instantiate<ActionIcon>();
        GetNode<Control>("ActionIconContainer").AddChild(_icon);
    }

	public void StartCooldown(float cooldown) => _icon.StartCooldown(cooldown);
}
