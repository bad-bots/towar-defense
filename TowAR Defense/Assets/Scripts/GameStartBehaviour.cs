using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartBehaviour : MonoBehaviour
{
  public GameObject gameControllerPrefab;

  // Start is called before the first frame update
  void Start()
  {
    NetworkManager.instance.StartGameEvent += OnStartGame;
  }

  private void OnStartGame(NetworkManager.PlayerJSON playerData)
  {
    Debug.Log("Starting Game");
    Debug.Log(JsonUtility.ToJson(playerData));
    var conObj = Instantiate(gameControllerPrefab);
    var gameCon = conObj.GetComponent<GameController>();
    gameCon.doubloons = playerData.doubloons;
    gameCon.isPlayer1 = playerData.playerNo == 1;

    Debug.Log("Switching Scenes...");
    GetComponent<SceneSwitcher>().SwitchScene();
  }
}
