using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartBehaviour : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    NetworkManager.instance.StartGameEvent += OnStartGame;
  }

  private void OnStartGame()
  {
    GetComponent<SceneSwitcher>().SwitchScene();
  }
}
