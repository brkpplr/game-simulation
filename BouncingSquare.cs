using Godot;
using System;

public partial class BouncingSquare : CharacterBody2D
{
	[Export]
	public Color SquareColor = Colors.Red; // Default color
	public float Speed = 400.0f;
	public int Size = 20;

	public override void _Ready()
	{
		// Set the size of the square
		var rect = new RectangleShape2D();
		rect.Size = new Vector2(Size, Size);
		GetNode<CollisionShape2D>("CollisionShape2D").Shape = rect;

		// Set the size and offset of the ColorRect
		var colorRect = GetNode<ColorRect>("ColorRect");
		colorRect.Size = new Vector2(Size, Size);
		colorRect.Position = new Vector2(-Size / 2, -Size / 2);
		colorRect.Color = SquareColor; // Apply the exported color

		// Set a random initial velocity
		var random = new Random();
		Velocity = new Vector2(random.Next(-1, 2), random.Next(-1, 2)).Normalized() * Speed;
		if (Velocity == Vector2.Zero)
		{
			Velocity = Vector2.Right * Speed; // Avoid zero velocity
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 newPosition = Position + Velocity * (float)delta;

		Vector2 screenSize = GetViewportRect().Size;
		float halfSize = Size / 2.0f;

		// Check for collision with left wall
		if (newPosition.X - halfSize < 0)
		{
			newPosition.X = halfSize; // Adjust position to be at the wall
			Velocity = new Vector2(-Velocity.X, Velocity.Y); // Reverse horizontal velocity
		}
		// Check for collision with right wall
		else if (newPosition.X + halfSize > screenSize.X)
		{
			newPosition.X = screenSize.X - halfSize; // Adjust position to be at the wall
			Velocity = new Vector2(-Velocity.X, Velocity.Y); // Reverse horizontal velocity
		}

		// Check for collision with top wall
		if (newPosition.Y - halfSize < 0)
		{
			newPosition.Y = halfSize; // Adjust position to be at the wall
			Velocity = new Vector2(Velocity.X, -Velocity.Y); // Reverse vertical velocity
		}
		// Check for collision with bottom wall
		else if (newPosition.Y + halfSize > screenSize.Y)
		{
			newPosition.Y = screenSize.Y - halfSize; // Adjust position to be at the wall
			Velocity = new Vector2(Velocity.X, -Velocity.Y); // Reverse vertical velocity
		}

		Position = newPosition;
	}
}
