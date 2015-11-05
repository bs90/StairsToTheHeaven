using UnityEngine;
using System.Collections;
using DG.Tweening;

public class InterfaceManager : MonoSingleton<InterfaceManager> {

	public Transform inventoryPanel;
	public Transform lockPanel;
	public Transform optionPanel;

	private bool inventoryShowing;
	private bool lockShowing;
	private bool optionShowing;

	public float originalLockPosition;
	public float activeLockPosition;

	public float originalInventoryPosition;
	public float activeInventoryPosition;

	public void ToggleLockWindow ()
	{
		RectTransform lockRectTransform = lockPanel.gameObject.GetComponent<RectTransform>();
		if (lockShowing) {
			lockRectTransform.DOAnchorPos(new Vector2(originalLockPosition, 0), 0.5f, false).OnComplete(EndLockToggle);
			lockShowing = false;
			//TODO Remove this
			Elevator.Instance.OpenDoors();
		}
		else {
			if (inventoryShowing) {
				ToggleInventoryWindow();
			}
			lockRectTransform.DOAnchorPos(new Vector2(activeLockPosition, 0), 0.5f, false).OnComplete(EndLockToggle);
			lockShowing = true;

		}
	}

	public void EndLockToggle ()
	{

	}

	public void ToggleInventoryWindow ()
	{
		RectTransform inventoryRectTransform = inventoryPanel.gameObject.GetComponent<RectTransform>();
		if (inventoryShowing) {
			inventoryRectTransform.DOAnchorPos(new Vector2(originalInventoryPosition, 0), 0.5f, false).OnComplete(EndInventoryToggle);
			inventoryShowing = false;
		}
		else {
			if (lockShowing) {
				ToggleLockWindow();
			}
			inventoryRectTransform.DOAnchorPos(new Vector2(activeInventoryPosition, 0), 0.5f, false).OnComplete(EndInventoryToggle);
			inventoryShowing = true;
		}
	}

	public void EndInventoryToggle ()
	{

	}

	void Update ()
	{
		if (Input.GetKeyDown("a")) {
			ToggleLockWindow();
		}
		if (Input.GetKeyDown("s")) {
			ToggleInventoryWindow();
		}
	}
}
