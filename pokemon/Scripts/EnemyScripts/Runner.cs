using Godot;
using System;

public partial class Runner : Enemy
{
	public Runner()
	{
		this.id = 0;
		this.speed = 400;
		this.health = 100;
	}
	
	public override void _Process(double delta)
	{
		if (health <= 0)
			Death();
		
		if (Position.DistanceTo(player.GlobalPosition) > 300)
		{
			this.speed = 600;
		}
		else 
			this.speed = 400;
	}
	
	private void Death()
	{
		player.money += 10;
		QueueFree();
	}
}
