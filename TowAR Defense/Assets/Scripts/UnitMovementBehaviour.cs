using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitType))]
public class UnitMovementBehaviour : MonoBehaviour
{
    public Transform target;

    private float distanceThreshold;
    private UnitType unitType;

    void Start()
    {
        unitType = GetComponent<UnitData>().type;
        Debug.Log(unitType);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform == target && collision.gameObject.CompareTag("Tower"))
        {
            NetworkManager.instance.CommandTakeTowerDamage(gameObject.name);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        if ((transform.position - target.position).sqrMagnitude > distanceThreshold)
            transform.position += transform.forward * unitType.speed * Time.deltaTime;
    }
}
