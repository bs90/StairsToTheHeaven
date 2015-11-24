using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum GameState {
	MainMenu,
	Adventure,
	Investigation,
	Inventory,
	Inspection,
	OpenLock,
	Password,
	Pause,
	Option,
	Choice,
	Confirmation,
	Uncontrolable,
	ActionEvent,
	Debugging,
	Credits
}

public class GameManager : MonoSingleton<GameManager> {
	[HideInInspector]public string info;

	private string presentFloor;

	public GameObject infoText;
	public GameObject modeText;

	[HideInInspector]public int minutes = 0;
	[HideInInspector]public int hour = 0;
	[HideInInspector]public int second = 0;
	
	[HideInInspector]public GameObject standingPoint;
	//TODO Public only for debug purpose
	public GameState gameState;
	public GameState State {
		get {
			return this.gameState;
		}
	}

	public bool debugMode;

	void Start () 
	{
		UpdateFloor(2);
		ClockSetup();
		InvokeRepeating("ClockUpdate", 0, 1);
		SetGameState(GameState.Investigation);
	}
	
	void Update () 
	{
		modeText.GetComponent<Text>().text = "State: " + gameState.ToString();
	}

	public void SetGameState (GameState state)
	{
		gameState = state;
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

	public void MoveToFloor (int floorNumber)
	{

	}
}
