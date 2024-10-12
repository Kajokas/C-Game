using Godot;
using System;
using System.Collections.Generic;

public class FlavorText : IFormattable
{
	private int time;
	private int money;
	
	public FlavorText(int t, int m)
	{
		this.time = t;
		this.money = m;
	}
	
	~FlavorText()
	{
		GD.Print("Bye");
	}
	
	public string ToString(string reason, IFormatProvider formatProvider)
	{
		switch(reason)
		{
			case "B":
				return "You should consider saving up some money next time";
			case "M":
				return "You shouldn't be fighting during shopping phase";
			case "T":
				return "Have you considered that you should spend money";
			case "S":
				return "Did you spend all your money on cool things and still lose?";
			default:
				return "You died " + time + " seconds left to the next round";
		}
	}
}

public partial class Player_Controller : CharacterBody2D
{
	private int speed = 500;
	private Vector2 currentVelocity;
	
	public int health = 100;
	private ProgressBar healthUI;
	private CanvasLayer DeathScreen;
	
	public int money = 0;
	private Label moneyUI;
	
	private PackedScene knife = GD.Load<PackedScene>("res://Scenes/knife.tscn");
	
	private bool isHoldingAttack = false;
	private bool attackCooldown = false;
	
	//[Export]
	//public Weapon[] weapons;
	public List<Weapon> weapons = new List<Weapon>();
	private int currWeapon = 0;
	
	public override void _Ready()
	{
		moneyUI = this.GetNode("Camera2D/Canvas/Control/MoneyUI") as Label;
		healthUI = this.GetNode("Camera2D/Canvas/Control/HPUI") as ProgressBar;
		DeathScreen = this.GetTree().Root.GetNode("MasterNode/DeathUI") as CanvasLayer;
		
		Weapon newKnife;
		getKnife(out newKnife);
		this.GetNode("Hand").AddChild(newKnife);
		weapons.Add(newKnife);
	}
	
	public void getKnife(out Weapon newKnife)
	{
		newKnife = knife.Instantiate() as Weapon;
	}
	
	public override void _Input(InputEvent @event)
{
	if (@event is InputEventMouseButton mouseEvent)
	{
		switch (mouseEvent.ButtonIndex)
		{
			case MouseButton.WheelUp:
				if (mouseEvent.Pressed)
				{
					weapons[currWeapon].Visible = false;
					weapons[currWeapon].SetProcess(false);
					if (currWeapon >= weapons.Count - 1)
						currWeapon = 0;
					else
						currWeapon++;
					weapons[currWeapon].Visible = true;
					weapons[currWeapon].SetProcess(true);
				}
				break;
			case MouseButton.WheelDown:
				if(mouseEvent.Pressed)
				{
					weapons[currWeapon].Visible = false;
					weapons[currWeapon].SetProcess(false);
					if (currWeapon <= 0)
						currWeapon = weapons.Count -1;
					else
						currWeapon--;
					weapons[currWeapon].Visible = true;
					weapons[currWeapon].SetProcess(true);
				}
				break;
			case MouseButton.Left:
				if (mouseEvent.Pressed && !attackCooldown)
				{
					isHoldingAttack = true;
					weapons[currWeapon].Attack();
					if (weapons[currWeapon].id == 1)
						YieldForSeconds(0.3f);
				}
				else
				{
					isHoldingAttack = false;
				}
				break;
		}
		if (currWeapon==0)
			speed = 700;
		else
			speed = 500;
	}
	if (@event is InputEventKey keyEvent && keyEvent.Pressed)
	{
		if (keyEvent.Keycode == Key.R)
		{
			weapons[currWeapon].Reload();
		}
	}
}

	async void YieldForSeconds(float neededTime) //there is no YieldForSeconds in godot like in Unity
	{
		attackCooldown = true;
		await ToSignal(GetTree().CreateTimer(neededTime), "timeout");
		attackCooldown = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(isHoldingAttack && weapons[currWeapon].id == 2 && !attackCooldown)
		{
			YieldForSeconds(0.1f);
			weapons[currWeapon].Attack();
		}
		healthUI.Value = health;
		moneyUI.Text = "Money: " + money;
		
		if (health <= 0)
			death();
		
		base._PhysicsProcess(delta);

		handleInput();

		Velocity = currentVelocity * speed;
		MoveAndSlide();
		
		KinematicCollision2D collision = GetLastSlideCollision();
		if (collision != null)
		{
			Node2D contact = collision.GetCollider() as Node2D;
			if (contact.IsInGroup("Enemy"))
			{
				health--;
			}
		}
	}
	
	public void _on_health_butt_pressed()
	{
		if (money >= 5 && health <= 95)
		{
			money -= 5;
			health += 5;
		}
	}
	
	public void _on_gun_butt_pressed()
	{
		if (money >= 10)
		{
			Weapon pistol = this.GetNode("Hand/Pistol") as Weapon ?? null;
			money -= 10;
			if (pistol is null)
			{
				var shops = GetTree().GetNodesInGroup("Shop");
				var shop = shops[0] as Shop;
				shop.GiveGun(this);
				pistol = this.GetNode("Hand/Pistol") as Weapon;
				pistol.ammo += 20;
				pistol.curMagAmmo += 10;
			}
			else
			{
				pistol.ammo += 30;
			}
		}
	}
	
	public void _on_rifle_butt_pressed()
	{
		if (money >= 30)
		{
			Weapon rifle = this.GetNode("Hand/Rifle") as Weapon ?? null;
			money -= 30;
			if (rifle is null)
			{
				var shops = GetTree().GetNodesInGroup("Shop");
				var shop = shops[0] as Shop;
				shop.GiveRifle(this);
				rifle = this.GetNode("Hand/Rifle") as Weapon;
				rifle.ammo += 30;
				rifle.curMagAmmo += 30;
			}
			else
			{
				rifle.ammo += 30;
			}
		}
	}

	public void handleInput()
	{
		currentVelocity = Input.GetVector("LEFT","RIGHT", "UP", "DOWN");
	}
	
	private void death()
	{
		var leftEnemies = GetTree().GetNodesInGroup("Enemy");
		
		foreach (var e in leftEnemies)
		{
			e.QueueFree();
		}
		
		double time = (GetTree().Root.GetNode("MasterNode/Spawner/RoundTimer") as Timer).TimeLeft;
		GetTree().Root.GetNode("MasterNode/Spawner").QueueFree();
		
		DeathScreen.Visible = true;
		
		FlavorText flavor = new FlavorText((int)time, money);
		FlavorDeterminer determiner = new FlavorDeterminer((int)time, money);
		
		(DeathScreen.GetNode("Panel/UniqueText") as Label).Text = flavor.ToString(determiner.determineFlavor(), null);
		QueueFree();
	}
}
