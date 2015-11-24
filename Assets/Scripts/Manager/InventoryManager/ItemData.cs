using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
	[HideInInspector] public Item item;
	[HideInInspector] public int amount;
	[HideInInspector] public int slot;

	private Vector2 offset;

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (item != null && !InventoryManager.Instance.InspectMode && GameManager.Instance.State == GameState.Inventory) {
			this.transform.SetParent(this.transform.parent.parent);
			GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		if (item != null) {
			//TODO Those states are getting really messy
			//TODO Too messy, but I can't redo them now
			if (InventoryManager.Instance.InspectMode && GameManager.Instance.State != GameState.Confirmation) {
				InterfaceManager.Instance.DisplayInvestMode(item);
			}
			else if (GameManager.Instance.State == GameState.Inventory) {
				offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
			}
		}
		else {
			Debug.Log("LOL");
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (item != null && !InventoryManager.Instance.InspectMode && GameManager.Instance.State == GameState.Inventory) {
			this.transform.position = eventData.position - offset;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (item != null && !InventoryManager.Instance.InspectMode && GameManager.Instance.State == GameState.Inventory) {
			this.transform.SetParent(InventoryManager.Instance.slots[slot].transform);
			this.transform.position = InventoryManager.Instance.slots[slot].transform.position;
			GetComponent<CanvasGroup>().blocksRaycasts = true;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Tooltip.Instance.Activate(item);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Tooltip.Instance.Deactivate();
	}
}
