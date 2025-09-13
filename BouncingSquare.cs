using Godot;
using System;

public partial class BouncingSquare : CharacterBody2D
{
	[Export]
	public Color SquareColor = Colors.Red; // Default color
	public float Speed = 1000.0f;
	public int Size = 40;

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

		// Set a random initial velocity with a random angle
		var random = new Random();
		float randomAngle = (float)(random.NextDouble() * 2 * Mathf.Pi); // Random angle in radians (0 to 2*PI)
		Velocity = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).Normalized() * Speed;
	}

	public bool IsCollidingWith(BouncingSquare otherSquare)
	{
		// Calculate the half-sizes for both squares
		float thisHalfSize = this.Size / 2.0f;
		float otherHalfSize = otherSquare.Size / 2.0f;

		// Calculate the centers of both squares (Position is already the center)
		Vector2 thisCenter = this.Position;
		Vector2 otherCenter = otherSquare.Position;

		// Calculate the absolute distance between the centers on each axis
		float deltaX = Mathf.Abs(thisCenter.X - otherCenter.X);
		float deltaY = Mathf.Abs(thisCenter.Y - otherCenter.Y);

		// If the sum of half-sizes is greater than the distance between centers on both axes, they are colliding
		return (deltaX < (thisHalfSize + otherHalfSize)) && (deltaY < (thisHalfSize + otherHalfSize));
	}

	private System.Collections.Generic.List<BouncingSquare> GetAllOtherBouncingSquares()
	{
		System.Collections.Generic.List<BouncingSquare> otherSquares = new System.Collections.Generic.List<BouncingSquare>();
		Node parent = GetParent();

		if (parent != null)
		{
			foreach (Node child in parent.GetChildren())
			{
				if (child is BouncingSquare otherSquare && otherSquare != this)
				{
					otherSquares.Add(otherSquare);
				}
			}
		}
		return otherSquares;
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

		// Placeholder for square-to-square collision detection
		// In a real scenario without Godot's built-in physics, you would need a way
		// to get all other BouncingSquare instances in the scene and iterate through them.
		// For example:
		foreach (BouncingSquare otherSquare in GetAllOtherBouncingSquares())
		{
			if (this != otherSquare && IsCollidingWith(otherSquare))
			{
				otherSquare.Velocity = new Vector2(-otherSquare.Velocity.X, -otherSquare.Velocity.Y);
				this.Velocity = new Vector2(-this.Velocity.X, -this.Velocity.Y);
			}
		}

		Position = newPosition;
	}
}
