using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public class MessageManager : MonoSingleton<MessageManager> {

	public delegate void OnMessageEvent();

	public Text messageTextBox;
	public Text nameTextBox;
	private bool messageOver = true;

	[Range(1, 20)]
	public int messageSpeed;
	private int messageFrameSkipped;

	private bool allowMessageSkip;
	private bool messageFlowFinish;

	public List<DialogMessage> messagesList;
	private string currentMessage;
	private string displayMessage;
	private string currentName;
	private int messageIndex;
	private int messageCharacterIndex;

	Job messageDisplayJob;

	private void OnEnable()
	{
		if (!messageTextBox) {
			if (GameObject.Find(Constants.MES_TEXT_BOX)) {
				messageTextBox = GameObject.Find(Constants.MES_TEXT_BOX).GetComponent<Text>();
			}
			else {
				Debug.LogError("What the fuck? Setup the scene first if you want to display messages!");
				Destroy(this.gameObject);
			}
		}
		if (!nameTextBox) {
			if (GameObject.Find(Constants.MES_NAME_BOX)) {
				nameTextBox = GameObject.Find(Constants.MES_NAME_BOX).GetComponent<Text>();
			}
		}
		// TODO: Read message speed from config
		messageSpeed = 5;
	}

	private void OnDisable()
	{
		Destroy(this);
	}

	private void CheckTextEnabled()
	{
		messageOver = false;
		if (!messageTextBox.enabled) {
			messageTextBox.enabled = true;
		}

		if (!nameTextBox.enabled) {
			nameTextBox.enabled = true;
		}
	}

	private void CreateList(string filePath) {
		messagesList = new List<DialogMessage>();
		ResetMessageFrameCounter();
		ReadMessagesFromCSV (filePath);
	}
	
	private void ReadMessagesFromCSV (string messageFileName) 
	{
		List<Dictionary<string,object>> data = CSVReader.Read(messageFileName);
		for (int i = Constants.ZERO; i < data.Count; i++) {
			if (Application.loadedLevelName == (string)data[i][Constants.MES_SCENE]) {
				messagesList.Add( new DialogMessage() {
					Character 	= (string)data[i][Constants.MES_CHARACTER],
					Skipable 	= bool.Parse((string)data[i][Constants.MES_SKIPABLE]),
					Voiced 		= bool.Parse((string)data[i][Constants.MES_VOICED]),
					Text 		= (string)data[i][Constants.MES_TEXT],
				});
			}
			else {
				return;
			}
		}
	}

	private void MessageDisplay() {
		if (!messageFlowFinish) {
			messageFrameSkipped += messageSpeed;
			if (messageFrameSkipped > Constants.MES_DISPLAY_COUNTER)
				messageFrameSkipped = Constants.MES_DISPLAY_COUNTER;
			
			if (messageFrameSkipped == Constants.MES_DISPLAY_COUNTER) {
				currentMessage = (string)messagesList[messageIndex].Text;
				currentName = (string)messagesList[messageIndex].Character;
				displayMessage = currentMessage.Substring(Constants.ZERO, messageCharacterIndex);
				if (messageCharacterIndex < currentMessage.Length)
					messageCharacterIndex++;
				ResetMessageFrameCounter();
			}
		}
		messageTextBox.text = displayMessage;
		if (nameTextBox) {
			nameTextBox.text = currentName;
		}
	}

	private void ResetMessageFrameCounter()
	{
		messageFrameSkipped = Constants.ZERO;
	}
	
	private void ResetMessageIndex() 
	{
		messageCharacterIndex = Constants.ZERO;
		messageFlowFinish = false;
	}

	private void DisplayFullMessage() {
		messageFlowFinish = true;	
		messageCharacterIndex = currentMessage.Length;
		displayMessage = currentMessage;
	}

	private void OnMessageClick()
	{
		if (Input.touchCount > Constants.ZERO) {
			GetNextMessage();
		}
		else if (Input.GetMouseButtonDown(Constants.ZERO)) {
			GetNextMessage();
		}
	}

	private void GetNextMessage() {
		if (displayMessage.Length == currentMessage.Length) {
			if (messageIndex + Constants.ONE < messagesList.Count){
				if (messageIndex + Constants.ONE < messagesList.Count) {
					messageIndex++;
				}
				ResetMessageFrameCounter();
				ResetMessageIndex();
			}
			else {
				DisableMessage();
			}
		}
		else {
			DisplayFullMessage();
		}
	}

	private void DisableMessage() 
	{
		messageOver = true;

		currentMessage = "";
		displayMessage = "";
		currentName = "";

		messageTextBox.text = "";
		nameTextBox.text = "";

		messageTextBox.enabled = false;
		nameTextBox.enabled = false;

		messageIndex = 0;
		messageCharacterIndex = 0;

		messageFlowFinish = false;

		messagesList.Clear();
	}

	public void DisplayMessages (string filePath, OnMessageEvent endMessage)
	{
		if (!messageOver) {
			Debug.LogError("The messages are still running, idiot!");
			return;
		}
		CheckTextEnabled();
		CreateList(filePath);
		messageDisplayJob = Job.Create(ReadMessages());
		messageDisplayJob.jobCompleted += (obj) => {
			endMessage();
		};
	}
	
	public void RemoveMessages ()
	{
		if (messageDisplayJob != null) {
			messageDisplayJob.KillJob();
			DisableMessage();
		}
		else {
			Debug.LogError("You haven't even started the job, mother fucker!");
		}
	}

	IEnumerator ReadMessages()
	{
		while(!messageOver) {
			MessageDisplay();
			OnMessageClick();
			yield return null;
		}
	}
}
