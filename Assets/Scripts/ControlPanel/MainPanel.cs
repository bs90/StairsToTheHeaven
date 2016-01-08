using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainPanel : MonoBehaviour {

	public Transform button1;
	public Transform button2;
	public Transform button3;

	public Transform mainButton;

	public Material[] colorMats;

	private bool isEventCleared;
	public int eventId;

	public RemotePanelColors button1Color;
	public RemotePanelColors button2Color;
	public RemotePanelColors button3Color;

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
		if (NavigationManager.Instance.GetPresentPoint().name == "Point5" && Input.GetMouseButtonDown(0)) {
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
			}
		}
	}

	private void Reward()
	{

	}

	private RemotePanelColors ChangeMatColor(Transform button, RemotePanelColors color)
	{
		Debug.Log (button.name);
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
		Debug.Log ("Gasp");
		List<RemotePanelColors> colors = new List<RemotePanelColors>();
		colors.Add(button1Color);
		colors.Add(button2Color);
		colors.Add(button3Color);
		DataManager.Instance.SavePanelColorData(colors);
		InterfaceManager.Instance.ToggleInfoWindow(button1Color.ToString() + " " + button2Color.ToString() + " " + button3Color.ToString() + " were toggled.", null);
	}
}
