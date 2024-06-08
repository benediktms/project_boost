using Godot;

namespace project_boost;

public partial class LandingPad : Node
{
    [Export(PropertyHint.File, "*.tscn")] public string FilePath { get; set; }
}