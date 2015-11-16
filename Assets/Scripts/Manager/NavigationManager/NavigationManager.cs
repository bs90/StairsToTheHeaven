using UnityEngine;
using System.Collections;

public class NavigationManager : MonoSingleton<NavigationManager> {

	public GameObject presentNavigationPoint;

	public GameObject[] basementTenthFloorPoints;

	public GameObject[] firstFloorPoints;
	public GameObject[] secondFloorPoints;
	public GameObject[] thirdFloorPoints;
	public GameObject[] forthFloorPoints;

	private void Start()
	{
		//TODO Read from save point
		SetPresentPoint(basementTenthFloorPoints[0]);
	}

	public void SetPresentPoint (GameObject presentPoint)
	{
		if (presentPoint != null) {
			presentNavigationPoint = presentPoint;
			presentPoint.SetActive(false);
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
	}

	public GameObject GetPresentPoint ()
	{
		//TODO Complete this function
		GameObject point = new GameObject();
		return point;
	}
}
