
using Godot;
using System.Collections.Generic;

public enum SoundEffects
{
    XWingShoot,
    TieShoot,
    TieFlyIn,
    TieBomberFly,
    MissileLaunch,
    ShipExplosion,
    AstromechActivation,
    AstromechReady
}

public enum MusicThemes
{
    SpaceBattle,
    Menu
}

public partial class AudioEngine : Node
{
    public static AudioEngine Instance { get; private set; } = null!;

    public float SoundEffectsVolume { get; set; } = 0.4f;
    public float MusicVolume { get; set; } = 0.5f;

    private const string ConfigFolderPath = "res://Data/Sounds/";
    private const string MusicFolderPath = "res://Data/Music/";
    private const int MinDelayMsec = 100;

    // --- SFX ---
    private List<AudioStreamPlayer> _players = [];
    private Dictionary<SoundEffects, ulong> _lastPlayedMsec = [];
    private readonly Dictionary<SoundEffects, Sound> _sounds = [];

    // --- MUSIC ---
    private AudioStreamPlayer _musicPlayer = null!;
    private readonly Dictionary<MusicThemes, MusicTheme> _musicThemes = [];
    private MusicTheme? _currentTheme;
    private AudioStream? _lastPlayedTrack;

    public override void _Ready()
    {
        Instance = this;

        // Load Resources
        LoadConfigsFromFolder();
        LoadMusicConfigsFromFolder();

        // Setup SFX-Players
        for (int i = 0; i < 10; i++)
        {
            CreateNewPlayer();
        }

        // Setup Music Player
        _musicPlayer = new AudioStreamPlayer();
        AddChild(_musicPlayer);
        _musicPlayer.Finished += OnMusicTrackFinished;
    }

    public void PlaySound(SoundEffects effect, bool randomizePitch = false)
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

    public void PlayTheme(MusicThemes themeType)
    {
        if (!_musicThemes.TryGetValue(themeType, out MusicTheme? theme))
        {
            GD.PrintErr($"AudioEngine: MusicTheme '{themeType}' nicht gefunden!");
            return;
        }

        if (theme.Tracks.Count == 0) return;

        _currentTheme = theme;
        PlayNextRandomTrack();
    }

    private void OnMusicTrackFinished()
    {
        if (_currentTheme != null)
        {
            PlayNextRandomTrack();
        }
    }

    private void PlayNextRandomTrack()
    {
        if (_currentTheme == null || _currentTheme.Tracks.Count == 0) return;

        AudioStream? nextTrack = null;

        if (_currentTheme.Tracks.Count == 1)
        {
            nextTrack = _currentTheme.Tracks[0];
        }
        else
        {
            do
            {
                nextTrack = _currentTheme.Tracks[GD.RandRange(0, _currentTheme.Tracks.Count - 1)];
            } while (nextTrack == _lastPlayedTrack);
        }

        _lastPlayedTrack = nextTrack;
        _musicPlayer.Stream = nextTrack;
        _musicPlayer.VolumeLinear = MusicVolume;
        _musicPlayer.Play();
    }

    public void StopMusic()
    {
        _currentTheme = null;
        _lastPlayedTrack = null;
        _musicPlayer.Stop();
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

    private void LoadMusicConfigsFromFolder()
    {
        using var dir = DirAccess.Open(MusicFolderPath);
        if (dir == null) return;

        dir.ListDirBegin();
        string fileName = dir.GetNext();

        while (fileName != "")
        {
            if (!dir.CurrentIsDir() && fileName.EndsWith(".tres"))
            {
                string cleanPath = MusicFolderPath + fileName.Replace(".remap", "");
                var config = GD.Load<MusicTheme>(cleanPath);

                if (config != null)
                {
                    _musicThemes[config.Theme] = config;
                }
            }
            fileName = dir.GetNext();
        }
    }
}