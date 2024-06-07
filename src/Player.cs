using Godot;

namespace project_boost;

public partial class Player : RigidBody3D
{
    [Export(PropertyHint.Range, "750.0,3000.0")] private float _thrust = 1000;
    [Export] private float _torqueThrust = 200;

    private Player()
    {
        ContactMonitor = true;
        AxisLockLinearZ = true;
        AxisLockAngularX = true;
        AxisLockAngularY = true;
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
            ApplyTorque(new(0, 0, (float)delta * _torqueThrust));
        }

        if (Input.IsActionPressed("rotate_right"))
        {
            ApplyTorque(new(0, 0, (float)delta * -_torqueThrust));
        }
    }

    private void OnBodyEntered(Node node)
    {
        var groups = node.GetGroups();
        if (groups.Contains("Goal"))
        {
            CompleteLevel();
        }
        else if (groups.Contains("Obstacle"))
        {
            CrashSequence();
        }
    }

    private void CrashSequence()
    {
        GD.Print("Boom");
        GetTree().ReloadCurrentScene();
    }

    private void CompleteLevel()
    {
        GD.Print("Level complete");
        GetTree().Quit();
    }
}
