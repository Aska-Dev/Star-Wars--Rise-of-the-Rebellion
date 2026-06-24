using Godot;
using System;

public partial class MasterScene : Node
{
	public static MasterScene Instance { get; private set; }

    [Signal] public delegate void SceneChangedEventHandler();
    [Export] public Godot.Collections.Dictionary<string, PackedScene> RegisteredScenes { get; set; } = new();

    public AnimationPlayer AnimationPlayer { get; set; } = null!;
    public Node SceneContainer { get; set;  } = null!;

    private Node? _currentSceneNode;
    private Node? _nextSceneNode;

    public override void _Ready()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }

        Instance = this;

        SceneContainer = GetNode<Node>("SceneContainer");
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        GD.Print($"MasterScene initialisiert. Anzahl registrierter Szenen: {RegisteredScenes?.Count ?? 0}");
        Callable.From(() => ChangeScene("Home", false)).CallDeferred();
    }

    public void ChangeScene(string sceneName, bool doTransition = true)
    {
        GD.Print("Scene change");
        if (RegisteredScenes.TryGetValue(sceneName, out PackedScene? nextScene))
        {
            SwitchScene(nextScene, doTransition);
        }
        else
        {
            GD.PrintErr($"MasterScene: Scene '{sceneName}' is not registered");
        }
    }

    private void SwitchScene(PackedScene newScene, bool transition)
    {
        _nextSceneNode = newScene.Instantiate();

        if(transition)
        {
            AnimationPlayer.Play("fadeIn");
            AnimationPlayer.AnimationFinished -= LoadNewScene;
            AnimationPlayer.AnimationFinished += LoadNewScene;
        }
        else
        {
            LoadNewScene();
        }

    }

    private void LoadNewScene(StringName animationName)
    {
        AnimationPlayer.AnimationFinished -= LoadNewScene;
        AnimationPlayer.Play("fadeOut");

        LoadNewScene();
    }

    private void LoadNewScene()
    {
        if (_currentSceneNode != null)
        {
            _currentSceneNode.QueueFree();
        }

        _currentSceneNode = _nextSceneNode;
        _nextSceneNode = null;

        SceneContainer.AddChild(_currentSceneNode);

        EmitSignalSceneChanged();
    }
}
