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
	public string PresentFloor {
		get {
			return this.presentFloor;
		}
	}

	public bool isInvestigationState {
		get {
			if (gameState == GameState.Investigation) {
				return true;
			}
			else {
				return false;
			}
		}
	}

	public bool isUncontrolableState {
		get {
			if (gameState == GameState.Uncontrolable) {
				return true;
			}
			else {
				return false;
			}
		}
	}

	public GameObject infoText;
	public GameObject modeText;

	public GameObject LoadingScene;
	public Image LoadingBar;

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

	void Awake ()
	{
		SetGameState(GameState.Investigation);
		Elevator.Instance.ToggleDoors(null);
	}

	void Start () 
	{
		ClockSetup();
		InvokeRepeating("ClockUpdate", 0, 1);
	}
	
	void Update () 
	{
		modeText.GetComponent<Text>().text = "State: " + gameState.ToString();

		if (Input.GetButtonDown("Fire1") && InterfaceManager.Instance.InfoShowing && State == GameState.Confirmation) {
			InterfaceManager.Instance.ToggleInfoWindow(string.Empty, InterfaceManager.Instance.onInfoToggleCallback);
		}
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

	public void UpdateFloor (string floor)
	{
		presentFloor = floor;
		DataManager.Instance.SaveFloor(floor);
	}

	public void MoveToFloor (int floorNumber)
	{

	}

	public void LoadScene(string sceneName)
	{
		StartCoroutine(SceneCoroutine(sceneName));
	}
	
	private IEnumerator SceneCoroutine(string sceneName)
	{
		LoadingScene.SetActive(true);
		DataManager.Instance.WriteGameData("GameData.json");
		AsyncOperation async = Application.LoadLevelAsync(sceneName);
		while (!async.isDone) {
			LoadingBar.fillAmount = async.progress / 0.9f;
			yield return null;
		}
		LoadingScene.SetActive(false);

	}

	private IEnumerator SaveGame(string saveName)
	{

		yield return null;
	}
}
