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
		SetPresentPoint(firstFloorPoints[0]);
	}

	public void SetPresentPoint (GameObject presentPoint)
	{
		presentNavigationPoint = presentPoint;
		presentPoint.SetActive(false);
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
