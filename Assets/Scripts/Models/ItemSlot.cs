using UnityEngine;
using System.Collections;

public class ItemSlot {

	public int Id { get; set; }
	public int Item { get; set; }
	public int Amount { get; set; }

	public ItemSlot(int id, int item, int amount)
	{
		this.Id = id;
		this.Item = item;
		this.Amount = amount;
	}
}
