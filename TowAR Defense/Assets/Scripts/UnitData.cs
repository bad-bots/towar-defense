using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    public UnitType type;
    public float currentHealth;
    public int playerNo = 1;
    public int unitId;

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
    }

    void OnDestroy()
    {
        GameController.instance.unitSpawner.RemoveUnit(this);
    }
}
