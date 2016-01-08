using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Globalization;
using LitJson;

public enum ApiPost
{
	send_phone,
	register,
	verified,
	login,
	userInfo,
	forget_password,
	sit_down,
	buyin,
}

public enum ApiGet
{
	table_types,
	info
}

public class GameConfig
{
	public static string SERVER_URL = "http://gs-poker.framgia.vn/api/v1/";
	public static string SERVER_URL_USER = SERVER_URL + "users/";
	public static string SERVER_URL_TABLE = SERVER_URL + "tables/";
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="levelName"></param>
	/// <returns></returns>
	public static IEnumerator LoadALevel(string levelName)
	{
		
		switch(levelName)
		{
		case "GameScene":
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			break;
		default:
			Screen.orientation = ScreenOrientation.Portrait;
			break;
		}
		
//		Loading.Instance.ShowWindow();
		yield return Application.LoadLevelAsync(levelName);
//		Loading.Instance.HideWindow();
	}
	
	public static string ConvertNumber(double number)
	{
		return String.Format(CultureInfo.InvariantCulture,
		                     "{0:0,0}", number);
	}
	
	public static string RandomString(int size)
	{
		StringBuilder builder = new StringBuilder();
		System.Random random = new System.Random();
		char ch;
		for (int i = 0; i < size; i++)
		{
			ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
			builder.Append(ch);
		}
		return builder.ToString();
	}
}

public class ServerApi : MonoSingleton<ServerApi>
{
	
	protected ServerApi() { }

	public WWW GET(string url, WWWForm form, ApiGet api)
	{
		url += ("?" + System.Text.Encoding.UTF8.GetString(form.data));
		Debug.Log("url = " + url);
		WWW www = new WWW(url);
		StartCoroutine(WaitForGet(www, api));
		return www;
	}
	
	/// <summary>
	/// "{\"user_name\":\"asadfasd\",\"password_confirmation\":\"asadfasd\" ,\"password\":\"asadfasd\",\"phone_number\":\"0912345ds678\",\"gender\":\"0\"}"
	/// </summary>
	/// <returns></returns>
	public WWW POST(string url, WWWForm form, ApiPost api)
	{
		Debug.Log("url = " + url);
		//JSONObject jsObjectUser = new JSONObject(param);
		
		WWW www = new WWW(url, form);
		Debug.Log(System.Text.Encoding.UTF8.GetString(form.data));
		
		StartCoroutine(WaitForPost(www, api));
		return www;
	}
	
	private IEnumerator WaitForPost(WWW www, ApiPost api)
	{
		Debug.Log("ShowWindow");
//		Loading.Instance.ShowWindow();
		yield return www;
//		Loading.Instance.HideWindow();
		Debug.Log("HideWindow");
		// check for errors
		if (www.error == null)
		{
			Debug.Log(www.text);

		}
		else
		{
			Debug.Log(www.error + "\n" + www.text);
			JsonData jsData = JsonMapper.ToObject(www.text);
			if(jsData["error"] == null)
			{
//				DialogMessage.Instance.ShowWindow("Error", www.error);
			}
			else
			{
//				DialogMessage.Instance.ShowWindow("Error", (string)jsData["error"]);
			}
		}
	}
	
	private IEnumerator WaitForGet(WWW www, ApiGet api)
	{
		Debug.Log("ShowWindow WaitForGet");
//		Loading.Instance.ShowWindow();
		yield return www;
//		Loading.Instance.HideWindow();
		Debug.Log("HideWindow WaitForGet");
		// check for errors
		if (www.error == null)
		{
			Debug.Log(www.text);
			switch (api)
			{
			case ApiGet.info:
//				GameController.Instance.LoadTableData(www.text);
				break;
			}
		}
		else
		{
			Debug.Log(www.text);
			JsonData jsData = JsonMapper.ToObject(www.text);
			if (jsData["error"] == null)
			{
//				DialogMessage.Instance.ShowWindow("Error", www.error);
			}
			else
			{
//				DialogMessage.Instance.ShowWindow("Error", (string)jsData["error"]);
			}
		}
	}
	
	public bool ConnectToServer()
	{
		return false;
	}
	
	/// <summary>
	/// string phone_number
	/// </summary>
	public void RegistrationPhone(string phone_number)
	{
		WWWForm form = new WWWForm();
		form.AddField("phone_number", phone_number.Trim());
		ServerApi.Instance.POST(GameConfig.SERVER_URL_USER + ApiPost.send_phone, form, ApiPost.send_phone);
	}
	
	/// <summary>
	/// string code
	/// </summary>
	public void VerificationCode(string code)
	{
		WWWForm form = new WWWForm();
		form.AddField("code", code.Trim());
		ServerApi.Instance.POST(GameConfig.SERVER_URL_USER + ApiPost.verified, form, ApiPost.verified);
	}
	
	/// <summary>
	/// string userName, string password, string password_confirmation, string phone_number, string birthday, bool gender
	/// </summary>
	public void RegistrationUserInfo(string userName, string password, string password_confirmation, string phone_number, string birthday, bool gender)
	{
		WWWForm form = new WWWForm();
		form.AddField("user[user_name]", userName);
		form.AddField("user[password]", password.Trim());
		form.AddField("user[password_confirmation]", password_confirmation.Trim());
		form.AddField("user[phone_number]", phone_number.Trim());
		form.AddField("user[gender]", gender ? 1 : 0);
		form.AddField("user[birth_date]", birthday);
		ServerApi.Instance.POST(GameConfig.SERVER_URL_USER + ApiPost.register, form, ApiPost.register);
	}
	
	/// <summary>
	/// string phone_number, string password
	/// </summary>
	public void Login(string phone_number, string password)
	{
		WWWForm form = new WWWForm();
		form.AddField("login", phone_number.Trim());
		form.AddField("password", password.Trim());
		ServerApi.Instance.POST(GameConfig.SERVER_URL_USER + ApiPost.login, form, ApiPost.login);
	}
}

public class DataTemp
{
	//public static bool SaveData(JSONObject data)
	//{
	//    PlayerPrefs.SetString("Data", data.Print());
	//    PlayerPrefs.Save();
	//    return true;
	//}
	
	//public static bool LoginCheck(string userName, string password)
	//{
	//    JSONObject jsData =  new JSONObject(PlayerPrefs.GetString("Data", ""));
	//    JSONObject jsUserInfo = jsData.GetField("userInfo");
	//    if (userName.ToLower().Equals(jsUserInfo.GetField("userName").str) && password.Equals(jsUserInfo.GetField("password").str))
	//    {
	//        return true;
	//    }
	//    return false;
	//}
	
	//public static bool RegisUser(JSONObject data)
	//{
	//    JSONObject jsData = new JSONObject(PlayerPrefs.GetString("Data", ""));
	//    if (jsData.GetField("userInfo")!= null && jsData.GetField("userInfo").GetField("userName") != null && data.GetField("userName").Equals(jsData.GetField("userInfo").GetField("userName")))
	//    {
	//        return false;
	//    } else
	//    {
	//        jsData.SetField("userInfo", data);
	//        PlayerPrefs.SetString("Data", jsData.Print());
	//        PlayerPrefs.Save();
	//    }
	//    return true;
	//}
}
