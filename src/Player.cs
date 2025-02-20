using Godot;

namespace project_boost;

public partial class Player : RigidBody3D
{
    [Export(PropertyHint.Range, "750.0,3000.0")]
    private float _thrust = 1000;

    [Export] private float _torqueThrust = 200;

    private bool _isTransitioning;
    private string _nextLevelFile;
    private AudioStreamPlayer _explosionAudio;
    private AudioStreamPlayer _stageCompleteAudio;

    private Player()
    {
        ContactMonitor = true;
        AxisLockLinearZ = true;
        AxisLockAngularX = true;
        AxisLockAngularY = true;
    }

    public override void _Ready()
    {
        base._Ready();
        _explosionAudio = GetNode<AudioStreamPlayer>("ExplosionAudio");
        _stageCompleteAudio = GetNode<AudioStreamPlayer>("StageCompleteAudio");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionPressed("boost"))
        {
            ApplyCentralForce(Basis.Y * (float)(delta * 1000));
        }

        if (Input.IsActionPressed("rotate_left"))
        {
            ApplyTorque(new Vector3(0, 0, (float)delta * _torqueThrust));
        }

        if (Input.IsActionPressed("rotate_right"))
        {
            ApplyTorque(new Vector3(0, 0, (float)delta * -_torqueThrust));
        }
    }

    private void OnBodyEntered(Node node)
    {
        if (_isTransitioning) return;

        var groups = node.GetGroups();
        if (groups.Contains("Goal"))
        {
            var landingPad = node as LandingPad;
            CompleteLevel(landingPad?.FilePath);
        }
        else if (groups.Contains("Obstacle"))
        {
            CrashSequence();
        }
    }

    private void CrashSequence()
    {
        GD.Print("Boom");
        _explosionAudio.Play();
        _isTransitioning = true;
        SetProcess(false);
        var tween = CreateTween();
        tween.TweenInterval(_explosionAudio.Stream.GetLength());
        tween.TweenCallback(new Callable(this, nameof(RestartLevel)));
    }

    private void RestartLevel()
    {
        GetTree().ReloadCurrentScene();
    }

    private void CompleteLevel(string nextLevelFile)
    {
        _nextLevelFile = nextLevelFile;
        GD.Print("Level complete");
        _stageCompleteAudio.Play();
        _isTransitioning = true;
        SetProcess(false);
        var tween = CreateTween();
        tween.TweenInterval(_stageCompleteAudio.Stream.GetLength());
        tween.TweenCallback(new Callable(this, nameof(LoadNextLevel)));
    }

    public void LoadNextLevel()
    {
        if (_nextLevelFile != null)
        {
            GetTree().ChangeSceneToFile(_nextLevelFile);
        }
    }
}