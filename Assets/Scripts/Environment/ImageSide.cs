using UnityEngine;
using System.Collections;

public class ImageSide : MonoBehaviour {
	public Transform leftSide;
	public Transform rightSide;

	private bool isEventCleared;
	public int eventId = 0;

	void OnEnable () {
		isEventCleared = DataManager.Instance.GetEventState(eventId);
		if (isEventCleared) {
			Destroy(this.gameObject);
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
		if (NavigationManager.Instance.GetPresentPoint().name == "Point4" && Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100)) {
				if (hit.collider.transform == leftSide) {
					Debug.Log ("Hit left side");
				}
				else if (hit.collider.transform == rightSide) {
					Debug.Log ("Hit right side");
				}
			}
		}
	}
}
