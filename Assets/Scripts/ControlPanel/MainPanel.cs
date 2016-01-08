using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainPanel : MonoBehaviour {

	public Transform button1;
	public Transform button2;
	public Transform button3;

	public Transform mainButton;
	public Transform resetButton;

	public Material[] colorMats;

	private bool isEventCleared;
	public int eventId;

	public RemotePanelColors button1Color;
	public RemotePanelColors button2Color;
	public RemotePanelColors button3Color;

	public string accessPoint = "Point3";
	public int rewardItem = 14;
//	public RemotePanel panel1;
//	public RemotePanel panel2;
//	public RemotePanel panel3;
//	public RemotePanel panel4;

	void Awake() 
	{
		StartCoroutine(SetupColors());
	}

	IEnumerator SetupColors()
	{
		yield return new WaitForSeconds(1);
		button1.GetComponent<MeshRenderer>().material = colorMats[0];
		button2.GetComponent<MeshRenderer>().material = colorMats[1];
		button3.GetComponent<MeshRenderer>().material = colorMats[2];

		button1Color = (RemotePanelColors)0;
		button2Color = (RemotePanelColors)1;
		button3Color = (RemotePanelColors)2;

		isEventCleared = DataManager.Instance.GetEventState(eventId);
		if (isEventCleared) {
			this.gameObject.GetComponent<MainPanel>().enabled = false;
		}
	}

	void OnEnable () {
		isEventCleared = DataManager.Instance.GetEventState(eventId);
		if (isEventCleared) {
			Destroy(this.gameObject);
		}
	}

	void Update () {
		if (NavigationManager.Instance.GetPresentPoint() != null && !isEventCleared) {
			if (GameManager.Instance.isInvestigationState) {
				ButtonsInteraction();
			}
		}
	}

	void ButtonsInteraction()
	{
		if (NavigationManager.Instance.GetPresentPoint().name == accessPoint && Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100)) {
				if (hit.collider.transform == button1) {
					button1Color = ChangeMatColor(button1, button1Color);
				}
				else if (hit.collider.transform == button2) {
					button2Color = ChangeMatColor(button2, button2Color);
				}
				else if (hit.collider.transform == button3) {
					button3Color = ChangeMatColor(button3, button3Color);
				}
				else if (hit.collider.transform == mainButton) {
					ExecuteColorChange();
				}
				else if (hit.collider.transform == resetButton) {
					ResetColors();
				}
			}
		}
	}

	private void Reward()
	{
		if (DataManager.Instance.ComparePanelData() == true) {
			InterfaceManager.Instance.ToggleInfoWindow("Something unlocked, and a piece of paper fell out.", GetItem);
		}
		else {
			InterfaceManager.Instance.ToggleInfoWindow("Nothing happened...", null);
		}
	}

	private void GetItem()
	{
		DataManager.Instance.SaveEventData(eventId, true);
		InterfaceManager.Instance.ToggleInfoWindow("You picked up a <color=yellow>" + ItemDatabase.Instance.FetchItemByID(rewardItem).Title + "</color>.", RewardItem);
	}

	private void RewardItem()
	{
		InventoryManager.Instance.AddItem(rewardItem, 1);
	}

	private RemotePanelColors ChangeMatColor(Transform button, RemotePanelColors color)
	{
		int colorValue = (int)color;
		if (colorValue < 6) {
			colorValue += 1;
		}
		else {
			colorValue = 0;
		}
		button.GetComponent<MeshRenderer>().material = colorMats[colorValue];
		return (RemotePanelColors)colorValue;
	}

	private void ExecuteColorChange()
	{
		List<RemotePanelColors> colors = new List<RemotePanelColors>();
		colors.Add(button1Color);
		colors.Add(button2Color);
		colors.Add(button3Color);
		DataManager.Instance.SavePanelColorData(colors);
		DataManager.Instance.WriteGameData("GameData.json");
//		StartCoroutine(panel1.LightUp());
//		StartCoroutine(panel2.LightUp());
//		StartCoroutine(panel3.LightUp());
//		StartCoroutine(panel4.LightUp());
		InterfaceManager.Instance.ToggleInfoWindow(button1Color.ToString() + " " + button2Color.ToString() + " " + button3Color.ToString() + " were toggled.", Reward);
	}

	private void ResetColors()
	{
		InterfaceManager.Instance.ToggleInfoWindow("You pressed a reset button, the colors are back to default.", null);
		DataManager.Instance.ResetPanelData();
		DataManager.Instance.WriteGameData("GameData.json");
	}
}
