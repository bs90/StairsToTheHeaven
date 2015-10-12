using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MouseLook : MonoBehaviour {
	
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

	public Transform playerTransform;
	public Transform headTransform;

	private Animator animator;
	private AnimationHash hash;
	
	public Transform[] pathPoints;
	public int presentPoint;

	void Start ()
	{
		hash = GetComponent<AnimationHash>();
		animator = playerTransform.GetComponent<Animator>();

		controlable = true;
		presentPoint = 0;
	}

	void Update ()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idling") && controlable) {
			this.transform.SetParent(playerTransform);
			if (Input.GetButton("Fire2")) {
				Rotate();
			}
		}
		else {
			this.transform.SetParent(headTransform);
		}

//		if (Input.GetButtonUp("Fire1") && controlable) {
//			animator.SetTrigger(hash.fallTrigger);
//			this.transform.rotation = this.transform.parent.rotation;
//		}

		if (Input.GetKeyUp("a") && controlable) {
			presentPoint += 1;
			if (presentPoint >= pathPoints.Length) {
				presentPoint = 1;
			}

//			playerTransform.rotation = transform.rotation;
			playerTransform.LookAt(pathPoints[presentPoint]);
			rotationY = 0;

			Sequence moveSequence = DOTween.Sequence();
			moveSequence.Append(transform.DORotate(playerTransform.rotation.eulerAngles, 3, RotateMode.Fast));
//			moveSequence.PrependInterval(1);
			moveSequence.Append(playerTransform.DOMove(pathPoints[presentPoint].position, 5, false).OnStart(StartPath).OnComplete(EndPath));
			moveSequence.Play();
		}
	}

	void FixedUpdate ()
	{

	}

	void StartPath ()
	{
		controlable = false;
		animator.SetBool(hash.walkBool, true);
	}

	void EndPath ()
	{
		animator.SetBool(hash.walkBool, false);
		controlable = true;
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
