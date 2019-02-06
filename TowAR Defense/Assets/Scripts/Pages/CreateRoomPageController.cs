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
        NetworkManager.instance.InitRoomEvent += OnRoomCreated;
        NetworkManager.instance.CommandCreateRoom(roomName, OnRoomCreated);

    }

    private void OnRoomCreated(string JoinToken)
    {
        PersistantStats.joinToken = JoinToken;
        NetworkManager.instance.InitRoomEvent -= OnRoomCreated;
        PageController.instance.SwitchPage("GamePendingPage");
    }

}
