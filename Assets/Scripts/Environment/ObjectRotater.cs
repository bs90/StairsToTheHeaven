using UnityEngine;
using System.Collections;

public class ObjectRotater : MonoBehaviour {

	public float sensitivityX = 15;
	public float sensitivityY = 15;

	public Transform objectCamera;

	void Start () {
	
	}
	
	void Update () {
		if (Input.GetButton("Fire1")) {
			float rotationX = Input.GetAxis("Mouse X") * sensitivityX;
			float rotationY = Input.GetAxis("Mouse Y") * sensitivityY;

//			transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.Euler(rotationY, -rotationX, this.transform.rotation.z), Time.deltaTime);

			transform.RotateAround(transform.position, objectCamera.up, -rotationX * Time.deltaTime);
			transform.RotateAround(transform.position, objectCamera.right, rotationY * Time.deltaTime);
//			transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
		}

	}
}
