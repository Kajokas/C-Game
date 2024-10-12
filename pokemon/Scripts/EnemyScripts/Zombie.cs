using Godot;
using System;

public partial class Zombie : Enemy
{
	public Zombie()
	{
		this.id = 0;
		this.speed = 200;
		this.health = 100;
	}
	
	public override void _Process(double delta)
	{
		if (health <= 0)
			Death();
	}
	
	private void Death()
	{
		player.money += 5;
		QueueFree();
	}
}
