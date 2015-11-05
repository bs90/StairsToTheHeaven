using UnityEngine;
using System.Collections;

using LgOctEngine.CoreClasses;

[System.Serializable]
public class GameData : LgJsonDictionary {
	public string PlayerName { get { return GetValue<string>("PlayerName", ""); } set { SetValue<string>("PlayerName", value); } }
	public double Time { get { return GetValue<double>("Time", 0); } set { SetValue<double>("Time", value); } }

	public string SceneName { get { return GetValue<string>("SceneName", ""); } set { SetValue<string>("SceneName", value); } }
	public Vector3 Position { get { return GetValue<Vector3>("Position", Vector3.zero); } set { SetValue<Vector3>("Position", value); } }

	public int InvSlot1 { get { return GetValue<int>("InvSlot1", 0); } set { SetValue<int>("InvSlot1", value); } }
	public int InvSlot2 { get { return GetValue<int>("InvSlot2", 0); } set { SetValue<int>("InvSlot2", value); } }
	public int InvSlot3 { get { return GetValue<int>("InvSlot3", 0); } set { SetValue<int>("InvSlot3", value); } }
	public int InvSlot4 { get { return GetValue<int>("InvSlot4", 0); } set { SetValue<int>("InvSlot4", value); } }
	public int InvSlot5 { get { return GetValue<int>("InvSlot5", 0); } set { SetValue<int>("InvSlot5", value); } }
	public int InvSlot6 { get { return GetValue<int>("InvSlot6", 0); } set { SetValue<int>("InvSlot6", value); } }
	public int InvSlot7 { get { return GetValue<int>("InvSlot7", 0); } set { SetValue<int>("InvSlot7", value); } }
	public int InvSlot8 { get { return GetValue<int>("InvSlot8", 0); } set { SetValue<int>("InvSlot8", value); } }
	public int InvSlot9 { get { return GetValue<int>("InvSlot9", 0); } set { SetValue<int>("InvSlot9", value); } }
	public int InvSlot10 { get { return GetValue<int>("InvSlot10", 0); } set { SetValue<int>("InvSlot10", value); } }
	public int InvSlot11 { get { return GetValue<int>("InvSlot11", 0); } set { SetValue<int>("InvSlot11", value); } }
	public int InvSlot12 { get { return GetValue<int>("InvSlot12", 0); } set { SetValue<int>("InvSlot12", value); } }
	public int InvSlot13 { get { return GetValue<int>("InvSlot13", 0); } set { SetValue<int>("InvSlot13", value); } }
	public int InvSlot14 { get { return GetValue<int>("InvSlot14", 0); } set { SetValue<int>("InvSlot14", value); } }
	public int InvSlot15 { get { return GetValue<int>("InvSlot15", 0); } set { SetValue<int>("InvSlot15", value); } }
	public int InvSlot16 { get { return GetValue<int>("InvSlot16", 0); } set { SetValue<int>("InvSlot16", value); } }

}
