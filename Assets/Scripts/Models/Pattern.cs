using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pattern {
	public int Id { get; set; }
	public List<Vector2> PatternCombination { get; set; }
	public int RequiredItemId { get; set; }
	public string Floor { get; set; }

	public Pattern(int id, List<Vector2> patternCombination, int requiredItemId, string floor) {
		this.Id = id;
		this.PatternCombination = patternCombination;
		this.RequiredItemId = requiredItemId;
		this.Floor = floor;
	}
}
