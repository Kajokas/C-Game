using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node
{
	private RandomNumberGenerator random = new RandomNumberGenerator();
	
	private List<PackedScene> enemies = new List<PackedScene>();
	
	private Timer timer;
	private Timer buyTimer;
	
	private Label roundUI;
	
	private Node2D spawnedShop;
	
	private bool buyingPhase = true;
	private int curRound = 0;
	
	private PackedScene shopBuilding = GD.Load<PackedScene>("res://Scenes/building.tscn");
	
	private PackedScene Zombie = GD.Load<PackedScene>("res://Scenes/Enemies/Zombie.tscn");
	private PackedScene Runner = GD.Load<PackedScene>("res://Scenes/Enemies/Runner.tscn");
	
	public override void _Ready()
	{
		timer = GetNode("RoundTimer") as Timer;
		buyTimer = GetNode("BuyTimeTimer") as Timer;
		
		roundUI = (GetTree().GetNodesInGroup("Player"))[0].GetNode("Camera2D/Canvas/Control/RoundUI") as Label;
		
		//GD.Print(timer.Name);
		
		enemies.Add(Zombie);
		enemies.Add(Runner);
		
		buyTimer.Start(30);
	}
	
	public override void _Process(double delta)
	{
		switch (buyingPhase)
		{
			case true:
				roundUI.Text = "Shop phase " + ": " + (int)buyTimer.TimeLeft + "s";
				break;
			case false:
				roundUI.Text = "Round " + curRound + ": " + (int)timer.TimeLeft + "s";
				break;
		}
	}
	
	
	
	private void _on_round_timer_timeout()
	{
		//buyTimer.Stop();
		buyingPhase = true;
		spawnedShop = shopBuilding.Instantiate() as Node2D;
		GetTree().Root.GetNode("MasterNode").AddChild(spawnedShop);
		(GetTree().Root.GetNode("MasterNode/ExtraPlayersUI/ShopPhase") as Control).Visible = true;
		GD.Print("Done");
		buyTimer.Start(30);
		timer.Stop();
	}
	
	private void _on_buy_time_timer_timeout()
	{
		//timer.Stop();
		buyingPhase = false;
		GetTree().Root.GetNode("MasterNode/Building").QueueFree();
		(GetTree().Root.GetNode("MasterNode/ExtraPlayersUI/ShopPhase") as Control).Visible = false;
		curRound++;
		newRound(curRound);
		timer.Start(60);
		buyTimer.Stop();
	}
	
	private void newRound(int round)
	{
		var spawnedEnemies = GetTree().GetNodesInGroup("Enemy");
		for(int i = 0; i<10*round; i++)
			Spawn();
		if (spawnedEnemies.Count > 200)
		{
			var toKillEnemies = spawnedEnemies[0..(spawnedEnemies.Count/2)];
			foreach (var e in toKillEnemies)
				e.QueueFree();
		}
	}
	
	private void Spawn()
	{
		random.Randomize();
		int toSpawn = random.RandiRange(0,1);
		
		var newEnemy = enemies[toSpawn].Instantiate() as Enemy;
		
		newEnemy.Position = RandVector(); 
		
		var map = this.GetTree().Root.GetChild(0);
		
		CallDeferred(nameof(DeferredAddChild), map, newEnemy);
	}
	
	private void DeferredAddChild(Node parent, Node child)
	{
		parent.AddChild(child);
	}
	
	public Vector2 RandVector()
	{
		random.Randomize();
		int xCoord = random.RandiRange(-10000,10000) * (1+curRound/2);
		int yCoord = random.RandiRange(-10000,10000) * (1+curRound/2);
		
		return new Vector2(xCoord, yCoord);
	}
}
