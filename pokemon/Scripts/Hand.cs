using Godot;
using System;

public partial class Hand : Node2D
{
	public override void _Process(double delta)
	{
		Vector2 cursorPos = GetLocalMousePosition();
		this.Rotation += cursorPos.Angle();
	}
}
