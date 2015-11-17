using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class ItemDatabase : MonoSingleton<ItemDatabase>
{
	private List<Item> itemDatabase = new List<Item>();
	private JsonData itemData;

	private void OnEnable()
	{
		ConstructItemDatabase("Items.json");
	}

	public Item FetchItemByID(int id)
	{
		for (int i = 0; i < itemData.Count; i++) {
			if (itemDatabase[i].Id == id) {
				return itemDatabase[i];
			}
		}
		return null;
	}

	public int Count()
	{
		return itemDatabase.Count;
	}

	// TODO I feel the flow is somehow wrong with this one
	private void ConstructItemDatabase(string fileName)
	{
		itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json")); // StreamingAssets are not to be compiled into the game
		for (int i = 0; i < itemData.Count; i++) {
			Dictionary<string, int> comboDict = new Dictionary<string, int>();
			comboDict.Add("combineId1", (int)itemData[i]["combination"]["id1"]);
			comboDict.Add("combineId2", (int)itemData[i]["combination"]["id2"]);
			comboDict.Add("combineId3", (int)itemData[i]["combination"]["id3"]);
			comboDict.Add("combineResult", (int)itemData[i]["combination"]["idResult"]);
			itemDatabase.Add(new Item((int)itemData[i]["id"], 
			                          itemData[i]["title"].ToString(),
			                          itemData[i]["tag"].ToString(),
			                          itemData[i]["description"].ToString(),
			                          (bool)itemData[i]["stackable"],
			                          (bool)itemData[i]["inspectable"],
			                          (int)itemData[i]["value"],
			                          (bool)itemData[i]["combineable"],
			                          comboDict,
			                          itemData[i]["icon"].ToString()));
		}
	}
}

