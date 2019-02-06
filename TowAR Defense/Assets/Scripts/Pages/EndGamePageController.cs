using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePageController : MonoBehaviour
{
    public Text winningText;

    void Start()
    {
        // NetworkManager.instance.UpdateGameStateEvent += OnWinningPlayerChange;
        winningText.text = "Player " + PersistantStats.winningPlayer +" wins";
    }

    void OnWinningPlayerChange()
    {
        winningText.text = "Player " + PersistantStats.winningPlayer +" wins";
    }

    public void PlayAgain()
    {
        NetworkManager.instance.CommandJoinRoom(PersistantStats.joinToken);
    }

    public void ReturnToMain()
    {
        // Leave Room on server and switch to the main page
        NetworkManager.instance.CommandLeaveRoom();
        PageController.instance.SwitchPage("MainPage");
    }

    void OnDisable()
    {
        UnWatchEvents();
    }

    void UnWatchEvents()
    {
        // NetworkManager.instance.UpdateGameStateEvent -= OnWinningPlayerChange;
    }
}
