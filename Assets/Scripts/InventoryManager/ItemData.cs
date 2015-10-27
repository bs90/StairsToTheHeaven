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
		//TODO Something is wrong here, the Item is off, I don't have time to fix it yet
		if (item != null) {
			this.transform.SetParent(this.transform.parent.parent);
//			this.transform.position = eventData.position;
			GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
	}
	
	public void OnPointerDown(PointerEventData eventData)
	{
		if (item != null) {
			offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
		}
		else {
			Debug.Log("LOL");
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (item != null) {
			this.transform.position = eventData.position - offset;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (item != null) {
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
