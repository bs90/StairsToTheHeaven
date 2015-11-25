using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
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
	
	public Transform[] pathPoints;
	public int presentPoint;

	private GameObject destinationPoint;

	private bool inTheMove;

	private GameManager game;

	private Blur blur;
	public Blur CameraBlur{
		get {
			return this.gameObject.GetComponentInChildren<Blur>();
		}
	}

	void Start ()
	{
		game = GameManager.Instance;
		presentPoint = 0;
	}

	void Update ()
	{
		if (Input.GetButton("Fire2") && game.State == GameState.Investigation) {
			Rotate();
		}
	}

	public void Move (GameObject navigationPoint, bool faceFrontElevator, TweenCallback callback)
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
		if (callback != null) {
			moveSequence.OnComplete(callback);
		}
		moveSequence.Play();
	}

	public void Rotate (GameObject rotateToObject, TweenCallback callback) {
		Quaternion targetRotation = Quaternion.LookRotation (rotateToObject.transform.position - transform.position);
		Sequence rotateSequence = DOTween.Sequence();
		rotateSequence.Append(transform.DORotate(targetRotation.eulerAngles, 2, RotateMode.Fast)).OnStart(StartRotate).OnComplete(EndRotate);
		if (callback != null) {
			rotateSequence.OnComplete(callback);
		}
		rotateSequence.Play();
	}

	void StartRotate()
	{
		game.SetGameState(GameState.Uncontrolable);
	}

	void EndRotate ()
	{
		game.SetGameState(GameState.Investigation);
	}

	void StartPath ()
	{
		//TODO Kinda bad reference
		game.SetGameState(GameState.Uncontrolable);
		NavigationManager.Instance.MoveAwayFromPoint(NavigationPoints.Instance.presentNavigationPoint);
	}

	void EndPath ()
	{
		//TODO Sometimes it causes warnings, why?
		if (destinationPoint.GetComponentInChildren<InteractableObject>().isElevator) {
			InterfaceManager.Instance.ToggleInfoWindow("The elevator is locked with a pattern code, enter the code to start up (yes, like your android smartphones).", OpenLockPanel);
		}

		game.SetGameState(GameState.Investigation);
		NavigationManager.Instance.SetPresentPoint(destinationPoint);
		destinationPoint = null;
	}

	void OpenLockPanel()
	{
		InterfaceManager.Instance.ToggleLockWindow();
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

	void HeadBob () { 

	}
}
