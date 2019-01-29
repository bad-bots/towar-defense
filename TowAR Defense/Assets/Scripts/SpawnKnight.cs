using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnKnight : MonoBehaviour
{
    public GameObject knightPrefab;
    public Transform gameBoard;
    public System.Random ran = new System.Random();

    public List<GameObject> createdObjects = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Spawn()
    {
        Vector3 position = new Vector3(0, 0, ran.Next(-4, 4));
        if (knightPrefab != null)
        {
            Instantiate(knightPrefab, position, Quaternion.identity, gameBoard);
        }
    }
}