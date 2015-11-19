using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

public class Slot : MonoBehaviour, IDropHandler {
	public int id;
	public bool equipSlot;

	public void OnDrop(PointerEventData eventData)
	{
		if (InventoryManager.Instance.InspectMode) {
			return;
		}
		//OnDrop finishes before End Drag
		ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
		if (droppedItem == null) {
			return;
		}

		if (InventoryManager.Instance.items[id].Id == -1) {
			InventoryManager.Instance.MoveItem(droppedItem, id);
		}
		else if (droppedItem.slot != id) {
			//TODO This is kinda stupid... kinda
			if(this.transform.childCount > 0) {
				Transform swapItem = this.transform.GetChild(0);
				ItemData swapItemData = swapItem.GetComponent<ItemData>();

				swapItemData.slot = droppedItem.slot;
				InventoryManager.Instance.SwapItem(droppedItem, swapItem.GetComponent<ItemData>(), id);

				swapItem.transform.SetParent(InventoryManager.Instance.slots[droppedItem.slot].transform);
				swapItem.transform.position = InventoryManager.Instance.slots[droppedItem.slot].transform.position;

				droppedItem.slot = id;
				droppedItem.transform.SetParent(this.transform);
				droppedItem.transform.position = this.transform.position;

				if (droppedItem.item.Combineable && droppedItem.item.CombineId1 == swapItemData.item.Id && !equipSlot) {
					if (droppedItem.item.CombineResult == swapItemData.item.CombineResult) {
						InventoryManager.Instance.CombineItem(droppedItem.item.Id, swapItemData.item.Id, droppedItem.item.CombineResult);					
					}
				}
			}
		}
	}
}
