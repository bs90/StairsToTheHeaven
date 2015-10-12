using UnityEngine;
using System.Collections;

public struct DialogMessage {
	public string 	Character { get; set; }
	public bool 	Skipable { get; set; }
	public bool 	Voiced { get; set; }
	public string 	Text { get; set; }
}