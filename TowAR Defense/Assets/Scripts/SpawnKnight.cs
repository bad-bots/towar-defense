using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnKnight : MonoBehaviour
{
    public GameObject knightPrefab;
    public Transform gameBoard;
    public Transform tower1;
    public Transform tower2;
    public System.Random ran = new System.Random();

    public void HandleClick() {
        Vector3 position1 = new Vector3(1, 0, 4);
        Vector3 position2 = new Vector3(-1, 0, -4);
        Spawn(position1, Quaternion.identity, true);
        Spawn(position2, Quaternion.identity, false);
    }

    public void Spawn(Vector3 position, Quaternion rotation, bool isPlayerOne)
    {
        Transform target = isPlayerOne ? tower2 : tower1;
        if (knightPrefab != null)
        {
            GameObject knight = Instantiate(knightPrefab, gameBoard) as GameObject;
            knight.transform.localScale = new Vector3(.2f, .2f, .2f);
            knight.transform.localPosition = position;
            knight.tag = isPlayerOne ? "player1" : "player2";
            knight.GetComponent<UnitMovementBehaviour>().target = target;
        }
    }
}