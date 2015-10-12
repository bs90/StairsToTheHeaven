using UnityEngine;
using System.Collections;

public class TestMessage : MonoBehaviour {
	
	void Update () {
		if (Input.GetKeyDown("x") ) {
			MessageManager.Instance.DisplayMessages(Constants.MF_A01, CallEnd);
		}
		if (Input.GetKeyDown("z") ) {
			MessageManager.Instance.RemoveMessages();
		}
	}

	void CallEnd()
	{
		Debug.Log ("End");
	}
}
