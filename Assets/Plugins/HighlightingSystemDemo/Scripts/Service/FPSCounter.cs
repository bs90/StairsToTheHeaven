using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour
{
	private const float updateTime = 1f;

	private float frames = 0f;
	private float time = 0f;

	private string fps = "";

	// 
	void Update()
	{
		time += Time.deltaTime;
		if (time >= updateTime)
		{
			fps = "FPS: " + (frames / time).ToString("f2");
			time = 0f;
			frames = 0f;
		}
		frames++;
	}
}
