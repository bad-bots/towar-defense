using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMovementBehaviour))]
public class UnitTargetingBehaviour : MonoBehaviour
{
    #region Private Members
    private UnitMovementBehaviour movementBehaviour;
    #endregion

    #region MonoBehaviour Methods
    void Awake()
    {
        movementBehaviour = GetComponent<UnitMovementBehaviour>();
    }

    void Start()
    {
        movementBehaviour.target = FindEnemyTower().transform;
    }
    #endregion

    #region Private Methods
    private GameObject FindEnemyTower()
    {
        bool isPlayerOne = GameController.instance.isPlayer1;
        string towerName = "Tower" + (isPlayerOne ? "2" : "1");
        var towers = GameObject.FindGameObjectsWithTag("Tower");
        if (towers[0].name == towerName)
        {
            return towers[0];
        }
        else
        {
            return towers[1];
        }
    }
    #endregion
}
