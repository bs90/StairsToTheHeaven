using UnityEngine;
using System.Collections;

public class NavigationManager : MonoSingleton<NavigationManager> {

	public GameObject presentNavigationPoint;

	public GameObject[] navigationPoints;

//	public GameObject[] firstFloorPoints;
//	public GameObject[] secondFloorPoints;
//	public GameObject[] thirdFloorPoints;
//	public GameObject[] forthFloorPoints;

	private void Start()
	{
		//TODO Read from save point
		MoveAwayFromPoint(navigationPoints[1]);
		SetPresentPoint(navigationPoints[0]);
	}

	public void SetPresentPoint (GameObject presentPoint)
	{
		if (presentPoint != null) {
			presentNavigationPoint = presentPoint;
			InteractableObject interactable = presentPoint.GetComponentInChildren<InteractableObject>();
			if (interactable) {
				presentPoint.GetComponentInChildren<InteractableObject>().OnClosingIn();
				if (!interactable.isObject) {
					presentPoint.SetActive(false);
				}
				if (interactable.isElevator) {
					InterfaceManager.Instance.SwitchElevatorButton(true);
				}
			}
		}
		else {
			return;
		}
	}

	public void SetMoveablePoints (GameObject[] moveablePoints)
	{
		foreach (GameObject moveablePoint in moveablePoints) {
			moveablePoint.SetActive(true);
		}
	}

	public void MoveAwayFromPoint (GameObject awayPoint)
	{
		presentNavigationPoint = null;
		awayPoint.SetActive(true);
		InteractableObject interactable = awayPoint.GetComponentInChildren<InteractableObject>();
		if (interactable) {
			interactable.OnMovingAway();
			if (!interactable.isObject) {
				InterfaceManager.Instance.SwitchElevatorButton(false);
			}
		}
	}

	public GameObject GetPresentPoint ()
	{
		//TODO Complete this function
		GameObject point = new GameObject();
		return point;
	}
}
