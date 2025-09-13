using Godot;
using System;

public partial class CameraControl : Camera2D
{
	[Export]
	public float ZoomLevel = 1.0f; // Default zoom level

	public override void _Ready()
	{
		Zoom = new Vector2(ZoomLevel, ZoomLevel);
	}
}
