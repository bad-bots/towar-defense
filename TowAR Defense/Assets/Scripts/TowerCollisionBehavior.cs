using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerCollisionBehavior : MonoBehaviour
{
    public int towerHealth;

    public Text healthText;
    public Text winText;

    // Start is called before the first frame update
    void Start()
    {
        towerHealth = 10;
        winText.text = "";
        SetHealthText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Ground") {
            Destroy(other.gameObject);
        }
        DecrementHealth();  // replace with emit to server
    }

    public void DecrementHealth()
    {
        towerHealth--;
        SetHealthText();
    }

    public void SetHealthText()
    {
        healthText.text = "Enemy HP" + towerHealth.ToString();
        if (towerHealth <= 0)
        {
            winText.text = "You Win!";
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}