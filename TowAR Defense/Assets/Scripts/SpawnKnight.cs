using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnKnight : MonoBehaviour
{
    public GameObject knightPrefab;
    public Transform gameBoard;
    public Transform target;
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

    public void HandleClick() {
        Vector3 position = new Vector3(ran.Next(-2, 2), 0, ran.Next(-4, 4));
        Spawn(position, Quaternion.identity);
    }

    public void Spawn(Vector3 position, Quaternion rotation)
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