using System.Collections;
using System.Collections.Generic;

public class Panel {
	public Dictionary<string, bool> Colors = new Dictionary<string, bool>();

	public int Floor;

	public Panel(int floor)
	{
		this.Floor = floor;
	}
}
