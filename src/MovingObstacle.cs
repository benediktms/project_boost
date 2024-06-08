using Godot;

namespace project_boost;

public partial class MovingObstacle : AnimatableBody3D
{
    [Export] private Vector3 _destination;
    [Export] private float _duration = 1;

    public override void _Ready()
    {
        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(this, "GlobalPosition", GlobalPosition + _destination, 2);
        tween.TweenProperty(this, "GlobalPosition", GlobalPosition, 2);
    }
}