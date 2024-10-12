using Godot;
using System;

public partial class MainSceneController : Node
{
	public void _on_exit_pressed()
	{
		GetTree().Quit();
	}
	
	public void _on_skip_pressed()
	{
		(GetNode("Spawner/BuyTimeTimer") as Timer).Start(1);
	}
	
	public void _on_teleport_pressed()
	{
		(GetTree().GetNodesInGroup("Player")[0] as Player_Controller).Position = new Vector2(0, 0);
	}
}
