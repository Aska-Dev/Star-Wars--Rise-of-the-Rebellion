using Godot;
using System;

public partial class SettingsMenu : Control
{
	public void GoBackToMenu()
	{
		MasterScene.Instance.ChangeScene("Home");
    }

	public void SetMusicVolume(float volume)
    {
        AudioEngine.Instance.SetMusicVolume(volume * 0.01f);
    }

    public void SetSfxVolume(float volume)
    {
        AudioEngine.Instance.SetSoundEffectsVolume(volume * 0.01f);
    }

    public void SetAmbienceVolume(float volume)
    {
        AudioEngine.Instance.SetAmbienceVolume(volume * 0.01f);
    }
}
