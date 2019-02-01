using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnKnight : MonoBehaviour
{
    public GameObject knightPrefab;

    private Transform gameBoard;
    private Transform tower1;
    private Transform tower2;

    void Start()
    {
        gameBoard = GameObject.FindGameObjectWithTag("Game Board").transform;
        if (gameBoard == null) return;

        var towers = GameObject.FindGameObjectsWithTag("Tower");
        if (towers[0].name == "Tower1")
        {
            tower1 = towers[0].transform;
            tower2 = towers[1].transform;
        }
        else
        {
            tower1 = towers[1].transform;
            tower2 = towers[0].transform;

        }
    }

    public void Spawn(Vector3 position, Quaternion rotation, bool isPlayerOne)
    {
        Transform target = isPlayerOne ? tower2 : tower1;
        if (knightPrefab != null)
        {
            GameObject knight = Instantiate(knightPrefab, gameBoard) as GameObject;
            knight.transform.localScale = new Vector3(.2f, .2f, .2f);
            knight.transform.localPosition = position;
            knight.tag = isPlayerOne ? "Player1" : "Player2";
            knight.GetComponent<UnitMovementBehaviour>().target = target;
        }
    }
}

