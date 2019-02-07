using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBehaviour : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public GameObject healthBarPrefab;
    public float scale = 1;
    public Vector3 offset;
    public bool alwaysShowHealth = false;

    private Slider m_healthBar;
    private float lastHealth = 0;

    // Start is called before the first frame update
    void Start()
    {
        var healthCanvas = Instantiate(healthBarPrefab, transform);
        healthCanvas.transform.localPosition += offset;
        healthCanvas.transform.localScale = Vector3.Scale(healthCanvas.transform.localScale, new Vector3(scale, scale, scale));
        m_healthBar = healthCanvas.GetComponentInChildren<Slider>();
        CheckShowHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (health != lastHealth)
        {
            m_healthBar.value = health;
            m_healthBar.maxValue = maxHealth;
            CheckShowHealthBar();
        }
    }

    private void CheckShowHealthBar()
    {
        if (alwaysShowHealth) return;

        if (health != maxHealth)
        {
            m_healthBar.gameObject.SetActive(true);
        }
        else
        {
            m_healthBar.gameObject.SetActive(false);
        }
    }
}
