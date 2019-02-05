using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Units/Type", order = 85)]
public class UnitType : ScriptableObject
{
    [Tooltip("Prefab to spawn for this unit type")]
    public GameObject unitPrefab;
    [Tooltip("Speed (in units/s) this unit moves")]
    public float speed = 1f;
    [Tooltip("Maximum health this unit has")]
    public float maxHealth = 10;
    [Tooltip("The range (in units) this unit can attack from")]
    public float attackRange = 0.1f;
    public float attackDamage = 5;
    [Tooltip("The speed (in attacks/sec) this unit can attack at")]
    public float attackSpeed = 1;
}
