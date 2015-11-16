using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum PasswordInputType
{
	Numeric,
	Roman,
	Kanji,
	Symbols
}

public class PasswordManager : MonoSingleton<PasswordManager> {

	public GameObject passwordInputPanel;

	public GameObject keyButtonPrefab;
	public GameObject keysPanel;

	public GameObject confirmButton;
	public GameObject cancelButton;

	public GameObject passwordTextBox;

	private string passwordText;
	public string PasswordText {
		get {
			return this.passwordText;
		}
	}

	private string correctPassword;

	public PasswordInputType inputType;

	void Update()
	{
		if (Input.GetKeyDown("q")) {
			SetupPasswordInput(PasswordInputType.Roman, "AbCd");
		}
		if (Input.GetKeyDown("w")) {
			SetupPasswordInput(PasswordInputType.Numeric, "123");
		}
		if (Input.GetKeyDown("e")) {
			SetupPasswordInput(PasswordInputType.Kanji, "日月火水木金土");
		}
	}

	public void SetupPasswordInput(PasswordInputType type, string password)
	{
		correctPassword = password;

		if (passwordInputPanel.activeInHierarchy) {
			return;
		}

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
			char[] kanjis = "日月火水木金土".ToCharArray();
			foreach(char kanji in kanjis) {
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
		passwordText += key;
		UpdatePasswordString();
	}

	private void OnBackspaceKey()
	{
		if (passwordText.Length > 0) {
			passwordText = passwordText.Remove(passwordText.Length - 1);
			UpdatePasswordString();
		}
	}

	private void OnClickConfirm(string correctPassword)
	{
		if (correctPassword.ToUpper() == passwordText || correctPassword.ToLower() == passwordText) {
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
		
		UpdatePasswordString();
		RemoveKeys();
		
		cancelButton.GetComponent<Button>().onClick.RemoveAllListeners();
		confirmButton.GetComponent<Button>().onClick.RemoveAllListeners();
		
		passwordInputPanel.SetActive(false);
	}

	private void UpdatePasswordString()
	{
		passwordTextBox.GetComponent<InputField>().text = passwordText;
	}	

	private void PasswordSolved()
	{
		//TODO Give item, obviously
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
