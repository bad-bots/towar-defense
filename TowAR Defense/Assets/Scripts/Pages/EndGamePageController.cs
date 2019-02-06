using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePageController : MonoBehaviour
{
    public SceneField gameScene;

    void Start()
    {
        Debug.Log(PersistantStats.winningPlayer);
        if(PersistantStats.winningPlayer == 1 || PersistantStats.winningPlayer == 2)
        {
            var textBoxG = GameObject.FindGameObjectWithTag("Winner");
            Debug.Log(textBoxG);
            var textBox = textBoxG.GetComponent<Text>();
            Debug.Log(textBox);
            textBox.text = "Player " + PersistantStats.winningPlayer +" wins";
        } else {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {

    }

    public void PlayAgain()
    {
        NetworkManager.instance.CommandRestartGame();
        NetworkManager.instance.CommandJoinRoom(PersistantStats.joinToken);
    }

    public void ReturnToMain()
    {
        // Leave Room on server and switch to the main page
        NetworkManager.instance.CommandLeaveRoom();
        GetComponent<PageSwitcher>().SwitchPage();
    }
}
