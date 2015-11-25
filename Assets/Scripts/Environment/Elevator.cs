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

	public bool doorsOpened;

	private GameState previousState;

	public void ToggleDoors(TweenCallback callback) 
	{
		previousState = GameManager.Instance.State;
		GameManager.Instance.SetGameState(GameState.Uncontrolable);
		doorsOpened = !doorsOpened;

		if (doorsOpened) {
			Sequence doorSequence = DOTween.Sequence();
			doorSequence.Append(rightDoor.DOMove(rightDoorClosePos, 2));
			doorSequence.OnComplete(callback);
			doorSequence.Play();

			leftDoor.DOMove(leftDoorClosePos, 2).OnComplete(OnDoorsEnd);
		}
		else {
			Sequence doorSequence = DOTween.Sequence();
			doorSequence.Append(rightDoor.DOMove(rightDoorOrigPos, 2));
			doorSequence.OnComplete(callback);
			doorSequence.Play();

			leftDoor.DOMove(leftDoorOrigPos, 2).OnComplete(OnDoorsEnd);
		}

	}

	public void OnDoorsEnd()
	{
		GameManager.Instance.SetGameState(previousState);
	}
}
