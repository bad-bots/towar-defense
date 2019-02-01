using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementBehaviour : MonoBehaviour
{
    public Transform target;
    public float speed = 1;
    public float distanceThreshold = 0.1f;

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
            transform.position += transform.forward * speed * Time.deltaTime;
    }
}
