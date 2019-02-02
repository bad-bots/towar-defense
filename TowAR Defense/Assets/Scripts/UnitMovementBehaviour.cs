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
            int attackedPlayer = collision.gameObject.name == "Tower1" ? 1 : 2;

            // Only have attacking player send the request to attack enemy castle.
            if (GameController.instance.isPlayer1 && attackedPlayer == 2)
            {
                NetworkManager.instance.CommandTakeTowerDamage(gameObject.name, attackedPlayer);
            }
            else if (!GameController.instance.isPlayer1 && attackedPlayer ==1)
            {
                NetworkManager.instance.CommandTakeTowerDamage(gameObject.name, attackedPlayer);
            }

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
