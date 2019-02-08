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

        //Find winning player and update text accordingly
        if(GameController.instance.isPlayer1 && PersistantStats.winningPlayer == 1)
        {
            winningText.text = "You Win!";
            // winningText.text = "Player " + PersistantStats.winningPlayer +" wins";
        }
        else if(GameController.instance.isPlayer1 && PersistantStats.winningPlayer != 1)
        {
            winningText.text = "You Lose!";
        }
        else if(!GameController.instance.isPlayer1 && PersistantStats.winningPlayer == 2)
        {
            winningText.text = "You Win!";
        }
        else if(!GameController.instance.isPlayer1 && PersistantStats.winningPlayer != 2)
        {
            winningText.text = "You Lose!";
        }
    }

    void OnWinningPlayerChange()
    {
        //Find winning player and update text accordingly
        if(GameController.instance.isPlayer1 && PersistantStats.winningPlayer == 1)
        {
            winningText.text = "You Win!";
        }
        else if(GameController.instance.isPlayer1 && PersistantStats.winningPlayer != 1)
        {
            winningText.text = "You Lose!";
        }
        else if(!GameController.instance.isPlayer1 && PersistantStats.winningPlayer == 2)
        {
            winningText.text = "You Win!";
        }
        else if(!GameController.instance.isPlayer1 && PersistantStats.winningPlayer != 2)
        {
            winningText.text = "You Lose!";
        }
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
