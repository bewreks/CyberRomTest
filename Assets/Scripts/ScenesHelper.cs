using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class ScenesHelper
{
	public const string StartSceneName = "StartScene";
	public const string MainSceneName  = "MainScene";
	public const string EndSceneName   = "EndScene";


	public static void LoadScene(string sceneName)
	{
		if (NetworkManager.Singleton.IsListening)
		{
			NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
		}
		else
		{
			SceneManager.LoadScene(sceneName);
		}
	}
}