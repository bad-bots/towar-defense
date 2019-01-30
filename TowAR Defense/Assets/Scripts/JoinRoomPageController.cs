using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomPageController : MonoBehaviour
{
  public InputField roomCodeInput;
  public Text wrongCodeText;

  void Start()
  {
    NetworkManager.instance.StartGameEvent += OnStartGame;
    NetworkManager.instance.IncorrectRoomCodeEvent += OnIncorrectGameCode;
  }

  public void HandleClick()
  {
    string roomCode = roomCodeInput.text;
    NetworkManager.instance.CommandJoinRoom(roomCode);
  }

  private void OnStartGame()
  {
    GetComponent<SceneSwitcher>().SwitchScene();
  }

  private void OnIncorrectGameCode()
  {
    wrongCodeText.gameObject.SetActive(true);
  }
}
