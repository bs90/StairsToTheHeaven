using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ItemPicker : MonoBehaviour {
	public bool pickedUp;
	public int itemId;

	private void Awake()
	{
		StartCoroutine(SetState());
	}

	private IEnumerator SetState()
	{
		yield return new WaitForSeconds(1);
		pickedUp = DataManager.Instance.GetPickUpState(itemId);
//		Debug.Log ("Pickup " + itemId + " is picked: " + pickedUp);
		if (pickedUp) {
			Destroy(this.gameObject);
		}
	}

	public void PickUp()
	{
		InterfaceManager.Instance.ToggleInfoWindow ("You picked up item " + ItemDatabase.Instance.FetchItemByID (itemId).Title + ".", ItemPickUp);
	}

	private void ItemPickUp()
	{
		InventoryManager.Instance.AddItem (itemId, 1);
		DataManager.Instance.SavePickUpState(itemId, true);
		this.gameObject.SetActive(false);
	}
}
