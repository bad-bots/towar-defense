using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementBehaviour : MonoBehaviour
{
    public Transform target;
    public float speed = 1;
    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("tower hit");
        if (collision.gameObject == target.gameObject && collision.gameObject.CompareTag("Tower")) 
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(target);
        rb.velocity = transform.forward * speed;
    }
}
