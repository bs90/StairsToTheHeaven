using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Elevator : MonoSingleton<Elevator> {

	public Vector3 rightDoorClosePos = new Vector3(2.45f, -0.35f, -1.1f);
	public Vector3 leftDoorClosePos = new Vector3(1.55f, -0.35f, -1.1f);

	public Vector3 rightDoorOrigPos = new Vector3(3.45f, -0.35f, -1.1f);
	public Vector3 leftDoorOrigPos = new Vector3(0.55f, -0.35f, -1.1f);
	
	public Transform rightDoor;
	public Transform leftDoor;

	public bool doorsOpened = true;
	
	void Update() 
	{
		if (Input.GetButtonDown("Jump")) {
			if (doorsOpened) {
				CloseDoors();
			}
			else {
				OpenDoors();
			}
		}
	}

	public void OpenDoors() 
	{
		doorsOpened = true;

		rightDoor.DOMove(rightDoorOrigPos, 2);
		leftDoor.DOMove(leftDoorOrigPos, 2);
	}

	public void CloseDoors()
	{
		doorsOpened = false;

		rightDoor.DOMove(rightDoorClosePos, 2);
		leftDoor.DOMove(leftDoorClosePos, 2);
	}
}
