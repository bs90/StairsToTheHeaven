using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chest : MonoBehaviour {
	public int chestID;
	public int rewardId;

	public bool opened;

	public PasswordInputType inputType;

	public List<int> spaces;

	public int limit;
	public string password;
	public string placeholder;
	public string instruction;
	public string rewardMessage;
	public char[] kanjiSet;

	private void Awake()
	{
		StartCoroutine(SetState());
	}

	private IEnumerator SetState()
	{
		yield return new WaitForSeconds(1);
		opened = DataManager.Instance.GetChestState(chestID);
//		Debug.Log ("Chest " + chestID + " open state is " + opened);
		if (opened) {
			this.gameObject.GetComponentInChildren<InteractableObject>().enabled = false;
			this.gameObject.SetActive(false);
		}
	}

	public void OnEventInvestigateChest ()
	{
		InterfaceManager.Instance.ToggleInfoWindow("It seems the chest is locked with a password, maybe you should try to unlock it.", InvestigateChest);
	}

	public void InvestigateChest ()
	{
		if (!opened) {
			switch (inputType) {
				case PasswordInputType.Numeric :
					PasswordManager.Instance.SetupPasswordInput(PasswordInputType.Numeric, password, placeholder, spaces, limit, rewardId, this.gameObject);
				break;
					
				case PasswordInputType.Roman :
					PasswordManager.Instance.SetupPasswordInput(PasswordInputType.Roman, password, placeholder, spaces, limit, rewardId, this.gameObject);
				break;
					
				case PasswordInputType.Kanji :
					if (kanjiSet != null) {
						PasswordManager.Instance.KanjiCharactersSet = kanjiSet;
					}
					else {
						PasswordManager.Instance.KanjiCharactersSet = "日月火水木金土".ToCharArray();
					}
					PasswordManager.Instance.SetupPasswordInput(PasswordInputType.Kanji, password, placeholder, spaces, limit, rewardId, this.gameObject);
				break;

				case PasswordInputType.Symbols :
					//TODO To be implemented if needed
				break;
			}
		}
	}

	public void OpenChest ()
	{
		if (!opened) {
			this.gameObject.GetComponent<Chest>().enabled = false;
			//TODO Repeated code, ugh
			this.gameObject.GetComponentInChildren<InteractableObject>().SetInteractAbility();
			opened = true;
			//TODO Save progress
			this.gameObject.GetComponent<Animation>()["ChestAnim"].speed = 1;
			this.gameObject.GetComponent<Animation>().Play("ChestAnim");
			DataManager.Instance.SaveChestState(chestID, true);
		}
	}

	public void TestParameter (int i) {

	}
}
