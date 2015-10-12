using UnityEngine;
using System.Collections;

// Class that turns every class inherit from it to a Singleton

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
	
	protected static T instance;
	
	static public bool isActive { 
		get { 
			return instance != null; 
		} 
	}
	
	public static T Instance 
	{
		get {
			if(instance == null) {
				instance = (T) FindObjectOfType(typeof(T));
				
				if (instance == null) {
					Debug.LogError("An instance of " + typeof(T) + 
					               " is needed in the scene, but there is none, creating one.");
					GameObject go = new GameObject(typeof(T).Name);
					DontDestroyOnLoad(go);
					instance = go.AddComponent<T>();
				}
			}
			return instance;
		}
	}

	void OnApplicationQuit()
	{
		instance = null;
	}
}