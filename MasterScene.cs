using Godot;
using Godot.NativeInterop;
using System;

public partial class MasterScene : Node
{
    public static MasterScene Instance { get; private set; } = null!;

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

        Callable.From(() => ChangeScene("Home", false)).CallDeferred();
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
