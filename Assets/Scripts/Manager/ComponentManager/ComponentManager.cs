using UnityEngine;
using System.Collections;

public class ComponentManager : MonoSingleton<ComponentManager> {

	void OnEnable () 
	{
		ComponentManager[] managers = (ComponentManager[])FindObjectsOfType(typeof(ComponentManager));
		foreach(ComponentManager manager in managers) {
			if (manager != this.gameObject.GetComponent<ComponentManager>()) {
				Destroy(this.gameObject);
			}
		}
		this.gameObject.name = "Super Important";
		DontDestroyOnLoad(this.gameObject);
	}
}
