using UnityEngine;
using System.Collections;

public class Clock : MonoBehaviour {

	//-- set start time 00:00
//    public int minutes = 0;
//    public int hour = 0;
//	public int second = 0;
	public bool realTime;
	
	public GameObject pointerSeconds;
    public GameObject pointerMinutes;
    public GameObject pointerHours;
    
    //-- time speed factor
    public float clockSpeed = 1.0f;     // 1.0f = realtime, < 1.0f = slower, > 1.0f = faster

    //-- internal vars
//    int seconds;
//    float msecs;

void Start() 
{
//    msecs = 0.0f;
//    seconds = 0;
	//-- set real time
//	if (realTime)
//	{
//		hour=System.DateTime.Now.Hour;
//		minutes=System.DateTime.Now.Minute;
//		second=System.DateTime.Now.Second;
//	}

}

void Update() 
{
    //-- calculate pointer angles
    float rotationSeconds = (360.0f / 60.0f)  * GameManager.Instance.second;
    float rotationMinutes = (360.0f / 60.0f)  * GameManager.Instance.minutes;
	float rotationHours   = ((360.0f / 12.0f) * GameManager.Instance.hour) + ((360.0f / (60.0f * 12.0f)) * GameManager.Instance.minutes);

    //-- draw pointers
    pointerSeconds.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationSeconds);
    pointerMinutes.transform.localEulerAngles = new Vector3(0.0f, 0.0f, rotationMinutes);
    pointerHours.transform.localEulerAngles   = new Vector3(0.0f, 0.0f, rotationHours);

}
}
