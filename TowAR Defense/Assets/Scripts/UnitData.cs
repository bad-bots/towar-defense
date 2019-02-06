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
    Animator m_Animator;

    private HealthBehaviour m_healthBehaviour;
    private float previousHealth;

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
        m_Animator = gameObject.GetComponent<Animator>();
        m_healthBehaviour = GetComponent<HealthBehaviour>();
        m_healthBehaviour.maxHealth = type.maxHealth;
        m_healthBehaviour.health = type.maxHealth;
    }

    void Update()
    {
        m_healthBehaviour.health = currentHealth;
        if (currentHealth <= 0 && currentHealth != previousHealth)
        {
            if (!GameController.instance.unitSpawner.RemoveUnit(this))
                Debug.LogWarning("unit not removed");
            m_Animator.SetBool("Dead", true);
            Invoke("DestroyUnit", 3);
        }
        previousHealth = currentHealth;
    }

    void OnDestroy()
    {
        GameController.instance.unitSpawner.RemoveUnit(this);
    }

    void DestroyUnit()
    {
        Destroy(gameObject);
    }
}
