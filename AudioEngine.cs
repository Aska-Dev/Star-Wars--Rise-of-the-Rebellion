
using Godot;
using System.Collections.Generic;

public enum SoundEffect
{
    XWingShoot,
    TieShoot,
    TieFlyIn
}

public partial class AudioEngine : Node
{
    public static AudioEngine Instance { get; private set; } = null!;

    public float SoundEffectsVolume { get; set; } = 0.4f;

    private const string ConfigFolderPath = "res://Data/Sounds/";
    private const int MinDelayMsec = 100;

    private List<AudioStreamPlayer> _players = new();
    private Dictionary<SoundEffect, ulong> _lastPlayedMsec = new();
    private readonly Dictionary<SoundEffect, Sound> _sounds = new();

    public override void _Ready()
    {
        Instance = this;

        LoadConfigsFromFolder();

        for (int i = 0; i < 10; i++)
        {
            CreateNewPlayer();
        }
    }

    public void PlaySound(SoundEffect effect, bool randomizePitch = false)
    {
        if (!_sounds.TryGetValue(effect, out Sound? config)) return;

        ulong now = Time.GetTicksMsec();

        if (_lastPlayedMsec.TryGetValue(effect, out ulong lastPlayedTime))
        {
            if (now - lastPlayedTime < MinDelayMsec)
            {
                return;
            }
        }

        AudioStream streamToPlay = config.PrimaryStream;

        if (config.AlternateStream != null && GD.Randf() >= 0.5f)
        {
            streamToPlay = config.AlternateStream;
        }

        if (streamToPlay != null)
        {
            PlayInternal(streamToPlay, randomizePitch);
            _lastPlayedMsec[effect] = now;
        }
    }

    private void PlayInternal(AudioStream stream, bool randomizePitch)
    {
        foreach (var player in _players)
        {
            if (!player.Playing)
            {
                StartPlaying(player, stream, randomizePitch);
                return;
            }
        }

        var newPlayer = CreateNewPlayer();
        StartPlaying(newPlayer, stream, randomizePitch);
    }

    private AudioStreamPlayer CreateNewPlayer()
    {
        var player = new AudioStreamPlayer();
        AddChild(player);
        _players.Add(player);
        return player;
    }

    private void StartPlaying(AudioStreamPlayer player, AudioStream stream, bool randomizePitch)
    {
        player.Stream = stream;
        player.VolumeLinear = SoundEffectsVolume;
        player.PitchScale = randomizePitch ? (float)GD.RandRange(0.9f, 1.1f) : 1;
        player.Play();
    }

    private void LoadConfigsFromFolder()
    {
        using var dir = DirAccess.Open(ConfigFolderPath);
        if (dir == null)
        {
            GD.PrintErr($"AudioEngine: Ordner {ConfigFolderPath} nicht gefunden!");
            return;
        }

        dir.ListDirBegin();
        string fileName = dir.GetNext();

        while (fileName != "")
        {
            if (!dir.CurrentIsDir() && fileName.EndsWith(".tres"))
            {
                string cleanPath = ConfigFolderPath + fileName.Replace(".remap", "");
                var config = GD.Load<Sound>(cleanPath);

                if (config != null && config.PrimaryStream != null)
                {
                    _sounds[config.EffectName] = config;
                }
            }
            fileName = dir.GetNext();
        }
    }
}