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

  // Start is called before the first frame update
  void Start()
  {
    //statusMessageText.text = "Loading....";
  }

  public void HandleCreateRoom()
  {
    string roomName = roomNameInput.text;
    NetworkManager.instance.CommandCreateRoom(roomName, HandleRoomCreate);

  }

  private void HandleRoomCreate(string joinToken)
  {
    statusMessageText.text = "Created Room!\nGive this code to your buddy!";
    roomCodeText.text = joinToken;
    roomCodeText.gameObject.SetActive(true);
    roomNameInput.interactable = false;
    button.SetActive(false);
  }

}
