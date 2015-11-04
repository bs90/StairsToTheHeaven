using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameState {
	MainMenu,
	Adventure,
	Invenstigation,
	Inventory,
	Option,
	Credits
}

public class GameManager : MonoSingleton<GameManager> {
	public string info;
	private string presentFloor;

	public GameObject infoText;

	[HideInInspector]public int minutes = 0;
	[HideInInspector]public int hour = 0;
	[HideInInspector]public int second = 0;

	void Start () 
	{
		UpdateFloor(2);
		ClockSetup();
		InvokeRepeating("ClockUpdate", 0, 1);
	}
	
	void Update () 
	{
	
	}

	void ClockSetup ()
	{
		hour = System.DateTime.Now.Hour;
		minutes = System.DateTime.Now.Minute;
		second = System.DateTime.Now.Second;
	}

	void ClockUpdate ()
	{
		second++;
		if(second >= 60) {
			second = 0;
			minutes++;
			if(minutes > 60)
			{
				minutes = 0;
				hour++;
				if(hour >= 24)
					hour = 0;
			}
		}

		info = string.Format(presentFloor + " - Time: " + hour + ":" + minutes + ":" + second);
		infoText.GetComponent<Text>().text = info;
	}

	public void UpdateFloor (int floorNumber)
	{
		presentFloor = string.Format("Floor: " + floorNumber);
	}
}
