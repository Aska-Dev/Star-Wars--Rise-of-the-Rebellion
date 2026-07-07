using Godot;
using System;

public partial class TabBar : Godot.TabBar
{
	[Export] public Control SoundTabContent { get; set; } = null!;
	[Export] public Control ControlsTabContent { get; set; } = null!;

	private Control[] _tabContents = [];

    public override void _Ready()
    {
        InitVolumeSettings();

        _tabContents = [SoundTabContent, ControlsTabContent];
        TabSelected += OnTabSelected;
    }

    private void OnTabSelected(long tabIndex)
    {
        for (int i = 0; i < _tabContents.Length; i++)
        {
            _tabContents[i].Visible = (i == tabIndex);
        }
    }

    public void InitVolumeSettings()
    {
        var musicControl = SoundTabContent.GetNode<SliderComponent>("MusicVolume");
        var sfxControl = SoundTabContent.GetNode<SliderComponent>("SFXVolume");
        var ambienceControl = SoundTabContent.GetNode<SliderComponent>("AmbienceVolume");

        musicControl.SetValue(AudioEngine.Instance.MusicVolume * 100);
        sfxControl.SetValue(AudioEngine.Instance.SoundEffectsVolume * 100);
        ambienceControl.SetValue(AudioEngine.Instance.AmbienceVolume * 100);
    }
}
