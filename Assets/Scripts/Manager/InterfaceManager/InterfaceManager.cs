using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class InterfaceManager : MonoSingleton<InterfaceManager> {

	public Transform inventoryPanel;
	public Transform inspectButton;
	public Transform lockPanel;
	public Transform optionPanel;

	public Transform infoPanel;
	public Transform messagePanel;

	public Transform inventoryButton;
	public Transform inspectOffButton;
	public Transform patternButton;

	public Transform inspectItem;

	private bool inventoryShowing;
	public bool InventoryShowing {
		get {
			return this.inventoryShowing;
		}
	}
	private bool lockShowing;
	private bool optionShowing;

	private bool infoShowing;
	private bool messageShowing;

	private bool inspectImageShowing;

	public Vector2 originalLockPosition;
	public Vector2 activeLockPosition;

	public Vector2 originalInventoryPosition;
	public Vector2 activeInventoryPosition;

	public Vector2 minimizedInfoSize;
	public Vector2 maximizedInfoSize;

	[HideInInspector]public TweenCallback onInfoToggleCallback;  

	public void ToggleInfoWindow (string info, TweenCallback onCloseEvent)
	{
		GameManager.Instance.SetGameState(GameState.Uncontrolable);
		infoPanel.gameObject.SetActive(true);
		RectTransform infoRectTransform = infoPanel.gameObject.GetComponent<RectTransform>();
		infoRectTransform.sizeDelta = minimizedInfoSize;
		if (infoShowing) {
			infoPanel.GetComponentInChildren<Text>().text = string.Empty;
			if (onCloseEvent == null) {
				infoRectTransform.DOSizeDelta(minimizedInfoSize, 0.1f, false).OnComplete(()=>EndInfoToggle(null));
			}
			else {
				Sequence closeSequence = DOTween.Sequence();
				closeSequence.Append(infoRectTransform.DOSizeDelta(minimizedInfoSize, 0.1f, false).OnComplete(()=>EndInfoToggle(null)));
				closeSequence.OnComplete(onCloseEvent);
				closeSequence.Play();
				onInfoToggleCallback = null;
			}
			infoShowing = false;
		}
		else {
			if (lockShowing) {
				ToggleLockWindow();
			}
			if (onCloseEvent != null) {
				onInfoToggleCallback = onCloseEvent;
			}
			infoRectTransform.DOSizeDelta(maximizedInfoSize, 0.3f, false).OnComplete(()=>EndInfoToggle(info));
			infoShowing = true;
		}
	}

	void EndInfoToggle (string info)
	{
		if (infoShowing) {
			infoPanel.GetComponentInChildren<Text>().text = info;
			GameManager.Instance.SetGameState(GameState.Confirmation);
		}
		else {
			infoPanel.gameObject.SetActive(false);
			GameManager.Instance.SetGameState(GameState.Investigation);
		}
	}

	public void ToggleLockWindow ()
	{
		RectTransform lockRectTransform = lockPanel.gameObject.GetComponent<RectTransform>();
		if (lockShowing) {
			patternButton.GetComponentInChildren<Text>().text = "Pattern Lock";
			lockRectTransform.DOAnchorPos(originalLockPosition, 0.3f, false).OnComplete(EndLockToggle);
			//TODO Remove this
			Elevator.Instance.OpenDoors();
			lockShowing = false;
		}
		else {
			if (inventoryShowing) {
				ToggleInventoryWindow();
			}
			if (GameManager.Instance.State != GameState.Investigation) {
				return;
			}
			lockPanel.gameObject.SetActive(true);
			GameManager.Instance.SetGameState(GameState.Uncontrolable);
			patternButton.GetComponentInChildren<Text>().text = "Close Pattern";
			lockRectTransform.DOAnchorPos(activeLockPosition, 0.3f, false).OnComplete(EndLockToggle);
			lockShowing = true;
		}
	}

	void EndLockToggle ()
	{
		if (lockShowing) {
			GameManager.Instance.SetGameState(GameState.OpenLock);
		}
		else {
			lockPanel.gameObject.SetActive(false);
			GameManager.Instance.SetGameState(GameState.Investigation);
		}
	}

	public void ToggleInventoryWindow ()
	{
		RectTransform inventoryRectTransform = inventoryPanel.gameObject.GetComponent<RectTransform>();
		if (inventoryShowing) {
			inventoryButton.GetComponentInChildren<Text>().text = "Inventory";
			if (InventoryManager.Instance.InspectMode) {
				InventoryManager.Instance.ToggleInspectMode();
			}
			inspectButton.GetComponent<Button>().onClick.RemoveAllListeners();
			inventoryRectTransform.DOAnchorPos(originalInventoryPosition, 0.3f, false).OnComplete(EndInventoryToggle);
			inventoryShowing = false;
		}
		else {
			if (lockShowing) {
				ToggleLockWindow();
			}
			if (GameManager.Instance.State != GameState.Investigation) {
				return;
			}
			GameManager.Instance.SetGameState(GameState.Inventory);
			inventoryPanel.gameObject.SetActive(true);
			inventoryButton.GetComponentInChildren<Text>().text = "Close Inventory";
			inspectButton.GetComponent<Button>().onClick.AddListener(InventoryManager.Instance.ToggleInspectMode);
			inventoryRectTransform.DOAnchorPos(activeInventoryPosition, 0.3f, false).OnComplete(EndInventoryToggle);
			inventoryShowing = true;
		}
	}

	void EndInventoryToggle ()
	{
		if (inventoryShowing) {
			GameManager.Instance.SetGameState(GameState.Inventory);
		}
		else {
			inventoryPanel.gameObject.SetActive(false);
			//TODO This is really stupid, the inspect mode should show up later
			if (GameManager.Instance.State != GameState.Inspection) {
				GameManager.Instance.SetGameState(GameState.Investigation);
			}
		}
	}

	public void SwitchElevatorButton (bool show)
	{
		patternButton.gameObject.SetActive(show);
	}

	public void DisplayInvestMode (Item item)
	{
		ToggleInventoryWindow();
		inspectItem.GetComponent<Image>().sprite = item.InspectSprite;
		ToggleInspectImage();
	}
	
	public void ToggleInspectImage ()
	{
		if (!inspectImageShowing) {
			GameManager.Instance.SetGameState(GameState.Inspection);

			inspectImageShowing = true;
			PlayerControlManager.Instance.CameraBlur.enabled = true;
			inspectItem.parent.gameObject.SetActive(true);

			inspectOffButton.gameObject.SetActive(true);
			inventoryButton.gameObject.SetActive(false);
		}
		else {
			GameManager.Instance.SetGameState(GameState.Investigation);

			inspectImageShowing = false;
			PlayerControlManager.Instance.CameraBlur.enabled = false;
			inspectItem.parent.gameObject.SetActive(false);
			
			inspectOffButton.gameObject.SetActive(false);
			inventoryButton.gameObject.SetActive(true);

			ToggleInventoryWindow();
		}
	}

	void Update ()
	{
		if (Input.GetKeyDown("a")) {
			ToggleLockWindow();
		}
		if (Input.GetKeyDown("s")) {
			ToggleInventoryWindow();
		}
		if (Input.GetButtonDown("Fire1") && infoShowing && GameManager.Instance.State == GameState.Confirmation) {
			ToggleInfoWindow(string.Empty, onInfoToggleCallback);
		}


	}
}
