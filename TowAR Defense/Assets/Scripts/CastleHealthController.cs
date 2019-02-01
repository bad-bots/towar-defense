using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleHealthController : MonoBehaviour
{
    public TextMesh castleHealth;
    public TextMesh enemyCastleHealth;
    private Transform myTower;
    private Transform enemyTower;

    // Start is called before the first frame update
    void Start()
    {
        var towers = GameObject.FindGameObjectsWithTag("Tower");
        if (GameController.instance.isPlayer1)
        {
            if (towers[0].name == "Tower1")
            {
                myTower = towers[0].transform;
                enemyTower = towers[1].transform;
            } else
            {
                myTower = towers[1].transform;
                enemyTower = towers[0].transform;
            }
        } else
        {
            if (towers[0].name == "Tower1")
            {
                myTower = towers[1].transform;
                enemyTower = towers[0].transform;
            }
            else
            {
                myTower = towers[0].transform;
                enemyTower = towers[1].transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        myTower.gameObject.GetComponentInChildren<TextMesh>().text =
            GameController.instance.castleHealth.ToString();

        enemyTower.gameObject.GetComponentInChildren<TextMesh>().text =
            GameController.instance.enemyCastleHealth.ToString();
    }
}
