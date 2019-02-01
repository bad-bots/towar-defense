using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementBehaviour : MonoBehaviour
{
    public Transform target;
    public float speed = 1;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform == target && collision.gameObject.CompareTag("Tower"))
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
