using Godot;
using System;

public partial class PauseMenu : Control
{
	public override void _Ready()
	{
		GetTree().Paused = true;
    }

    public void ContinueGame()
    {
        GetTree().Paused = false;
        QueueFree();
    }

    public void ExitGame()
    {
        ContinueGame();
        MasterScene.Instance.ChangeScene("Home");
    }
}
