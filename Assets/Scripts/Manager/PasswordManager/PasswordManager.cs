using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum PasswordInputType
{
	Numeric,
	Roman,
	Kanji,
	Symbols
}

public class PasswordManager : MonoSingleton<PasswordManager> {

	private int characterLimit = 20;

	public GameObject passwordInputPanel;

	public GameObject keyButtonPrefab;
	public GameObject keysPanel;

	public GameObject confirmButton;
	public GameObject cancelButton;

	public GameObject passwordField;
	public GameObject placeholderText;

	private string correctPassword;
	private List<int> passwordSpaces;

	private GameObject chestObject;
	public GameObject ChestObject {
		get {
			return this.chestObject;
		}
		set {
			this.chestObject = value;
		}
	}

	private string passwordText;
	public string PasswordText {
		get {
			return this.passwordText;
		}
	}
			
	private char[] kanjiCharactersSet = "日月火水木金土".ToCharArray();
	public char[] KanjiCharactersSet {
		get {
			return this.kanjiCharactersSet;
		}
		set {
			this.kanjiCharactersSet = value;
		}
	}

	private int rewardedItem = -1;

	public void SetupPasswordInput(PasswordInputType type, string password, string placeholder, List<int> spaces, int limit, int itemId, GameObject chest)
	{
		if (passwordInputPanel.activeInHierarchy) {
			return;
		}

		if (password == null) {
			return;
		}
		correctPassword = password;

		if (placeholder != null) {
			placeholderText.GetComponent<Text>().text = placeholder;
		}

		if (spaces.Count != 0) {
			passwordSpaces = spaces;
		}
		else {
			passwordSpaces = new List<int>();
			if (type == PasswordInputType.Kanji) {
				for (int i = password.IndexOf('　'); i > -1; i = password.IndexOf('　', i + 1)){
					passwordSpaces.Add(i);
				}
			}
			else {
				for (int i = password.IndexOf(' '); i > -1; i = password.IndexOf(' ', i + 1)){
					passwordSpaces.Add(i);
				}
			}
		}

		if (limit != 0) {
			characterLimit = limit;
		}
		else {
			characterLimit = password.Length;
		}

		if (itemId != -1) {
			rewardedItem = itemId;
		}

		if (chest != null) {
			chestObject = chest;
		}

		passwordField.GetComponent<InputField>().characterLimit = characterLimit;
		passwordInputPanel.SetActive(true);
		GameManager.Instance.SetGameState(GameState.Password);
		RemoveKeys();

		if (InterfaceManager.Instance.InventoryShowing) {
			InterfaceManager.Instance.ToggleInventoryWindow();
		}

		switch (type)
		{
		case PasswordInputType.Roman:
			for (char c = 'A'; c <= 'Z'; c++) {
				string character = c.ToString();
				SetupKey(character, keysPanel);
			}
			break;
		case PasswordInputType.Numeric:
			for (int i = 0; i <= 9; i++) {
				string character = i.ToString();
				SetupKey(character, keysPanel);
			}
			break;
		case PasswordInputType.Kanji:
			foreach(char kanji in kanjiCharactersSet) {
				string character = kanji.ToString();
				SetupKey(character, keysPanel);
			}
			break;
		}
		
		GameObject backspaceKey = Instantiate(keyButtonPrefab) as GameObject;
		backspaceKey.GetComponentInChildren<Text>().text = "←";
		backspaceKey.name = "Backspace";
		backspaceKey.transform.SetParent(keysPanel.transform);
		Button backspaceButton = backspaceKey.GetComponent<Button>();
		backspaceButton.onClick.AddListener(OnBackspaceKey);

		cancelButton.GetComponent<Button>().onClick.AddListener(()=> OnClickCancel(false));
		confirmButton.GetComponent<Button>().onClick.AddListener(()=> OnClickConfirm(correctPassword));
	}

	private void SetupKey(string keyword, GameObject parent)
	{
		GameObject key = Instantiate(keyButtonPrefab) as GameObject;
		key.name = keyword;
		key.GetComponentInChildren<Text>().text = keyword;
		key.transform.SetParent(parent.transform);
		Button button = key.GetComponent<Button>();
		button.onClick.AddListener(() => OnPressKey(keyword));
	}

	private void OnPressKey(string key)
	{
		if (passwordText != null && passwordSpaces != null && passwordSpaces.Contains(passwordText.Length)) {
			passwordText += " ";
		}
		passwordText += key;
		if (passwordText.Length <= characterLimit) {
			UpdatePasswordString();
		}
		else {
			passwordText = passwordText.Remove(passwordText.Length - 1);
		}
	}

	private void OnBackspaceKey()
	{
		if (passwordText.Length > 0) {
			passwordText = passwordText.Remove(passwordText.Length - 1);
			if(passwordText.EndsWith(" ")) {
				passwordText = passwordText.Remove(passwordText.Length - 1);
			}
			UpdatePasswordString();
		}
	}

	private void OnClickConfirm(string correctPassword)
	{
		if (correctPassword == passwordText || correctPassword.ToUpper() == passwordText || correctPassword.ToLower() == passwordText) {
			InterfaceManager.Instance.ToggleInfoWindow(string.Format("Password " + correctPassword + " accepted."), PasswordSolved);
		}
		else {
			InterfaceManager.Instance.ToggleInfoWindow(string.Format("Password incorrect."), ReturnToPasswordState);
		}
	}

	private void ReturnToPasswordState()
	{
		GameManager.Instance.SetGameState(GameState.Password);
	}

	private void OnClickCancel(bool passwordCompleted)
	{
		passwordText = string.Empty;
		correctPassword = string.Empty;
		characterLimit = 15;
		if (passwordSpaces != null) {
			passwordSpaces.Clear();
		}

		UpdatePasswordString();
		RemoveKeys();
		
		cancelButton.GetComponent<Button>().onClick.RemoveAllListeners();
		confirmButton.GetComponent<Button>().onClick.RemoveAllListeners();

		placeholderText.GetComponent<Text>().text = string.Empty;

		if (!passwordCompleted) {
			GameManager.Instance.SetGameState(GameState.Investigation);
			rewardedItem = -1;
		}
		passwordInputPanel.SetActive(false);
	}

	private void OnPasswordFieldUpdate()
	{

	}

	private void UpdatePasswordString()
	{
		passwordField.GetComponent<InputField>().text = passwordText;
	}	

	private void PasswordSolved()
	{
		if (rewardedItem != -1) {
			GameManager.Instance.SetGameState(GameState.Uncontrolable);
			StartCoroutine(GiveReward());
			if (chestObject != null) {
				chestObject.GetComponent<Chest>().OpenChest();
				chestObject = null;
			}
		}
		OnClickCancel(true);
	}

	private IEnumerator GiveReward ()
	{
		yield return new WaitForSeconds(3);
		InterfaceManager.Instance.ToggleInfoWindow(string.Format("You picked up <color=yellow>" + ItemDatabase.Instance.FetchItemByID(rewardedItem).Title + "</color>."), 
		                                           null);
		InventoryManager.Instance.AddItem(rewardedItem, 1);
		GameManager.Instance.SetGameState(GameState.Investigation);
		rewardedItem = -1;
	}

	private void RemoveKeys()
	{
		if (keysPanel.transform.childCount != 0) {
			for (int i = 0; i < keysPanel.transform.childCount; i++) {
				//TODO Pooling, recycling, make this environment stress free
				Destroy(keysPanel.transform.GetChild(i).gameObject);
			}
		}
	}
}
