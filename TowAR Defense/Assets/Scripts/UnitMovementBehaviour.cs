using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitData))]
public class UnitMovementBehaviour : MonoBehaviour
{
    public Transform target;

    private float distanceThreshold;
    private UnitData unitData;

    void Start()
    {
        unitData = GetComponent<UnitData>();
        distanceThreshold = Mathf.Pow(unitData.type.attackRange, 2);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        if ((transform.localPosition - target.localPosition).sqrMagnitude > distanceThreshold)
            transform.localPosition += transform.forward * unitData.type.speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform != target) return;
        if (CheckTowerCollision(collision)) return;
        if (CheckUnitCollision(collision)) return;
    }

    private bool CheckUnitCollision(Collision collision)
    {
        return false;
    }

    private bool CheckTowerCollision(Collision collision)
    {
        string towerName = "Tower" + (unitData.enemyPlayerNo);
        if (collision.gameObject.CompareTag("Tower"))
        {
            int attackedPlayer = collision.gameObject.name == "Tower1" ? 1 : 2;

            // Only have attacking player send the request to attack enemy castle.
            if (GameController.instance.isPlayer1 && attackedPlayer == 2)
            {
                NetworkManager.instance.CommandTakeTowerDamage(gameObject.name, attackedPlayer);
            }
            else if (!GameController.instance.isPlayer1 && attackedPlayer == 1)
            {
                NetworkManager.instance.CommandTakeTowerDamage(gameObject.name, attackedPlayer);
            }

            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
