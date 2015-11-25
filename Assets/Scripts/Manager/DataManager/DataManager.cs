using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class DataManager : MonoSingleton<DataManager> 
{
	private List<Pattern> patternDatabase = new List<Pattern>(); //accessor doesn't work here, why?
	private JsonData patternData;

	private void OnEnable()
	{
		ConstructPatternDatabase("Patterns.json");
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

	public int PatternCount()
	{
		return  patternData.Count;
	}

	private void ConstructPatternDatabase(string fileName)
	{
		patternData = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/" + fileName));
		for (int i = 0; i < patternData.Count; i++) {
			List<Vector2> patternCoors = new List<Vector2>();
			for (int p = 0; p < patternData[i]["pattern"].Count; p++) {
				patternCoors.Add(new Vector2((int)patternData[i]["pattern"][p]["x"], (int)patternData[i]["pattern"][p]["y"]));
			}
			patternDatabase.Add(new Pattern((int)patternData[i]["id"], patternCoors, (int)patternData[i]["requires"], patternData[i]["floor"].ToString()));
		}
	}
}
