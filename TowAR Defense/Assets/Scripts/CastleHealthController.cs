using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @brief A class for stuff and things
 */
public class CastleHealthController : MonoBehaviour
{
    public TextMesh castleHealth;
    public TextMesh enemyCastleHealth;
    private GameObject myTower;
    private GameObject enemyTower;
    private HealthBehaviour castleHealthBar;
    private HealthBehaviour enemyCastleHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        var towers = GameObject.FindGameObjectsWithTag("Tower");
        if (GameController.instance.isPlayer1)
        {
            if (towers[0].name == "Tower1")
            {
                myTower = towers[0];
                enemyTower = towers[1];
            }
            else
            {
                myTower = towers[1];
                enemyTower = towers[0];
            }
        }
        else
        {
            if (towers[0].name == "Tower1")
            {
                myTower = towers[1];
                enemyTower = towers[0];
            }
            else
            {
                myTower = towers[0];
                enemyTower = towers[1];
            }
        }

        castleHealth = myTower.GetComponentInChildren<TextMesh>();
        enemyCastleHealth = enemyTower.GetComponentInChildren<TextMesh>();
        castleHealthBar = myTower.GetComponent<HealthBehaviour>();
        enemyCastleHealthBar = enemyTower.GetComponent<HealthBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        castleHealth.text = GameController.instance.castleHealth.ToString();
        enemyCastleHealth.text = GameController.instance.enemyCastleHealth.ToString();

        castleHealthBar.health = GameController.instance.castleHealth;
        enemyCastleHealthBar.health = GameController.instance.enemyCastleHealth;

    }

}
