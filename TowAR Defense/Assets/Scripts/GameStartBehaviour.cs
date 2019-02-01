using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartBehaviour : MonoBehaviour
{
  public SceneField gameScene;

  private NetworkManager.PlayerJSON playerData;

  // Start is called before the first frame update
  void Start()
  {
    NetworkManager.instance.StartGameEvent += OnStartGame;
  }

  private void OnStartGame(NetworkManager.PlayerJSON _playerData)
  {
    Debug.Log("Starting Game");
    Debug.Log(JsonUtility.ToJson(_playerData));
    playerData = _playerData;

    Debug.Log("Switching Scenes...");
    SceneManager.sceneLoaded += OnSceneLoad;
    UnityEngine.SceneManagement.SceneManager.LoadScene(gameScene);
  }

  private void OnSceneLoad(Scene scene, LoadSceneMode mode)
  {
    SceneManager.sceneLoaded -= OnSceneLoad;
    var gameCon = GameObject.FindGameObjectWithTag("GameController")
          .GetComponent<GameController>();

    Debug.Log("Initializing game controller");
    gameCon.Initialize(playerData);
  }
}
