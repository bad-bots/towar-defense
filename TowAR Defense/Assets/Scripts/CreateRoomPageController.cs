using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomPageController : MonoBehaviour
{
  public Text statusMessageText;
  public Text roomCodeText;
  public InputField roomNameInput;
  public GameObject button;

  public void HandleCreateRoom()
  {
    string roomName = roomNameInput.text;
    statusMessageText.text = "Loading....";
    NetworkManager.instance.CommandCreateRoom(roomName, OnRoomCreated);

  }

  private void OnRoomCreated(string joinToken)
  {
    statusMessageText.text = "Created Room!\nGive this code to your buddy!";
    roomCodeText.text = joinToken;
    roomCodeText.gameObject.SetActive(true);
    roomNameInput.interactable = false;
    button.SetActive(false);
  }

}
