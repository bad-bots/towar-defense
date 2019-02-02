using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMovementBehaviour))]
[RequireComponent(typeof(UnitData))]
public class UnitTargetingBehaviour : MonoBehaviour
{
    #region Private Members
    private UnitMovementBehaviour movementBehaviour;
    private UnitData unitData;
    #endregion

    #region MonoBehaviour Methods
    void Awake()
    {
        movementBehaviour = GetComponent<UnitMovementBehaviour>();
        unitData = GetComponent<UnitData>();
    }

    void Start()
    {
        SelectNewTarget();
    }
    #endregion

    #region Public Methods

    public void SelectNewTarget(Transform target = null)
    {
        if (target == null)
        {
            target = FindNewTarget();
        }
        movementBehaviour.target = target;
        var targetMovement = target.GetComponent<UnitMovementBehaviour>();
        if (targetMovement)
            SetTargetIfOnTower(targetMovement, this.transform);

        string alliesTag = GameController.instance.isPlayer1 ? "Player1" : "Player2";
        var allies = GameObject.FindGameObjectsWithTag(alliesTag);
        foreach (GameObject allyGO in allies)
        {
            var allyMovement = allyGO.GetComponent<UnitMovementBehaviour>();
            SetTargetIfOnTower(allyMovement, target);
        }
    }

    #endregion

    #region Private Methods

    private void SetTargetIfOnTower(UnitMovementBehaviour _movementBehaviour, Transform target)
    {
        if (_movementBehaviour.target == null || _movementBehaviour.target.tag.StartsWith("Tower"))
        {
            _movementBehaviour.target = target;
        }
    }

    private Transform FindNewTarget()
    {
        string enemyTag = "Player" + (unitData.enemyPlayerNo);
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform target = FindNearestEnemy(enemies);
        if (target == null)
        {
            target = FindEnemyTower();
        }

        return target;
    }

    private Transform FindNearestEnemy(GameObject[] enemies)
    {
        Transform nearest = null;
        float minDistSqr = Mathf.Infinity;
        Vector3 currPostion = transform.position;
        foreach (GameObject potentialTarget in enemies)
        {
            Vector3 dirToTarget = potentialTarget.transform.position - currPostion;
            float dSqrToTarget = dirToTarget.sqrMagnitude;
            if (dSqrToTarget < minDistSqr)
            {
                minDistSqr = dSqrToTarget;
                nearest = potentialTarget.transform;
            }
        }

        return nearest;
    }

    private Transform FindEnemyTower()
    {
        string towerName = "Tower" + (unitData.enemyPlayerNo);
        var towers = GameObject.FindGameObjectsWithTag("Tower");
        int towerIndex = (towers[0].name == towerName) ? 0 : 1;
        return towers[towerIndex].transform;
    }
    #endregion
}
