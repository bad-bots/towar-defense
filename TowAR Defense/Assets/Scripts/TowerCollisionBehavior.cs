using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerCollisionBehavior : MonoBehaviour
{

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.tag != "Ground")
    {
      Destroy(other.gameObject);
    }
  }

}
