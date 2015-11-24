using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLoader : MonoBehaviour {

	public GameObject LoadingScene;
	public Image LoadingBar;

	public void LoadScene(string sceneName)
	{
		StartCoroutine(SceneCoroutine("Scen"));
	}

	private IEnumerator SceneCoroutine(string sceneName)
	{
		LoadingScene.SetActive(true);
		AsyncOperation async = Application.LoadLevelAsync(sceneName);

		while (!async.isDone) {
			LoadingBar.fillAmount = async.progress / 0.9f;
			yield return null;
		}
	}
}
