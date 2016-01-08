using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.Linq;

public enum RemotePanelColors {
	Red = 0,
	Green = 1,
	White = 2,
	Purple = 3,
	Blue = 4,
	Orange = 5,
	Yellow = 6,
	None = 7
}

public class DataManager : MonoSingleton<DataManager> 
{
	private JsonData patternData;
	private JsonData gameData;

	private List<Pattern> patternDatabase = new List<Pattern>(); //accessor doesn't work here, why?

	private List<bool> chestData = new List<bool>();
	public List<bool> ChestData {
		get {
			return this.chestData;
		}
	}

	private List<bool> pickUpData = new List<bool>();
	public List<bool> PickUp {
		get {
			return this.pickUpData;
		}
	}

	private List<ItemSlot> inventoryData = new List<ItemSlot>();
	public List<ItemSlot> InventoryData {
		get {
			return this.inventoryData;
		}
	}

	private List<bool> eventData = new List<bool>();
	public List<bool> EventData {
		get {
			return this.eventData;
		}
	}

	private List<bool> quizData = new List<bool>();
	public List<bool> QuizData {
		get {
			return this.quizData;
		}
	}

	private List<Panel> panelData = new List<Panel>();
	public List<Panel> PanelData {
		get {
			return this.panelData;
		}
	}

	private List<Panel> correctPanelData = new List<Panel>();
	public List<Panel> CorrectPanelData {
		get {
			return this.correctPanelData;
		}
	}

	public int PatternCount {
		get {
			return  this.patternData.Count;
		}
	}

	public int SlotsCount {
		get {
			return this.inventoryData.Count;
		}
	}

	private void Awake()
	{
		if (gameData == null || patternData == null) {
			AssignGameDataFile("GameData.json");
			AssignPatternDataFile("Patterns.json");
			ConstructPatternDatabase();
			ConstructChestData();
			ConstructPickUpData();
			ConstructInventoryData();
			ConstructEventData();
			ConstructQuizData();
			ConstructRemotePanelData();
			ConstructCorrectPanelData();
		}
	}

	public Pattern FetchPatternById(int id)
	{
		for (int i = 0; i < patternData.Count; i++) {
			if (patternDatabase[i].Id == id) {
				return patternDatabase[i];
			}
		}
		return null;
	}

	public bool GetChestState(int id) {
		for (int i = 0; i < chestData.Count; i++) {
			if (id == i) {
				return chestData[i];
			}
		}
		return false;
	}

	public bool GetPickUpState(int id) {
		for (int i = 0; i < pickUpData.Count; i++) {
			if (id == i) {
				return pickUpData[i];
			}
		}
		return false;
	}

	public bool GetPanelComponentState(string color, int floor) {
		if (floor < 0 || floor > panelData.Count) {
			return false;
		}
		Panel panel = panelData[floor];
		if (panel.Colors.ContainsKey(color)) {
			return panel.Colors[color];
		}
		Debug.LogError("The frigging color doesn't exist");
		return false;
	}

	public bool GetEventState(int id) {
		for (int i = 0; i < eventData.Count; i++) {
			if (id == i) {
				return eventData[i];
			}
		}
		return false;
	}

	//TODO The naming is terrible
	public ItemSlot GetItemSlot(int id)
	{
		for (int i = 0; i < inventoryData.Count; i++) {
			if (id == i) {
				return inventoryData[i];
			}
		}
		return null;
	}

	public string GetFloor()
	{
		return (string)gameData[0]["floor"];
	}

	public string GetPlayerName()
	{
		return (string)gameData[0]["playerName"];
	}

	public string GetPlayedTime()
	{
		return (string)gameData[0]["playedTime"];
	}

	public void SaveFloor(string floor) {
		gameData[0]["floor"] = floor;
	}

	public void SavePlayerName(string playerName) {
		gameData[0]["playerName"] = playerName;
	}

	public void SavePlayedTime(string playedTime) {
		gameData[0]["playedTime"] = playedTime;
	}

	public void SaveChestState (int id, bool openState)
	{
		for (int i = 0; i < gameData[0]["chests"].Count; i++) {
			if (i == id) {
				chestData[id] = openState;
				gameData[0]["chests"][i] = openState;
				return;
			}
		}
	}

	public void SavePickUpState (int id, bool openState)
	{
		for (int i = 0; i < gameData[0]["pickUps"].Count; i++) {
			if (i == id) {
				pickUpData[id] = openState;
				gameData[0]["pickUps"][i] = openState;
				return;
			}
		}
	}

	public void SaveInventoryState ()
	{
		InventoryManager inventory = InventoryManager.Instance;
		for (int i = 0; i < gameData[0]["slotItem"].Count; i++) {
			gameData[0]["slotItem"][i] = inventory.GetItemId(i);
			gameData[0]["slotAmount"][i] = inventory.GetAmount(i);
		}
	}

	public void SaveEventData (int id, bool eventState)
	{
		for (int i = 0; i < gameData[0]["events"].Count; i++) {
			if (i == id) {
				eventData[id] = eventState;
				gameData[0]["events"][i] = eventState;
				return;
			}
		}
	}

	public void SavePanelData(List<Panel> panels)
	{
		for (int i = 0; i < gameData[0]["remotePanel"].Count; i++) {
			for (int c = 0; c < panels[i].Colors.Count; c++) {
				string color = panels[i].Colors.Keys.ElementAt(c);
				gameData[0]["remotePanel"][i][color] =  panels[i].Colors[color];
			}
		}
	}

	public void SavePanelColorData(List<RemotePanelColors> colors)
	{
		for (int i = 0; i < gameData[0]["remotePanel"].Count; i++) {
			for (int c = 0; c < panelData[i].Colors.Count; c++) {
				for (int d = 0; d < colors.Count; d++) {
					if (panelData[i].Colors.Keys.ElementAt(c) == colors[d].ToString()) {
						bool presentState = panelData[i].Colors[colors[d].ToString()];
						panelData[i].Colors[colors[d].ToString()] = !presentState;
						gameData[0]["remotePanel"][i][colors[d].ToString()] = panelData[i].Colors[colors[d].ToString()];
						Debug.Log ("Panel " + i + "'s color" + colors[d].ToString() + " has been changed to " + panelData[i].Colors[colors[d].ToString()]);
						break;
					}
				}
			}
		}
	}

	public void WriteGameData (string fileName)
	{
		SaveInventoryState();
		System.Text.StringBuilder builder = new System.Text.StringBuilder();
		JsonWriter writer = new JsonWriter(builder);
		writer.PrettyPrint = true;
		writer.IndentValue = 2;
		JsonMapper.ToJson(gameData, writer);
		
		File.WriteAllText(Application.streamingAssetsPath + "/" + fileName, builder.ToString());
	}

	private void AssignGameDataFile(string fileName)
	{
		gameData = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/" + fileName));
	}
	
	private void AssignPatternDataFile(string fileName)
	{
		patternData = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/" + fileName));
	}

	private void ConstructPatternDatabase()
	{
		for (int i = 0; i < patternData.Count; i++) {
			List<Vector2> patternCoors = new List<Vector2>();
			for (int p = 0; p < patternData[i]["pattern"].Count; p++) {
				patternCoors.Add(new Vector2((int)patternData[i]["pattern"][p]["x"], (int)patternData[i]["pattern"][p]["y"]));
			}
			patternDatabase.Add(new Pattern((int)patternData[i]["id"], patternCoors, (int)patternData[i]["requires"], patternData[i]["floor"].ToString()));
		}
	}

	private void ConstructChestData()
	{
		for (int i = 0; i < gameData[0]["chests"].Count; i++) {
//			Debug.Log (i " open state is " + (bool)gameData[0]["chests"][i]);
			chestData.Add((bool)gameData[0]["chests"][i]);
		}
	}

	private void ConstructPickUpData()
	{
		for (int i = 0; i < gameData[0]["pickUps"].Count; i++) {
			pickUpData.Add((bool)gameData[0]["pickUps"][i]);
		}
	}

	private void ConstructInventoryData()
	{
		for (int i = 0; i < gameData[0]["slotItem"].Count; i++) {
			int id = (int)gameData[0]["slotItem"][i];
			int amount = 0;
			if (id != -1) {
				amount = (int)gameData[0]["slotAmount"][i];
			}
			inventoryData.Add(new ItemSlot(i, id, amount));
		}
	}

	private void ConstructEventData()
	{
		for (int i = 0; i < gameData[0]["events"].Count; i++) {
			eventData.Add((bool)gameData[0]["events"][i]);
		}
	}

	private void ConstructQuizData()
	{
		for (int i = 0; i < gameData[0]["quiz"].Count; i++) {
			quizData.Add((bool)gameData[0]["quiz"][i]);
		}
	}

	private void ConstructRemotePanelData()
	{
		for (int i = 0; i < gameData[0]["remotePanel"].Count; i++) {
			Panel newPanel = new Panel(i);
			int colorCount = gameData[0]["remotePanel"][i].Count;
//			Debug.Log ("panel " + i + " count: " + colorCount);
			for (int c = 0; c < colorCount; c++) {
				RemotePanelColors enumDisplayStatus = (RemotePanelColors)c;
				string color = enumDisplayStatus.ToString();
//				Debug.Log ("color " + color + " at panel " + i + " is lit: " + gameData[0]["remotePanel"][i][color]);
				newPanel.Colors[color] = (bool)gameData[0]["remotePanel"][i][color];
			}
			panelData.Add(newPanel);
		}
	}

	private void ConstructCorrectPanelData()
	{
		for (int i = 0; i < gameData[0]["correctOrder"].Count; i++) {
			Panel newPanel = new Panel(i);
			int colorCount = gameData[0]["correctOrder"][i].Count;
			for (int c = 0; c < colorCount; c++) {
				RemotePanelColors enumDisplayStatus = (RemotePanelColors)c;
				string color = enumDisplayStatus.ToString();
				newPanel.Colors[color] = (bool)gameData[0]["correctOrder"][i][color];
			}
			correctPanelData.Add(newPanel);
		}
	}

	public bool ComparePanelData()
	{
		for (int i = 0; i < panelData.Count; i++) {
			Panel panel = panelData[0];
			Panel correctPanel = correctPanelData[0];
			int colorCount = panel.Colors.Count;
			for (int c = 0; c < colorCount; c++) {
				RemotePanelColors enumDisplayStatus = (RemotePanelColors)c;
				string color = enumDisplayStatus.ToString();
				if (panel.Colors[color] != correctPanel.Colors[color]) {
					return false;
				}
			}
		}
		return true;
	}
}
