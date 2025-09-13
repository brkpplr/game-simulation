using Godot;
using System;

public partial class BouncingCircle : CharacterBody2D
{
	[Export]
	public Color CircleColor = Colors.Red; // Default color
	public float Speed = 1000.0f;
	public int Radius = 40;

	public override void _Ready()
	{
		// Set the size of the circle
		var circle = new CircleShape2D();
		circle.Radius = Radius;
		GetNode<CollisionShape2D>("CollisionShape2D").Shape = circle;

		// Set a random initial velocity with a random angle
		var random = new Random();
		float randomAngle = (float)(random.NextDouble() * 2 * Mathf.Pi); // Random angle in radians (0 to 2*PI)
		Velocity = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).Normalized() * Speed;
	}

	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, Radius, CircleColor);
	}

	public bool IsCollidingWith(BouncingCircle otherCircle)
	{
		// Calculate the half-sizes for both circles (which is their radius)
		float thisRadius = this.Radius;
		float otherRadius = otherCircle.Radius;

		// Calculate the centers of both circles (Position is already the center)
		Vector2 thisCenter = this.Position;
		Vector2 otherCenter = otherCircle.Position;

		// Calculate the absolute distance between the centers on each axis
		float deltaX = Mathf.Abs(thisCenter.X - otherCenter.X);
		float deltaY = Mathf.Abs(thisCenter.Y - otherCenter.Y);

		// Perform an AABB check using radii (as per 'do not change collision functions' constraint)
		return (deltaX < (thisRadius + otherRadius)) && (deltaY < (thisRadius + otherRadius));
	}

	private System.Collections.Generic.List<BouncingCircle> GetAllOtherBouncingCircles()
	{
		System.Collections.Generic.List<BouncingCircle> otherCircles = new System.Collections.Generic.List<BouncingCircle>();
		Node parent = GetParent();

		if (parent != null)
		{
			foreach (Node child in parent.GetChildren())
			{
				if (child is BouncingCircle otherCircle && otherCircle != this)
				{
					otherCircles.Add(otherCircle);
				}
			}
		}
		return otherCircles;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 newPosition = Position + Velocity * (float)delta;

		Vector2 screenSize = GetViewportRect().Size;
		float currentRadius = Radius;

		// Check for collision with left wall
		if (newPosition.X - currentRadius < 0)
		{
			newPosition.X = currentRadius; // Adjust position to be at the wall
			Velocity = new Vector2(-Velocity.X, Velocity.Y); // Reverse horizontal velocity
		}
		// Check for collision with right wall
		else if (newPosition.X + currentRadius > screenSize.X)
		{
			newPosition.X = screenSize.X - currentRadius; // Adjust position to be at the wall
			Velocity = new Vector2(-Velocity.X, Velocity.Y); // Reverse horizontal velocity
		}

		// Check for collision with top wall
		if (newPosition.Y - currentRadius < 0)
		{
			newPosition.Y = currentRadius; // Adjust position to be at the wall
			Velocity = new Vector2(Velocity.X, -Velocity.Y); // Reverse vertical velocity
		}
		// Check for collision with bottom wall
		else if (newPosition.Y + currentRadius > screenSize.Y)
		{
			newPosition.Y = screenSize.Y - currentRadius; // Adjust position to be at the wall
			Velocity = new Vector2(Velocity.X, -Velocity.Y); // Reverse vertical velocity
		}

		foreach (BouncingCircle otherCircle in GetAllOtherBouncingCircles())
		{
			if (this != otherCircle && IsCollidingWith(otherCircle))
			{
				Vector2 collisionNormal = (this.Position - otherCircle.Position).Normalized();
				this.Velocity = collisionNormal * this.Speed;

				Vector2 otherCollisionNormal = (otherCircle.Position - this.Position).Normalized();
				otherCircle.Velocity = otherCollisionNormal * otherCircle.Speed;
			}
		}

		Position = newPosition;
	}
}
