using Godot;
using System;

public partial class Startmenu : Control
{
	public void StartGame()
	{
		MasterScene.Instance.ChangeScene("Level");
	}

	public void ExitGame()
	{
		GetTree().Quit();
	}
}
