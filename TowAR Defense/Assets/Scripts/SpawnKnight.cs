using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnKnight : MonoBehaviour
{
    public GameObject knightPrefab;

    private Transform gameBoard;

    void Start()
    {
        gameBoard = GameObject.FindGameObjectWithTag("Game Board").transform;
        if (gameBoard == null) return;
    }

    public void Spawn(Vector3 position, Quaternion rotation, bool isPlayerOne)
    {
        if (knightPrefab != null)
        {
            GameObject knight = Instantiate(knightPrefab, gameBoard) as GameObject;
            knight.transform.localPosition = position;
            knight.tag = isPlayerOne ? "Player1" : "Player2";
        }
        else
        {
            Debug.LogWarning("Cannot spawn knight without a prefab");
        }
    }
}

