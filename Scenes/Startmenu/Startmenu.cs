using Godot;
using System;

public partial class Startmenu : Control
{
    public override void _Ready()
    {
		AudioEngine.Instance.PlayTheme(MusicThemes.Menu);
    }

	public void StartGame()
	{
		MasterScene.Instance.ChangeScene("Level");
	}

	public void ExitGame()
	{
		GetTree().Quit();
	}
}
