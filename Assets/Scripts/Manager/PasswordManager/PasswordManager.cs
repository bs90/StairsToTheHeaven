﻿using UnityEngine;
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
		
	void Update()
	{
		if (Input.GetKeyDown("q")) {
			SetupPasswordInput(PasswordInputType.Roman, "I am eternal", "Password is I am internal.", new List<int> {1, 4}, 13, -1);
		}
		if (Input.GetKeyDown("w")) {
			SetupPasswordInput(PasswordInputType.Numeric, "123", "Example numeric characters password test.", null, 3, 4);
		}
		if (Input.GetKeyDown("e")) {
			SetupPasswordInput(PasswordInputType.Kanji, "日月火水木金土", "Example kanji characters password test.", null, 7, -1);
		}
	}

	public void SetupPasswordInput(PasswordInputType type, string password, string placeholder, List<int> spaces, int limit, int itemId)
	{
		if (passwordInputPanel.activeInHierarchy) {
			return;
		}

		if (password == null) {
			return;
		}
		correctPassword = password;

		if (spaces != null) {
			passwordSpaces = spaces;
		}

		if (placeholder != null) {
			placeholderText.GetComponent<Text>().text = placeholder;
		}

		if (limit != 0) {
			characterLimit = limit;
		}

		if (itemId != -1) {
			rewardedItem = 4;
		}
		else {
			rewardedItem = -1;
		}

		passwordField.GetComponent<InputField>().characterLimit = characterLimit;
		passwordInputPanel.SetActive(true);
		RemoveKeys();

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

		cancelButton.GetComponent<Button>().onClick.AddListener(OnClickCancel);
		confirmButton.GetComponent<Button>().onClick.AddListener(() => OnClickConfirm(correctPassword));
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
			InterfaceManager.Instance.ToggleInfoWindow(string.Format("Password incorrect."), null);
		}
	}

	private void OnClickCancel()
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
			InterfaceManager.Instance.ToggleInfoWindow(string.Format("You picked up <color=yellow>" + ItemDatabase.Instance.FetchItemByID(rewardedItem).Title + "</color>."), 
			                                           ()=> InventoryManager.Instance.AddItem(rewardedItem, 1));
		}
		OnClickCancel();
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