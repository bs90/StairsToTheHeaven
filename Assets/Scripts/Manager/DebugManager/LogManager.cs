using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LogManager : MonoSingleton<LogManager> {
	public GameObject debugPanel;

	public GameObject logPanel;
	public GameObject logHistoryText;
	public GameObject logCountText;
	public GameObject logObject;
	
	public GameObject clearButton;
	public GameObject commandField;
	public GameObject commandEnterButton;

	public Sprite assertIcon;
	public Sprite errorIcon;
	public Sprite exceptionIcon;
	public Sprite logIcon;
	public Sprite warningIcon;

	public int logLimit;

	private List<GameObject> logObjects = new List<GameObject>();
	private List<string> logStrings = new List<string>(); 

	private GameState previousState = GameState.Investigation;

	private string logText;
	public string LogText {
		get {
			return this.logText;
		}
	}

	private struct Log
	{
		public string message;
		public string stackTrace;
		public LogType type;
	}

	private void OnEnable ()
	{
		Application.logMessageReceived += HandleLog;
	}
	
	private void OnDisable ()
	{
		Application.logMessageReceived -= HandleLog;
	}
	
	private void HandleLog (string message, string stackTrace, LogType type)
	{
		WriteLog(message, type, stackTrace);
	}
	
	private void Update () 
	{
		if (Input.GetKeyUp("tab")) {
			ToggleLog();
		}
	}

	//TODO Haha despite the fact I wrote this shit for 4 hours it became useless, revert to using 1 string, faster.
	private void OnLogEntry (string message, LogType type)
	{
		if (logObjects.Count < logLimit) {
			GameObject newLog = Instantiate(logObject, logObject.transform.position, logObject.transform.rotation) as GameObject;
			logObjects.Add(newLog);
			newLog.transform.SetParent(logPanel.transform, false);

			newLog.name = "Log" + logObjects.Count;
			newLog.GetComponentInChildren<Text>().text = message;
			RectTransform newRect = newLog.GetComponent<RectTransform>();

			if (logObjects.Count > 1) {
				newRect.anchoredPosition = new Vector2(newRect.anchoredPosition.x, newRect.anchoredPosition.y - 60 * (logObjects.Count - 1));
			}

			RectTransform panelRect = logPanel.GetComponent<RectTransform>();
			panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, 100 + (50 * logObjects.Count) + (10 * logObjects.Count));

			if (logObjects.Count > 1) {
				for (int i = logObjects.Count - 1; i > 0; i--) {
					logObjects[i].GetComponentInChildren<Text>().text = logObjects[i - 1].GetComponentInChildren<Text>().text;
					logObjects[i].transform.GetChild(0).gameObject.GetComponentInChildren<Image>().sprite = 
						logObjects[i - 1].transform.GetChild(0).gameObject.GetComponentInChildren<Image>().sprite;
				}
			}
		}

		switch (type) {
			case LogType.Assert: {
				logObjects[0].transform.GetChild(0).gameObject.GetComponentInChildren<Image>().sprite = assertIcon;
				break;
			}
			case LogType.Error: {
				logObjects[0].transform.GetChild(0).gameObject.GetComponentInChildren<Image>().sprite = errorIcon;
				break;
			}
			case LogType.Exception: {
				logObjects[0].transform.GetChild(0).gameObject.GetComponentInChildren<Image>().sprite = exceptionIcon;
				break;
			}
			case LogType.Log: {
				logObjects[0].transform.GetChild(0).gameObject.GetComponentInChildren<Image>().sprite = logIcon;
				break;
			}
			case LogType.Warning: {
				logObjects[0].transform.GetChild(0).gameObject.GetComponentInChildren<Image>().sprite = warningIcon;
				break;
			}
		}
		logObjects[0].GetComponentInChildren<Text>().text = message;
	}

	private void WriteLog(string message, LogType type, string stackTrace)
	{
		if (logStrings.Count == logLimit) {
			logStrings.RemoveAt(0);
		}

		string logType = "";
		switch (type) {
			case LogType.Assert: {
				logType = "<b>Log type</b>: <color=blue>Assert</color> \n";
				break;
			}
			case LogType.Error: {
				logType = "<b>Log type</b>: <color=red>Error</color> \n";
				break;
			}
			case LogType.Exception: {
				logType = "<b>Log type</b>: <color=orange>Exception</color> \n";
				break;
			}
			case LogType.Log: {
				logType = "<b>Log type</b>: <color=white>Log</color> \n";
				break;
			}
			case LogType.Warning: {
				logType = "<b>Log type</b>: <color=yellow>Warning</color> \n";
				break;
			}
		}

		string logTrace = "";
		if (stackTrace != string.Empty) {
			logTrace = "<b>Trace</b>: " + stackTrace + "\n";
		}
		else {
			logTrace = "<b>Trace</b>: " + "No stack tracing during runtime." + "\n";
		}

		string logDate = "<b>DateTime</b>: " + System.DateTime.Now.ToString() + "\n";
		string logMessage = logType + logDate + "<b>Message</b>: " + message + "\n" + logTrace + "\n";

		logStrings.Add(logMessage);

		CreateLogText();
		if (debugPanel.activeInHierarchy) {
			AssignText();
		}
	}

	private static int GetLineBreakCount(string s)
	{
		int n = 0;
		foreach( var c in s ) {
			if ( c == '\n' ) n++;
		}
		return n + 1;
	}

	private void CreateLogText()
	{
		logText = "";
		if (logStrings.Count != 0) {
			foreach(string logLine in logStrings) {
				logText += logLine;
			}
		}
	}

	private void AssignText()
	{
		logHistoryText.GetComponent<Text>().text = logText;
		if (logCountText) {
			logCountText.GetComponent<Text>().text = "Log Count: " + logStrings.Count.ToString();
		}

		int lineNumber = GetLineBreakCount(logText);
		RectTransform panelRect = logPanel.GetComponent<RectTransform>();
		panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, (15 * lineNumber) + (10 * logStrings.Count) + 35);
	}

	public void ToggleLog () 
	{
		if (debugPanel.activeInHierarchy) {
			GameManager.Instance.SetGameState(previousState);
			debugPanel.SetActive(false);
		}
		else {
			previousState = GameManager.Instance.State;
			GameManager.Instance.SetGameState(GameState.Debugging);

			debugPanel.SetActive(true);
			commandField.GetComponent<InputField>().Select();
			CreateLogText();
			AssignText();
		}
	}

	public void ClearLogs ()
	{
		logStrings.Clear();
		CreateLogText();
		if (debugPanel.activeInHierarchy) {
			AssignText();
		}
	}

	public void EnterCommand ()
	{
		string command = commandField.GetComponent<InputField>().text;
		if (command == "exit") {
			ToggleLog();
		}
		if (command == "quit") {
			Application.Quit();
		}
		if (command == "give control") {
			previousState = GameState.Investigation;
			ToggleLog();
		}
		if (command == "stream path") {
			Debug.Log(Application.streamingAssetsPath);
		}
		if (command == "load f2") {
			ToggleLog();
			GameManager.Instance.LoadScene("F2");
		}
		if (command == "load b10") {
			ToggleLog();
			GameManager.Instance.LoadScene("B10");
		}
		commandField.GetComponent<InputField>().Select();
	}
}
