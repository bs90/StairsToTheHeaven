using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PatternLockManager : MonoSingleton<PatternLockManager> {

	[HideInInspector]public List<GameObject> pointList;
	public int patternRows;
	private int pointNumber;
	public GameObject pointPrefab;
	public GameObject pointParent;

	[HideInInspector]public List<int> patternNumbers;
	[HideInInspector]public int lineNumber;
	[HideInInspector]public Vector3 startLinePoint;
	[HideInInspector]public List<GameObject> connectingLines;
	public int lineWidth;
	public GameObject dotPrefab;
	public GameObject lineParent;
	private RectTransform presentRectTransform;
	
	void Start () 
	{
		patternNumbers = new List<int>();
		connectingLines = new List<GameObject>();
		pointList = new List<GameObject>();

		CreatePatternGrid();
	}

	public void CreatePatternGrid()
	{
		if (pointPrefab == null || pointParent == null) {
			//TODO Destroy the game, ahaha
			Debug.LogError("Setup not completed, critical error!");
		}

		//TODO PointLayout needs to be rewritten if this is to be used again
		GridLayoutGroup pointLayout = pointParent.GetComponent<GridLayoutGroup>();
		pointLayout.spacing = new Vector2(400 / patternRows - (5 * patternRows + 5), 400 / patternRows - (5 * patternRows + 5));

		for (int x = 0; x < patternRows; x++) {
			for (int y = 0; y < patternRows; y++) {
				GameObject pointObject = Instantiate(pointPrefab, pointPrefab.transform.position, pointPrefab.transform.rotation) as GameObject;
				pointObject.transform.SetParent(pointParent.transform);
				pointList.Add(pointObject);
				pointNumber = pointList.Count - 1;
				pointObject.name = string.Format("Point" + pointNumber.ToString()); 
				pointObject.GetComponent<PatternPoint>().pointId = pointNumber;
				pointObject.GetComponent<PatternPoint>().pointPosition = new Vector2(x, y);
			}
		}
	}

	public void IsUnsignedPointExistBetweenLine(Vector2 startPoint, Vector2 endPoint)
	{
		//TODO Someone help me refactor this shit
		Debug.Log("start point " + startPoint + " end point " + endPoint);
		if ((startPoint.x - endPoint.x == startPoint.y - endPoint.y) && Mathf.Abs(endPoint.x - startPoint.x) > 1) {
			MarkBetweenPoints(startPoint, endPoint, new Vector2(1, 1), true);
			Debug.Log ("Cross 1");
		}
		else if ((startPoint.x + startPoint.y == endPoint.x + endPoint.y) && Mathf.Abs(endPoint.x - startPoint.x) > 1) {
			MarkBetweenPoints(startPoint, endPoint, new Vector2(1, -1), true);
			Debug.Log ("Cross 2");
		}
		else if (startPoint.y == endPoint.y && startPoint.x != endPoint.x && Mathf.Abs(endPoint.x - startPoint.x) > 1) {
			MarkBetweenPoints(startPoint, endPoint, new Vector2(1, 0), true);
			Debug.Log ("Horizontal");
		}
		else if (startPoint.x == endPoint.x && startPoint.y != endPoint.y && Mathf.Abs(endPoint.y - startPoint.y) > 1) {
			MarkBetweenPoints(startPoint, endPoint, new Vector2(0, 1), false);
			Debug.Log ("Vertical");
		}
	}

	public void MarkBetweenPoints(Vector2 startPoint, Vector2 endPoint, Vector2 subsPoint, bool isX)
	{
		Vector2 furtherPoint = Vector2.zero;
		Vector2 closerPoint = Vector2.zero;

		int start = (isX) ? (int)startPoint.x : (int)startPoint.y;
		int end = (isX) ? (int)endPoint.x : (int)endPoint.y;

		if (start > end) {
			furtherPoint = startPoint;
			closerPoint = endPoint;
		}
		else {
			furtherPoint = endPoint;
			closerPoint = startPoint;
		}
		Vector2 pointBetween = furtherPoint - subsPoint;

		int startNum = (isX) ? (int)furtherPoint.x - 1 : (int)furtherPoint.y - 1;
		int untilNum = (isX) ? (int)closerPoint.x : (int)closerPoint.y;

		for(int i = startNum; i > untilNum; i--) {
			if (!GetPointByCoordination(pointBetween).GetComponent<PatternPoint>().isSelected) {
				MarkPoint(GetPointByCoordination(pointBetween).GetComponent<PatternPoint>().pointId);
			}
			pointBetween -= subsPoint;
		}  
	}

	public List<GameObject> GetObjectsBetweenPoints(Vector2 startPoint, Vector3 endPoint) 
	{
		return null;
	}

	public GameObject GetPointByCoordination(Vector2 coordinate)
	{
		if (pointList == null || pointList.Count == 0) {
			return null;
		}
		foreach(GameObject pointObject in pointList) {
			if (pointObject.GetComponent<PatternPoint>().pointPosition == coordinate) {
				return pointObject;
			}
		}
		return null;
	}

	public GameObject GetPointById(int id)
	{
		if (pointList == null || pointList.Count == 0) {
			return null;
		}
		foreach(GameObject pointObject in pointList) {
			if (pointObject.GetComponent<PatternPoint>().pointId == id) {
				return pointObject;
			}
		}
		return null;
	}

	public void StartDrag(int startPoint)
	{
		patternNumbers.Add(startPoint);
	}

	public void EnterPointDrag(int enterPoint)
	{
		if(patternNumbers.Exists(element => element == enterPoint) == false) {
			patternNumbers.Add(enterPoint);
		}
	}

	public bool ComparePoints(List<int> comparePoints, List<int> compareePoints)
	{
		int correctPoints = 0;
		//TODO Stupid naming at the best
		for (int cp = 0; cp < comparePoints.Count; cp++) {
			for (int tcp = 0; tcp < compareePoints.Count; tcp++) {
				if (compareePoints[tcp] == comparePoints[cp]) {
					correctPoints += 1;
				}
				compareePoints.Remove(compareePoints[tcp]);
			}
		}
		if (correctPoints == comparePoints.Count) {
			return true;
		}
		else {
			return false;
		}
	}

	public void ClearPoints()
	{
		patternNumbers.Clear();
		foreach(GameObject pointtObject in pointList) {
			pointtObject.GetComponent<PatternPoint>().isSelected = false;
		}
	}

	public void DrawAtPoint(Vector3 startPoint)
	{
		GameObject startLine = Instantiate(dotPrefab, startPoint, dotPrefab.transform.rotation) as GameObject;
		startLine.transform.SetParent(lineParent.transform);
		connectingLines.Add(startLine);
		lineNumber = connectingLines.Count;
		startLine.name = string.Format("Line" + lineNumber.ToString());
		startLinePoint = startPoint;
		presentRectTransform = startLine.GetComponent<RectTransform>();
	}

	public void DrawNextLine()
	{
		GameObject startLine = Instantiate(dotPrefab, startLinePoint, dotPrefab.transform.rotation) as GameObject;
		startLine.transform.SetParent(lineParent.transform);
		connectingLines.Add(startLine);
		lineNumber = connectingLines.Count;
		startLine.name = string.Format("Line" + lineNumber.ToString());
		presentRectTransform = startLine.GetComponent<RectTransform>();
	}

	public void SetStartPoint(Vector3 startPoint)
	{
		startLinePoint = startPoint;
	}

	public void DrawLine(Vector3 pointB)
	{
		Vector3 differenceVector = pointB - startLinePoint;
		if (presentRectTransform) {
			presentRectTransform.sizeDelta = new Vector2( differenceVector.magnitude, lineWidth);
			presentRectTransform.pivot = new Vector2(0, 0.5f);
			presentRectTransform.position = startLinePoint;
			float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
			presentRectTransform.rotation = Quaternion.Euler(0,0, angle);
		}
	}

	public void ClearLines()
	{
		foreach(GameObject lineObject in connectingLines) {
			Destroy(lineObject);
		}
		foreach(GameObject pointObject in pointList) {
			pointObject.GetComponent<Image>().color = Color.white;
		}
		connectingLines.Clear();
	}

	public void MarkPoint(int id) {
		if (pointList != null || pointList.Count != 0) {
			pointList[id].GetComponent<Image>().color = new Color32(64, 132, 242, 100);
			pointList[id].GetComponent<PatternPoint>().isSelected = true;
		}
	}
}
