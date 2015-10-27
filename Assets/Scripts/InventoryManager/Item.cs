using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item
{
	public int Id { get; set; }
	public string Title { get; set; }
	public string Tag { get; set; }
	public string Description { get; set; }
	
	public bool Stackable { get; set; }
	public int Value { get; set; }
	
	public bool Equipable { get; set; }
	public int HpExtension { get; set; }
	public int SpExtension { get; set; }
	public int Attack { get; set; }
	public int Defense { get; set; }
	public int Protection { get; set; }
	
	public bool Useable { get; set; }
	public int HpRecovery { get; set; }
	public int SpRecovery { get; set; }
	public int StatusCure { get; set; }
	
	public bool Combineable { get; set; }
	public int CombineId1 { get; set; }
	public int CombineId2 { get; set; }
	public int CombineId3 { get; set; }
	public int CombineResult { get; set; }

	public Sprite Icon { get; set; } 

	public Item(int id, string title, string tag, string desc,
	            bool stackable, int value,
	            bool combineable, Dictionary<string, int> combines, string icon)
	{
		this.Id = id;
		this.Title = title;
		this.Description = desc;
		
		this.Stackable = stackable;
		this.Value = value;
		
		this.Combineable = combineable;
		
		// TODO Shorten this please?
		int combId1;
		combines.TryGetValue("combineId1", out combId1);
		this.CombineId1 = combId1;
		
		int combId2;
		combines.TryGetValue("combineId2", out combId2);
		this.CombineId2 = combId2;
		
		int combId3;
		combines.TryGetValue("combineId3", out combId3);
		this.CombineId3 = combId3;
		
		int combResult;
		combines.TryGetValue("combineResult", out combResult);
		this.CombineResult = combResult;

		this.Icon = Resources.Load<Sprite>("Sprites/Icons/" + icon);
	}
	
	public Item(int id, string title, string tag, string desc, 
	            bool stackable, int value,
	            bool equiable, Dictionary<string, int> stats,
	            bool useable, Dictionary<string, int> properties,
	            bool combineable, Dictionary<string, int> combines,
	            string icon)
	{
		
	}
	
	public Item()
	{
		this.Id = -1;
		this.Title = "Empty Slot";
		this.Tag = "null";
		this.Description = "If you are a dev, you must have messed something seriously up, otherwise, happy cheating.";
		
		this.Stackable = false;
		this.Value = 0;
	}
}
