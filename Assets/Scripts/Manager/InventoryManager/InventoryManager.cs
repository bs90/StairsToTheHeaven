using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//[RequireComponent (typeof (InventoryData))]
public class InventoryManager : MonoSingleton<InventoryManager> {
	public GameObject inventoryPanel;
	public GameObject slotPanel;
	public GameObject slotScrollRect;
	public GameObject inventorySlot;
	public GameObject inventoryItem;

	public GameObject equipSlot;

//	public GameObject equipmentPanel;
//	public GameObject equipmentSlotPanel;
//	public GameObject equipmentSlotScrollRect;
//	public GameObject equipmentSlot;
//	public GameObject equipmentItem;

	public int inventorySlotAmount;
//	public int equipmentSlotAmount;

	public List<Item> items = new List<Item>();
	public List<GameObject> slots = new List<GameObject>();

//	public List<GameObject> equips = new List<GameObject>();

	private float edgeOffset = 5;

	void Start()
	{
		SetupComponents();
		SetupPanelSlots();
	}

	public void SetupComponents()
	{

		if (inventoryPanel == null) {
			inventoryPanel = GameObject.Find("InventoryPanel");
		}
		if (slotPanel == null) {
			slotPanel = inventoryPanel.transform.FindChild("SlotPanel").gameObject;
		}
		if (slotScrollRect == null) {
			slotScrollRect = inventoryPanel.transform.FindChild("SlotRect").gameObject;
		}
	}

	public void SetupPanelSlots()
	{
		if (items.Count != 0) {
			return;
		}

		//TODO This add items depend on slot amount, maybe it should just add available in inv items
		for (int i = 0; i < inventorySlotAmount; i++) {
			items.Add(new Item());

			GameObject slotObj = Instantiate(inventorySlot);
			slotObj.name = string.Format("Slot" + i);
			slots.Add(slotObj);
			slots[i].GetComponent<Slot>().id = i;
			slots[i].transform.SetParent(slotScrollRect.transform);	
		}

		if (equipSlot != null) {
			slots.Add(equipSlot);
			items.Add(new Item());
			equipSlot.GetComponent<Slot>().id = slots.Count - 1;
		}

		AddItem(0, 1);
		AddItem(1, 1);
		AddItem(2, 1);

	}

	public void AddItem(int id, int amount)
	{
		Item itemToAdd = ItemDatabase.Instance.FetchItemByID(id);
		if (itemToAdd == null) {
			return;
		}

		if (itemToAdd.Stackable && IsItemInInventory(itemToAdd)) {
			for (int i = 0; i < items.Count; i ++) {
				if (items[i].Id == id) {
					ItemData data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
					data.amount += amount;
					data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
					break;
				}
			}
		}
		else {
			for (int i = 0; i < items.Count; i++) {
				if(items[i].Id == -1) {
					items[i] = itemToAdd;
					slots[i].GetComponent<Image>().color = new Color32(64, 132, 242, 100);
					
					GameObject itemObj = Instantiate(inventoryItem);
					ItemData itemData = itemObj.GetComponent<ItemData>();
					itemData.item = itemToAdd;
					itemData.amount = amount;
					itemData.slot = i;

					itemObj.transform.SetParent(slots[i].transform);
					itemObj.transform.position = Vector3.zero;
					itemObj.transform.GetChild(0).GetComponent<Text>().text = itemData.amount.ToString();

					itemObj.GetComponent<Image>().sprite = itemToAdd.Icon;
					itemObj.name = itemToAdd.Title;

					if (itemObj.GetComponent<RectTransform>().offsetMin != new Vector2(edgeOffset, edgeOffset)) {
						itemObj.GetComponent<RectTransform>().offsetMax = new Vector2(-edgeOffset, -edgeOffset);
						itemObj.GetComponent<RectTransform>().offsetMin = new Vector2(edgeOffset, edgeOffset);
					}
					break;
				}
			}
		}
	}

	public void RemoveItem(int id, int amount)
	{
		Item itemToRemove = ItemDatabase.Instance.FetchItemByID(id);
		if (IsItemInInventory(itemToRemove)) {
			for (int i = 0; i < items.Count; i ++) {
				if (items[i].Id == id) {
					ItemData data = null;
					if (slots[i].transform.childCount == 0) {
						data = GameObject.Find(items[id].Title).GetComponent<ItemData>();
					}
					else {
						data = slots[i].transform.GetChild(0).GetComponent<ItemData>();
					}

					if (amount < data.amount) {
						data.amount -= amount;
						data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();
					}
					else if (amount >= data.amount) {
						slots[i].GetComponent<Image>().color = new Color32(103, 115, 131, 39);
						Destroy(slots[i].transform.GetChild(0).gameObject);
						items[i] = new Item();
					}
					else {
						//TODO I need a tester...
						Debug.Log("What the hell else can happen?");
					}
					break;
				}
			}
		}
		else {
			return;
		}
	}

	public void MoveItem(ItemData itemData, int slotId)
	{
		items[itemData.slot] = new Item();
		slots[itemData.slot].GetComponent<Image>().color = new Color32(103, 115, 131, 39);

		items[slotId] = itemData.item;
		slots[slotId].GetComponent<Image>().color = new Color32(64, 132, 242, 100);

		itemData.slot = slotId;
	}

	public void SwapItem(ItemData swapperItemData, ItemData swappeeItemData, int slotId) //LOL at the naming
	{
		items[swapperItemData.slot] = swappeeItemData.transform.GetComponent<ItemData>().item;
		items[slotId] = swapperItemData.item;
	}

	public void CombineItem(int combinerId, int combineeId, int resultId)
	{
//		GetItemAmount(ItemDatabase.Instance.FetchItemByID(combinerId));
//		GetItemAmount(ItemDatabase.Instance.FetchItemByID(combineeId));
		RemoveItem(combinerId, 1);
		RemoveItem(combineeId, 1);

		AddItem(resultId, 1);
	}

	public void LoadItem(int id)
	{
		Item itemToAdd = ItemDatabase.Instance.FetchItemByID(id);

		for (int i = 0; i < items.Count; i++) {
			if(items[i].Id == -1) {
				items[i] = itemToAdd;
				GameObject itemObj = Instantiate(inventoryItem);

				itemObj.transform.SetParent(slots[i].transform);
				slots[i].GetComponent<Image>().color = new Color32(64, 132, 242,100);

				itemObj.GetComponent<Image>().sprite = itemToAdd.Icon;
				itemObj.transform.position = Vector2.zero;
				itemObj.name = itemToAdd.Title;

				ItemData data = itemObj.GetComponent<ItemData>();
				data.amount = itemToAdd.Value;
				data.transform.GetChild(0).GetComponent<Text>().text = data.amount.ToString();

				if (!itemToAdd.Stackable && data.amount > 1) {
					data.amount = 1;
				}
				break;
			}
		}
	}

	public bool IsItemInInventory(Item item)
	{
		for (int i = 0; i < items.Count; i++) {
			if (items[i].Id == item.Id) {
				return true;
			}
		}
		return false;
	}

	public int GetItemAmount(Item item)
	{
		if (IsItemInInventory(item)) {
			for (int i = 0; i < items.Count; i ++) {
				if (items[i].Id == item.Id) {
					Debug.Log ("Amount of item " + items[i].Title + " is " + slots[i].transform.GetChild(0).GetComponent<ItemData>().amount);
					return slots[i].transform.GetChild(0).GetComponent<ItemData>().amount;
				}
			}
		}
		return 0;
	}
}
