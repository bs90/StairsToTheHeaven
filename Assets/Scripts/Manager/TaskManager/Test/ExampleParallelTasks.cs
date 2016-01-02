using UnityEngine;
using System.Collections;
using Svelto.Tasks;

public class ExampleParallelTasks : MonoBehaviour 
{
	int i;
	
	bool variableThatCouldHaveBeenUseful;
	
	// Use this for initialization
	void Start () 
	{
//		Application.targetFrameRate = 60;
//		
//		ParallelTaskCollection pt = new ParallelTaskCollection();
//		SerialTaskCollection	st = new SerialTaskCollection();
//		
//		pt.Add(Print("s1"));
//		pt.Add(Print("s2"));
//		pt.Add(Print("s3"));
//		pt.onComplete += Cool;
//
//		StartCoroutine(pt.GetEnumerator());

//		pt.Add(Print("1"));
//		pt.Add(Print("2"));
//		pt.Add(Print("3"));
//		pt.Add(Print("4"));
//		pt.Add(Print("5"));
//		pt.Add(st);
//		pt.Add(Print("6"));
//		pt.Add(WWWTest ());
//		pt.Add(Print("7"));
//		pt.Add(Print("8"));
			
//		StartCoroutine(pt.GetEnumerator());
	}

	void Cool()
	{
		Debug.Log ("Done");
	}

	IEnumerator Print(string i)
	{
		Debug.Log(i);
		yield return new WaitForSeconds(1);
	}
	
	IEnumerator DoSomethingAsynchonously()  //this can be awfully slow, I suppose it is synched with the frame rate
	{
		for (i = 0; i < 10; i++)
	        yield return i;
		
		Debug.Log("index " + i);
	}
	
	IEnumerator WWWTest()
	{
		WWW www = new WWW("www.google.com");
		
		yield return www;
		
		Debug.Log("www done:" + www.text);
	}
}