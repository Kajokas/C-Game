using Godot;
using System;

public partial class Shop : Area2D
{
	private Area2D detectionArea;
	private Control shopUI;
	
	private PackedScene pistol =  GD.Load<PackedScene>("res://Scenes/Pistol.tscn");
	private PackedScene rifle = GD.Load<PackedScene>("res://Scenes/Rifle.tscn");
	
	public override void _Ready()
	{
		this.BodyEntered += OnBodyEntered;
		this.BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			shopUI = body.GetNode("Camera2D/Canvas/Control/ShopPanel") as Control;
			shopUI.Visible = true;
		}
	}
	
	private void OnBodyExited(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			shopUI.Visible = false;
		}
	}
	
	public void GiveGun(Player_Controller player)
	{
		//GD.Print(player.Name);
		var newPistol = pistol.Instantiate() as Weapon;
		player.GetNode("Hand").AddChild(newPistol);
		player.weapons.Add(newPistol);
	}
	
	public void GiveRifle(Player_Controller player)
	{
		var newRifle = rifle.Instantiate() as Weapon;
		player.GetNode("Hand").AddChild(newRifle);
		player.weapons.Add(newRifle);
	}
}
