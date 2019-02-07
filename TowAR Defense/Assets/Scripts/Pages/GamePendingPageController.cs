using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePendingPageController : MonoBehaviour
{
    public Text textField;

    void Start()
    {
        NetworkManager.instance.UpdateGameStateEvent += OnJoinTokenChanged;
        textField.text = PersistantStats.joinToken;
    }

    public void OnJoinTokenChanged()
    {
        textField.text = PersistantStats.joinToken;
    }

    public void CancelCreateRoom()
    {
        NetworkManager.instance.CommandLeaveRoom();
        PageController.instance.SwitchPage("MainPage");
    }

}