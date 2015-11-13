using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class InterfaceManager : MonoSingleton<InterfaceManager> {

	public Transform inventoryPanel;
	public Transform lockPanel;
	public Transform optionPanel;

	public Transform infoPanel;
	public Transform messagePanel;

	private bool inventoryShowing;
	private bool lockShowing;
	private bool optionShowing;

	private bool infoShowing;
	private bool messageShowing;

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
		}
		else {
			infoPanel.gameObject.SetActive(false);
		}
		GameManager.Instance.SetGameState(GameState.Investigation);
	}

	public void ToggleLockWindow ()
	{
		GameManager.Instance.SetGameState(GameState.Uncontrolable);
		RectTransform lockRectTransform = lockPanel.gameObject.GetComponent<RectTransform>();
		if (lockShowing) {
			lockRectTransform.DOAnchorPos(originalLockPosition, 0.3f, false).OnComplete(EndLockToggle);
			//TODO Remove this
			Elevator.Instance.OpenDoors();
			lockShowing = false;
		}
		else {
			if (inventoryShowing) {
				ToggleInventoryWindow();
			}
			lockRectTransform.DOAnchorPos(activeLockPosition, 0.3f, false).OnComplete(EndLockToggle);
			lockShowing = true;
		}
	}

	void EndLockToggle ()
	{
		GameManager.Instance.SetGameState(GameState.Investigation);
	}

	public void ToggleInventoryWindow ()
	{
		GameManager.Instance.SetGameState(GameState.Uncontrolable);
		RectTransform inventoryRectTransform = inventoryPanel.gameObject.GetComponent<RectTransform>();
		if (inventoryShowing) {
			inventoryRectTransform.DOAnchorPos(originalInventoryPosition, 0.3f, false).OnComplete(EndInventoryToggle);
			inventoryShowing = false;
		}
		else {
			if (lockShowing) {
				ToggleLockWindow();
			}
			inventoryRectTransform.DOAnchorPos(activeInventoryPosition, 0.3f, false).OnComplete(EndInventoryToggle);
			inventoryShowing = true;
		}
	}

	void EndInventoryToggle ()
	{
		GameManager.Instance.SetGameState(GameState.Investigation);
	}

	void Start ()
	{
		GameManager.Instance.SetGameState(GameState.Investigation);
	}

	void Update ()
	{
		if (GameManager.Instance.State == GameState.Investigation) {
			if (Input.GetKeyDown("a")) {
				ToggleLockWindow();
			}
			if (Input.GetKeyDown("s")) {
				ToggleInventoryWindow();
			}
			if (Input.GetButtonDown("Fire1") && infoShowing) {
				ToggleInfoWindow(string.Empty, onInfoToggleCallback);
			}
		}
	}
}
