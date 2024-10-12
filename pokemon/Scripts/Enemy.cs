using Godot;
using System;

public abstract partial class Enemy : CharacterBody2D
{
	public Player_Controller player;
	
	public int id;
	
	public int speed;
	public int health;

	private ProgressBar healthUI;
	
	private Area2D detectionArea;
	
	public override void _Ready()
	{	
		var players = GetTree().GetNodesInGroup("Player");
		player = players[0] as Player_Controller;
		
		detectionArea = GetNode<Area2D>("Area2D");
		detectionArea.BodyEntered += OnBodyEntered;
		
		healthUI = GetChild(-1) as ProgressBar;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		//GD.Print(body.GetGroups());
		if (body.IsInGroup("Knife"))
		{
			health -= 5;
		}
		if (body.IsInGroup("Bullet"))
		{
			health -= 10;
			body.QueueFree();
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		healthUI.Value = health;
		
		Velocity = Position.DirectionTo(player.GlobalPosition) * speed;
		if (Position.DistanceTo(player.GlobalPosition) > 10)
		{
			MoveAndSlide();
		}
		
		KinematicCollision2D collision = GetLastSlideCollision();
		if (collision != null)
		{
			Node2D contact = collision.GetCollider() as Node2D;
			if (contact.IsInGroup("Player"))
			{
				player.health--;
			}
		}
	}
}
