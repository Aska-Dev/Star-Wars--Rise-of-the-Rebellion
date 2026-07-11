using Godot;
using Godot.NativeInterop;
using System;

public partial class MasterScene : Node
{
    public static MasterScene Instance { get; private set; } = null!;

    [Signal] public delegate void SceneChangedEventHandler();
    
    [Export] public Godot.Collections.Dictionary<string, PackedScene> RegisteredScenes { get; set; } = new();
    [Export] public PackedScene PauseMenu { get; set; } = null!;

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

        Callable.From(() => ChangeScene("Home", false)).CallDeferred();
    }

    public override void _Process(double delta)
    {
        // HANDLE ESCAPE KEY FOR PAUSE MENU
        if (Input.IsActionJustPressed("ui_cancel") && _currentSceneNode is not Startmenu)
        {
            if (_currentSceneNode is not null && HasNode("PauseMenu"))
            {
                var pauseMenu = GetNode<PauseMenu>("PauseMenu");
                pauseMenu.ContinueGame();
            }
            else
            {
                var pauseMenu = PauseMenu.Instantiate<PauseMenu>();
                AddChild(pauseMenu);
            }
        }
    }

    public void ChangeScene(string sceneName, bool doTransition = true)
    {
        if (RegisteredScenes.TryGetValue(sceneName, out PackedScene? nextScene))
        {
            SwitchScene(nextScene, doTransition);
        }
        else
        {
            GD.PrintErr($"MasterScene: Scene '{sceneName}' is not registered");
        }
    }

    public void AddToScene(Node? child)
    {
        if(_currentSceneNode is not null)
        {
            _currentSceneNode.AddChild(child);
        }
        else
        {
            throw new InvalidOperationException("Master Scene: Cannot add child to scene, current scene is null");
        }
    }

    private void SwitchScene(PackedScene newScene, bool transition)
    {
        _nextSceneNode = newScene.Instantiate();

        if(transition)
        {
            AnimationPlayer.Play("fadeIn");
            AnimationPlayer.AnimationFinished += LoadNewScene;
        }
        else
        {
            LoadNewScene();
        }

        // Stop all sounds when switching scenes
        AudioEngine.Instance.StopMusic();
        AudioEngine.Instance.StopAmbience();

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
