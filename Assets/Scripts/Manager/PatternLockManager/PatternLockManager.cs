﻿using UnityEngine;
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
	[HideInInspector]public List<Vector2> patternCoordinates;

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
		patternCoordinates = new List<Vector2>();
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
		pointLayout.spacing = new Vector2(400 / patternRows - 50, 400 / patternRows - 50);

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
		if ((startPoint.x - endPoint.x == startPoint.y - endPoint.y) && Mathf.Abs(endPoint.x - startPoint.x) > 1) {
			//Cross 1 Combo
			MarkBetweenPoints(startPoint, endPoint, new Vector2(1, 1), true);
		}
		else if ((startPoint.x + startPoint.y == endPoint.x + endPoint.y) && Mathf.Abs(endPoint.x - startPoint.x) > 1) {
			//Cross 2 Combo
			MarkBetweenPoints(startPoint, endPoint, new Vector2(1, -1), true);
		}
		else if (startPoint.y == endPoint.y && startPoint.x != endPoint.x && Mathf.Abs(endPoint.x - startPoint.x) > 1) {
			//Horizontal Combo
			MarkBetweenPoints(startPoint, endPoint, new Vector2(1, 0), true);
		}
		else if (startPoint.x == endPoint.x && startPoint.y != endPoint.y && Mathf.Abs(endPoint.y - startPoint.y) > 1) {
			//Vertical Combo
			MarkBetweenPoints(startPoint, endPoint, new Vector2(0, 1), false);
		}
		else {
			patternCoordinates.Add (endPoint);
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

		Dictionary<Vector2, float> newPoints = new Dictionary<Vector2, float>();

		for(int i = startNum; i > untilNum; i--) {
			if (!GetPointByCoordination(pointBetween).GetComponent<PatternPoint>().isSelected) {
				MarkPoint(GetPointByCoordination(pointBetween).GetComponent<PatternPoint>().pointId);
				float distanceBetween = Vector2.Distance(startPoint, pointBetween);
				newPoints.Add(pointBetween, distanceBetween);
			}
			pointBetween -= subsPoint;
		}  
		float endDistance = Vector2.Distance(startPoint, endPoint);
		newPoints.Add(endPoint, endDistance);
		var items = from pair in newPoints
			orderby pair.Value ascending
				select pair;
		foreach (KeyValuePair<Vector2, float> pair in items)
		{
			if (!patternCoordinates.Exists(element => element == pair.Key)) {
				patternCoordinates.Add(pair.Key);
			}
		}
	}

	public void MarkBetweenPointsAnother(Vector2 startPoint, Vector2 endPoint, Vector2 subsPoint, bool isX)
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
		patternCoordinates.Add(GetPointById(startPoint).GetComponent<PatternPoint>().pointPosition);
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
					compareePoints.Remove(compareePoints[tcp]);
				}
				else {
					//TODO Remove this if you don't want quick dead
					return false;
				}
			}
		}
		if (correctPoints == comparePoints.Count) {
			return true;
		}
		else {
			return false;
		}
	}

	public bool CompareCoordinations(List<Vector2> compareCoors,  List<Vector2> compareeCoors)
	{
		int correctPoints = 0;
		if(compareeCoors.Count == compareCoors.Count) {
			for (int i = 0; i < compareCoors.Count; i++) {
				if (compareeCoors[i] == compareCoors[i]) {
					correctPoints += 1;
				}
			}
			if (correctPoints == compareCoors.Count) {
				return true;
			}
		}
		return false;
	}

	public void CorrectPatternCheck ()
	{
		List<Vector2> toChecklist = new List<Vector2>();
		toChecklist.Add(new Vector2(0, 2));
		toChecklist.Add(new Vector2(1, 3));
		toChecklist.Add(new Vector2(2, 3));
		toChecklist.Add(new Vector2(2, 2));
		toChecklist.Add(new Vector2(1, 1));
		toChecklist.Add(new Vector2(1, 0));
		toChecklist.Add(new Vector2(2, 0));
		toChecklist.Add(new Vector2(3, 0));
		if (CompareCoordinations(toChecklist, patternCoordinates) == true) {
			//TODO Navigate to the right floor.
			if (InventoryManager.Instance.IsItemInInventory(4)) {
				InterfaceManager.Instance.ToggleInfoWindow("Floor 2's pattern successfully entered.", OnCompletePattern);
			}
			else {
				InterfaceManager.Instance.ToggleInfoWindow("The machine is acting weird.", null);
			}
		}
		else {
			InterfaceManager.Instance.ToggleInfoWindow("Entered pattern doesn't exist.", null);
		}
		
	}

	private void OnCompletePattern()
	{
		Elevator.Instance.ToggleDoors(()=> GameManager.Instance.LoadScene("F2"));
	}

	public void ClearPoints()
	{
		patternNumbers.Clear();
		patternCoordinates.Clear();
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
