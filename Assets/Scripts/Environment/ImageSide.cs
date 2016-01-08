using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class ImageSide : MonoBehaviour {
	public Transform leftSide;
	public Transform rightSide;

	public bool isEventCleared;
	public int eventId = 0;

	public List<int> correctSequence;
	public List<int> presentSequence = new List<int>();

	public int rewardItem = 0;

	void Awake () {
		StartCoroutine(CheckState());
	}

	IEnumerator CheckState()
	{
		yield return new WaitForEndOfFrame();
		isEventCleared = DataManager.Instance.GetEventState(eventId);
		if (isEventCleared) {
			this.gameObject.GetComponent<ImageSide>().enabled = false;
		}
	}


	void Update () {
		if (NavigationManager.Instance.GetPresentPoint() != null && !isEventCleared) {
			if (GameManager.Instance.isInvestigationState) {
				ImageInteraction();
			}
		}
	}

	void ImageInteraction()
	{
		if (NavigationManager.Instance.GetPresentPoint().name == "Point4" && Input.GetMouseButtonUp(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100)) {
				if (hit.collider.transform == leftSide) {
					if (presentSequence.Count != 0 && presentSequence[presentSequence.Count - 1] == 0) {
						this.transform.DOKill(true);
						this.transform.DOShakeRotation(0.5f, 5, 5, 0);
					}
					else {
						this.transform.DOKill(true);
						this.transform.DORotate(new Vector3(80, -180, 90), 1, RotateMode.Fast);
					}
					InterfaceManager.Instance.ToggleInfoWindow("You pressed the left side of the picture.", ()=>AddSequence(0));

				}
				else if (hit.collider.transform == rightSide) {
					if (presentSequence.Count != 0 && presentSequence[presentSequence.Count - 1] == 1) {
						this.transform.DOKill(true);
						this.transform.DOShakeRotation(0.5f, 5, 5, 0);
					}
					else {
						this.transform.DOKill(true);
						this.transform.DORotate(new Vector3(80, 0, -90), 1, RotateMode.Fast);
					}
					InterfaceManager.Instance.ToggleInfoWindow("You pressed the right side of the picture.", ()=>AddSequence(1));
				}
			}
		}
	}

	void AddSequence(int sequence)
	{
		presentSequence.Add(sequence);
		if (presentSequence.Count > correctSequence.Count) {
			presentSequence.RemoveAt(0);
		}
		if (presentSequence.Count == correctSequence.Count) {
			Debug.Log ("Run here");
			for (int i = 0; i < presentSequence.Count; i++) {
				if (presentSequence[i] != correctSequence[i]) {
					Debug.Log ("element " + i + " sequence " + presentSequence[i] + " is not equal " + correctSequence[i]);
					return;
				}
			}
			isEventCleared = true;
			InterfaceManager.Instance.ToggleInfoWindow("You heard something felt out from behind of picture.", RewardMessage);
		}
	}

	void RewardMessage()
	{
		InterfaceManager.Instance.ToggleInfoWindow("You picked up a <color=yellow>" + ItemDatabase.Instance.FetchItemByID(rewardItem).Title + "</color>.", RewardItem);
	}

	void RewardItem()
	{
		InventoryManager.Instance.AddItem(rewardItem, 1);
		DataManager.Instance.SaveEventData(0, true);
		DataManager.Instance.WriteGameData("GameData.json");
	}

	IEnumerator CorrectSequenceExecution()
	{
		yield return new WaitForSeconds(1);
	}
}
