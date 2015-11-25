using UnityEngine;
using System.Collections;

public class AnimationHash : MonoBehaviour {

	public int idlingState;
	public int walkingState;
	public int fallingState;
	public int standingState;

	public int walkBool;
	public int fallTrigger;
	
	void Awake() {
		idlingState = Animator.StringToHash("Base Layer.Idling");
		walkingState = Animator.StringToHash("Base Layer.Walking");
		fallingState = Animator.StringToHash("Base Layer.Falling");
		standingState = Animator.StringToHash("Base Layer.Standing");

		walkBool = Animator.StringToHash("Walk");
		fallTrigger = Animator.StringToHash("Fall");
	}
}
