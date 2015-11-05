using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerControlManager : MonoSingleton<PlayerControlManager> {
	
	public enum RotationAxes { 
		MouseXAndY = 0, 
		MouseX = 1, 
		MouseY = 2 
	}

	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;
	
	public float minimumX = -360F;
	public float maximumX = 360F;
	
	public float minimumY = -60F;
	public float maximumY = 60F;
	
	float rotationY = 0F;

	public float headRotationVelocity;

	public bool controlable;

//	private Animator animator;
//	private AnimationHash hash;
	
	public Transform[] pathPoints;
	public int presentPoint;

	private GameObject destinationPoint;

	void Start ()
	{
		controlable = true;
		presentPoint = 0;
	}

	void Update ()
	{
		if (Input.GetButton("Fire2") && controlable) {
			Rotate();
		}
	}

	public void Move (GameObject navigationPoint, bool faceFrontElevator)
	{
		destinationPoint = navigationPoint;
		Vector3 toPosition = navigationPoint.transform.position;
		Quaternion targetRotation = Quaternion.LookRotation (toPosition - transform.position);

		Sequence moveSequence = DOTween.Sequence();
		moveSequence.Append(transform.DORotate(targetRotation.eulerAngles, 1, RotateMode.Fast).OnStart(StartPath));

		if (faceFrontElevator) {
			moveSequence.Append(transform.DOMove(toPosition, 4, false));
            moveSequence.Append(transform.DORotate(navigationPoint.transform.rotation.eulerAngles, 2, RotateMode.Fast).OnComplete(EndPath));
		}
		else {
			moveSequence.Append(transform.DOMove(toPosition, 4, false).OnComplete(EndPath));
		}
		moveSequence.Play();
	}

	void StartPath ()
	{
		controlable = false;
		NavigationManager.Instance.MoveAwayFromPoint(NavigationManager.Instance.presentNavigationPoint);
	}

	void EndPath ()
	{
		//TODO Sometimes it causes warnings, why?
		if (destinationPoint.GetComponentInChildren<HighlightObject>().isElevator) {
			InterfaceManager.Instance.ToggleLockWindow();
			Elevator.Instance.CloseDoors();
		}

		controlable = true;
		NavigationManager.Instance.SetPresentPoint(destinationPoint);
		destinationPoint = null;
	}

	void Rotate()
	{
		if (axes == RotationAxes.MouseXAndY) {
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX) {
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else {
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
		}
	}
}
