using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitData))]
public class UnitMovementBehaviour : MonoBehaviour
{
    public Transform target;
    Animator m_Animator;
    private float distanceThreshold;
    private UnitData unitData;

    private float nextAttackTime = 0f;
    private bool isAttackingPlayer = false;

    void Start()
    {
        unitData = GetComponent<UnitData>();
        distanceThreshold = Mathf.Pow(unitData.type.attackRange, 2);
        m_Animator = gameObject.GetComponent<Animator>();
        isAttackingPlayer = (GameController.instance.isPlayer1 ? 1 : 2) == unitData.playerNo;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && unitData.currentHealth > 0)
        {
            transform.LookAt(target);
            bool isInRange = (transform.localPosition - target.localPosition).sqrMagnitude < distanceThreshold;
            if (target.CompareTag("Tower") || !isInRange)
            {
                transform.localPosition += transform.forward * unitData.type.speed * Time.deltaTime;
                m_Animator.SetFloat("speed", unitData.type.speed);
                m_Animator.SetBool("Moving", true);
            }
            else
            {
                m_Animator.SetBool("Moving", false);
                m_Animator.SetFloat("speed", 0);
                TryAttackUnit(target.GetComponent<UnitData>());
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform != target) return;
        if (CheckTowerCollision(collision)) return;
    }

    private bool CheckTowerCollision(Collision collision)
    {
        string towerName = "Tower" + (unitData.enemyPlayerNo);
        if (collision.gameObject.CompareTag("Tower"))
        {
            int attackedPlayer = collision.gameObject.name == "Tower1" ? 1 : 2;

            string unitType = GetComponent<UnitData>().type.name;
            // Only have attacking player send the request to attack enemy castle.
            if (GameController.instance.isPlayer1 && attackedPlayer == 2)
            {
                NetworkManager.instance.CommandTakeTowerDamage(unitType, attackedPlayer);
            }
            else if (!GameController.instance.isPlayer1 && attackedPlayer == 1)
            {
                NetworkManager.instance.CommandTakeTowerDamage(unitType, attackedPlayer);
            }
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    private void TryAttackUnit(UnitData unit)
    {
        if (Time.time > nextAttackTime)
        {
            m_Animator.SetTrigger("Attack1Trigger");
            nextAttackTime = Time.time + (1.0f / unitData.type.attackSpeed);
            if (isAttackingPlayer) GameController.instance.RequestUnitDamage(unitData.unitId, unit.unitId);
        }
    }

}
