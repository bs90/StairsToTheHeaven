using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tooltip : MonoSingleton<Tooltip>  {
	private Item item;
	private string data;
	public GameObject tooltip;

	private void Start()
	{
		//TODO I hate Find
		if (tooltip == null) {
			tooltip = GameObject.Find("Tooltip");
			//TODO Disable this script if not found tooltip
		}
		tooltip.SetActive(false);
	}

	private void Update()
	{
		if (tooltip.activeSelf) {
			tooltip.transform.position = Input.mousePosition;
		}
	}

	public void Activate(Item item)
	{
		this.item = item;
		ConstructDataString();
		tooltip.SetActive(true);
	}

	public void Deactivate()
	{
//		this.item = item;
		tooltip.SetActive(false);
	}

	public void ConstructDataString()
	{
		data = "<color=#000000><b>" + item.Title + "</b></color>\n\n " + item.Description + "";
		tooltip.transform.GetChild(0).GetComponent<Text>().text = data;
	}
}
