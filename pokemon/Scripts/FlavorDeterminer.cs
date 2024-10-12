using Godot;

public class FlavorDeterminer
{
	private int time;
	private int money;
	
	public FlavorDeterminer(int t, int m)
	{
		this.time = t;
		this.money = m;
	}
	
	~FlavorDeterminer()
	{
		GD.Print("Good Bye");
	}
	
	public string determineFlavor()
	{
		if ((time|money) == 0)
			return "B";
		if (money>0 && time==0)
			return "M";
		if (time>0 && money > 0)
			return "T";
		if (time>0 && money == 0)
			return "S";
		return "K";
	}
}
