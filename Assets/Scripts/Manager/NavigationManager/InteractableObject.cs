using UnityEngine;
using System.Collections;
using HighlightingSystem;

public class InteractableObject : MonoBehaviour {
	private bool notInteractable;
	public bool NotInteractable {
		get {
			return this.notInteractable;
		}
	}

	public bool isObject;
	public bool isElevator;

	public bool rotate;
	public float rotateSpeed;

	public GameObject[] moveablePoints; 

	private Ray ray;
	private RaycastHit hit;

	protected Highlighter highlighter;

	public GameObject[] availableObjects;
	
	public void OnMovingAway()
	{
		if (availableObjects.Length != 0) {
			foreach (GameObject availableObject in availableObjects) {
				ToggleInteractableObject(availableObject, false);
			}
		}
	}

	public void OnClosingIn()
	{
		if (availableObjects.Length != 0) {
			foreach (GameObject availableObject in availableObjects) {
				ToggleInteractableObject(availableObject, true);
			}
		}
	}

	private void ToggleInteractableObject (GameObject interactable, bool toggle)
	{
		//TODO Another shit like codes
		bool chestOpened = false;
		if (interactable.transform.GetComponent<Chest>()) {
			interactable.transform.GetComponent<Chest>().enabled = toggle;
			chestOpened = interactable.transform.GetComponent<Chest>().opened;
		}
		if (interactable.GetComponent<InteractableObject>()) {
			interactable.GetComponent<InteractableObject>().enabled = toggle;
		}
		else if (interactable.GetComponentInChildren<InteractableObject>()) {
			if (!chestOpened && toggle) {
				interactable.GetComponentInChildren<InteractableObject>().enabled = toggle;
			}
		}
	}

	void Start()
	{
		highlighter = GetComponent<Highlighter>();
		if (highlighter == null) { 
			highlighter = gameObject.AddComponent<Highlighter>(); 
		}
	}

	void Update () 
	{
		if (!notInteractable) {
			if (GameManager.Instance.State == GameState.Investigation) {
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit)) {
					if (hit.collider.transform == this.transform  && GameManager.Instance.State == GameState.Investigation) {
						highlighter.On(Color.cyan);
						if (Input.GetButtonDown("Fire1")) {
							PlayerControlManager player = PlayerControlManager.Instance;
							//TODO Why do I use controlable while I have divided game to states, I need better planning
							if (!isObject) {
								player.Move(this.transform.parent.gameObject, isElevator, null);
							}
							else if (isObject && !transform.parent.GetComponentInParent<Chest>().opened) {
								player.Rotate(this.transform.parent.gameObject, ()=> transform.parent.GetComponent<Chest>().OnEventInvestigateChest());
							}
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

	public void SetInteractAbility ()
	{
		if (notInteractable) {
			notInteractable = false;
		}
		else {
			notInteractable = true;
		}
	}
}
