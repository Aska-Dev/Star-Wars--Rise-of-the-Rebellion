
using Godot;
using System.Collections.Generic;

public enum SoundEffect
{
    XWingShoot,
    TieShoot
}

public partial class AudioEngine : Node
{
    public static AudioEngine Instance { get; private set; } = null!;

    private List<AudioStreamPlayer> _players = new();
    private Dictionary<SoundEffect, ulong> _lastPlayedMsec = new();

    private Dictionary<SoundEffect, AudioStream> _streamCache = new();

    private readonly Dictionary<SoundEffect, string> _soundPaths = new()
    {
        { SoundEffect.XWingShoot, "res://Resources/sounds/guns/xwing-fire.mp3" },
        { SoundEffect.TieShoot, "res://Audio/SFX/player_laser.mp3" },
    };

    private const int MinDelayMsec = 50;

    public AudioEngine()
    {
        Instance = this;

        for (int i = 0; i < 10; i++)
        {
            var player = new AudioStreamPlayer();
            AddChild(player);
            _players.Add(player);
        }
    }

    public void PlaySound(SoundEffect effect, bool randomizePitch = false)
    {
        ulong now = Time.GetTicksMsec();

        if (_lastPlayedMsec.TryGetValue(effect, out ulong lastPlayedTime))
        {
            if (now - lastPlayedTime < MinDelayMsec)
            {
                return;
            }
        }

        // 2. Stream laden oder aus Cache holen
        var stream = GetOrLoadStream(effect);
        if (stream == null) return;

        _lastPlayedMsec[effect] = now;

        foreach (var player in _players)
        {
            if (!player.Playing)
            {
                player.Stream = stream;
                
                if(randomizePitch)
                {
                    player.PitchScale = (float)GD.RandRange(0.9f, 1.1f);
                }

                player.Play();
                return;
            }
        }
    }

    private AudioStream? GetOrLoadStream(SoundEffect effect)
    {
        if (_streamCache.TryGetValue(effect, out var cachedStream))
        {
            return cachedStream;
        }

        if (_soundPaths.TryGetValue(effect, out string path))
        {
            var stream = GD.Load<AudioStream>(path);
            _streamCache[effect] = stream;
            return stream;
        }

        GD.PrintErr($"AudioEngine: Pfad für SoundEffect '{effect}' nicht gefunden!");
        return null;
    }
}