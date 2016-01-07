using UnityEngine;
using System.Collections;

public enum RemotePanelColors {
	red = 0,
	green = 1,
	white = 2,
	purple = 3,
	blue = 4,
	orange = 5,
	yellow = 6
}

public class RemotePanel : MonoBehaviour {
	public Transform part0;	
	public Transform part1;	
	public Transform part2;	
	public Transform part3;	
	public Transform part4;	
	public Transform part5;	
	public Transform part6;

	public Material redMat;
	public Material greenMat;
	public Material whiteMat;
	public Material purpleMat;
	public Material blueMat;
	public Material orangeMat;
	public Material yellowMat;
	public Material offMat;

	private void SetupColors()
	{

	}
}
