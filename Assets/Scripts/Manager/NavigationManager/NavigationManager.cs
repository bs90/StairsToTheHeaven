using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class NavigationManager : MonoSingleton<NavigationManager> {
	//TODO Reload on start new scene
	//TODO Read from save point
	void Start() {
		SetupNavigation();
	}

	public void SetupNavigation ()
	{
		MoveAwayFromPoint(NavigationPoints.Instance.navigationPoints[1]);
		SetPresentPoint(NavigationPoints.Instance.navigationPoints[0]);
	}

	public void SetPresentPoint (GameObject presentPoint)
	{
		if (presentPoint != null) {
			NavigationPoints.Instance.presentNavigationPoint = presentPoint;
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

	public void MoveAwayFromPoint (GameObject awayPoint)
	{
		NavigationPoints.Instance.presentNavigationPoint = null;
		awayPoint.SetActive(true);
		InteractableObject interactable = awayPoint.GetComponentInChildren<InteractableObject>();
		if (interactable) {
			interactable.OnMovingAway();
			if (!interactable.isObject) {
				InterfaceManager.Instance.SwitchElevatorButton(false);
			}
		}
	}

	public void SetMoveablePoints (GameObject[] moveablePoints)
	{
		foreach (GameObject moveablePoint in moveablePoints) {
			moveablePoint.SetActive(true);
		}
	}

	public GameObject GetPresentPoint ()
	{
		//TODO Complete this function
		if (NavigationPoints.instance.presentNavigationPoint != null) {
			return NavigationPoints.instance.presentNavigationPoint;
		}
		return null;
	}
}
