using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    public UnitType type;
    public float currentHealth;
    public int playerNo = 1;

    void Start()
    {
        currentHealth = type.maxHealth;
    }
}
