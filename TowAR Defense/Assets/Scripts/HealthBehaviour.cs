using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBehaviour : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public GameObject healthBarPrefab;

    private Slider m_healthBar;
    private float lastHealth;

    // Start is called before the first frame update
    void Start()
    {
        var healthCanvas = Instantiate(healthBarPrefab, transform);
        m_healthBar = healthCanvas.GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else if (health != lastHealth)
        {
            m_healthBar.value = health;
            m_healthBar.maxValue = maxHealth;
        }
    }
}
