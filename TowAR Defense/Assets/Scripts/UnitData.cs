using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthBehaviour))]
public class UnitData : MonoBehaviour
{
    public UnitType type;
    public float currentHealth;
    public int playerNo = 1;
    public int unitId;

    private HealthBehaviour m_healthBehaviour;

    public int enemyPlayerNo
    {
        get
        {
            return 3 - playerNo;
        }
    }

    void Start()
    {
        currentHealth = type.maxHealth;

        m_healthBehaviour = GetComponent<HealthBehaviour>();
        m_healthBehaviour.maxHealth = type.maxHealth;
        m_healthBehaviour.health = type.maxHealth;
    }

    void Update()
    {
        m_healthBehaviour.health = currentHealth;
    }

    void OnDestroy()
    {
        GameController.instance.unitSpawner.RemoveUnit(this);
    }
}
