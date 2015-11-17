using UnityEngine;
using System.Collections;
using HighlightingSystem;

public class HighlightObject : MonoBehaviour {
	public bool isObject;
	public bool isElevator;

	public bool rotate;
	public float rotateSpeed;

	public GameObject[] moveablePoints; 

	private Ray ray;
	private RaycastHit hit;

	protected Highlighter highlighter;

	void Start()
	{
		highlighter = GetComponent<Highlighter>();
		if (highlighter == null) { 
			highlighter = gameObject.AddComponent<Highlighter>(); 
		}
	}

	void Update () 
	{
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit)) {
			if (hit.collider.transform == this.transform  && PlayerControlManager.Instance.controlable) {
				highlighter.On(Color.cyan);
				if (Input.GetButtonDown("Fire1")) {
					PlayerControlManager player = PlayerControlManager.Instance;
					if (!isObject && player.controlable) {
						player.Move(this.transform.parent.gameObject, isElevator);
					}
				}
			}
		}

		if (this.transform.eulerAngles.y == 360 || this.transform.eulerAngles.y == -360) {
			this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 0, this.transform.eulerAngles.z);
		}

		if (rotate) {
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, 
			                                           Quaternion.Euler(this.transform.eulerAngles.x, 
			                 											this.transform.eulerAngles.y + rotateSpeed,
                                                                        this.transform.eulerAngles.z), 
			                                           Time.deltaTime);
		}
	}
}
