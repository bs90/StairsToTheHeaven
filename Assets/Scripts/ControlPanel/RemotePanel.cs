using UnityEngine;
using System.Collections;

public class RemotePanel : MonoBehaviour {
	public int floor;

	public Transform[] lightComponents;
	public Material[] materials;
	
	public Material offMat;

	public int[] componentOrder;

	private void Awake()
	{
		Debug.Log ("Awake");
		StartCoroutine(LightUp());
	}

	private IEnumerator LightUp()
	{
		yield return new WaitForSeconds(1);
		SetupColors();
		Debug.Log ("Correct ? " + DataManager.Instance.ComparePanelData());
	}

	private void SetupColors()
	{
		if (lightComponents.Length != materials.Length || 
		    lightComponents.Length != DataManager.Instance.PanelData[floor].Colors.Count) {
			return;
		}

		for (int i = 0; i < materials.Length; i++) {
			bool lit = DataManager.Instance.GetPanelComponentState(materials[i].name, floor);
			Debug.Log (materials[i].name + " is lit: " + lit);
			if (lit) {
				lightComponents[i].GetComponent<MeshRenderer>().material = materials[i];
			}
			else {
				lightComponents[i].GetComponent<MeshRenderer>().material = offMat;
			}
		}
	}


}
