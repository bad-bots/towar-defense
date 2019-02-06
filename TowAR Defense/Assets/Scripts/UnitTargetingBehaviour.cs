using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    void Update()
    {
        if ((movementBehaviour.target == null) && unitData.currentHealth > 0)
        {
            SelectNewTarget();
        }
        else if (movementBehaviour.target != null)
        {
            var targetUnitData = movementBehaviour.target.GetComponent<UnitData>();
            if (targetUnitData != null && targetUnitData.currentHealth <= 0)
            {
                SelectNewTarget();
            }
        }
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

        var allies = GameController.instance.unitSpawner.GetAllyUnits(unitData.playerNo);
        foreach (UnitData allyGO in allies)
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
        var enemies = GameController.instance.unitSpawner.GetEnemyUnits(unitData.playerNo);
        Transform target = FindNearestEnemy(enemies);
        if (target == null)
        {
            target = FindEnemyTower();
        }

        return target;
    }

    private Transform FindNearestEnemy(ReadOnlyCollection<UnitData> enemies)
    {
        Transform nearest = null;
        float minDistSqr = Mathf.Infinity;
        Vector3 currPostion = transform.position;
        foreach (UnitData potentialTarget in enemies)
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
