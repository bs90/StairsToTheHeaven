using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

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
				gameData[0]["chests"][i] = openState;
				return;
			}
		}
	}

	public void SavePickUpState (int id, bool openState)
	{
		for (int i = 0; i < gameData[0]["pickUps"].Count; i++) {
			if (i == id) {
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
				gameData[0]["events"][i] = eventState;
				return;
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
//			Debug.Log ((bool)gameData[0]["chests"][i]);
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
}
