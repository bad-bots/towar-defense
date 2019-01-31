using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnKnight : MonoBehaviour
{
  public GameObject knightPrefab;
  public Transform gameBoard;
  public Transform target;


  public void Spawn(Vector3 position, Quaternion rotation, bool isPlayer1)
  {
    if (knightPrefab != null)
    {
      GameObject knight = Instantiate(knightPrefab, gameBoard) as GameObject;
      knight.transform.localScale = new Vector3(.2f, .2f, .2f);
      knight.transform.localPosition = position;
      knight.GetComponent<UnitMovementBehaviour>().target = target;
    }
  }
}
