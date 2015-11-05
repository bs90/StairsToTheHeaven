using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {
	public  float updateInterval = 0.5F;
	private string fpsString;
	private float accum = 0; 
	private float timeLeft; 
	private int frames = 0; 

	public GameObject fpsCounterText;

	void Start () {
		fpsString = string.Format("FPS: " );
	}
	
	void Update () {
		timeLeft -= Time.fixedDeltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames += 1;
		
		if( timeLeft <= 0.0 ) {
			float fps = accum/frames;
			fpsString = System.String.Format("FPS: " + (int)fps);
			timeLeft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
		fpsCounterText.GetComponent<Text>().text = string.Format(fpsString);
	}	
}
