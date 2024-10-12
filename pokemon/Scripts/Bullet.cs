using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
	public int speed = 100;
	public double lifeTime = 3.0f;
	
	public override void _Ready()
	{
		Vector2 direction = new Vector2(1, 0).Rotated(Rotation);
		Velocity = direction * speed;
	}

	public override void _Process(double delta)
	{
		lifeTime -= delta;
		
		if (lifeTime <= 0)
		{
			QueueFree();
		}
		
		MoveAndCollide(Velocity * (float)delta * speed);
	}
}
