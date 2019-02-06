using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndBehavior : MonoBehaviour
{
    public SceneField joinGameScene;

    void Start()
    {
        NetworkManager.instance.EndGameEvent += OnEndGame;
    }

    private void OnEndGame(NetworkManager.WinningPlayer gameResult)
    {
        PersistantStats.winningPlayer = gameResult.winningPlayer;

        SceneManager.sceneLoaded += OnSceneLoad;
        UnityEngine.SceneManagement.SceneManager.LoadScene(joinGameScene);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        var mainPage = GameObject.FindGameObjectWithTag("MainPage");
        mainPage.SetActive(false);
    }
}
