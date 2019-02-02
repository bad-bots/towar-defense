using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Units/Type", order = 85)]
public class UnitType : ScriptableObject
{
    public GameObject unitPrefab;
    public float speed = 1f;
    public float maxHealth = 10;
    public float attackRange = 0.1f;
    public float attackDamage = 5;
}
