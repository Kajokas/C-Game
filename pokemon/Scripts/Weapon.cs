using Godot;
using System;

public partial class Weapon : StaticBody2D
{
	private PackedScene bullet =  GD.Load<PackedScene>("res://Scenes/bullet.tscn");
	public Node2D hand;
	private Label ammoCounterUI;
	
	public int ammo = 0;
	public int curMagAmmo = 0;
	
	public delegate void calculateAmmo(int a, int b, int c);
	
	public override void _Ready()
	{
		hand = this.GetParent() as Node2D;
		ammoCounterUI = this.GetParent().GetParent().GetNode("Camera2D/Canvas/Control/AmmoCounter") as Label;
	}
	
	[Export]
	public int id;
	
	public override void _Notification(int what)
	{
		if (what == NotificationVisibilityChanged)
		{
			if (IsVisibleInTree())
			{
				OnBecameVisible();
			}
			else
			{
				OnBecameInvisible();
			}
		}
	}
	
	public override void _Process(double delta)
	{
		ammoCounterUI.Text = "Ammo: " + curMagAmmo + "/" + ammo;
	}

	private void OnBecameVisible()
	{
		if (id == 0)
		{
			ammoCounterUI.Visible = false;
		}
	}

	private void OnBecameInvisible()
	{
		if (id == 0)
		{
			ammoCounterUI.Visible = true;
		}
	}

	public void Attack()
	{
		if ((this.id == 1 || this.id == 2) && curMagAmmo>0)
		{
			var newBullet = bullet.Instantiate() as Node2D;
			newBullet.Rotation = this.GlobalRotation;
			Vector2 bulletOffset = new Vector2(50, 0).Rotated(this.GlobalRotation);
			newBullet.Position = this.GlobalPosition + bulletOffset;
			var Map = this.GetTree().Root.GetChild(0);
			Map.AddChild(newBullet);
			curMagAmmo--;
		}
	}
	
	public void Reload()
	{
		calculateAmmo newCalc = new calculateAmmo(LastBullets);
		switch(id)
		{
			case 1:
				newCalc(curMagAmmo, ammo, 10);
				break;
			case 2:
				newCalc(curMagAmmo, ammo, 30);
				break;
		}
	}
	
	public void LastBullets(int curr, int all, int max)
	{
		if (all > 0)
		{
			switch (curr)
			{
				case int x when x<max && all >= max:
					ammo -= (max-curr);
					curMagAmmo = max;
					break;
				case int x when x<max && all < max && all + curr < max:
					ammo = 0;
					curMagAmmo = curr+all;
					break;
				case int x when x<max && all < max && all + curr >= max:
					ammo = all+curr-max;
					curMagAmmo = max;
					break;
				default:
					break;
			}
		}	
	}
}
