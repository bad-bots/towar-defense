using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoubloonsController : MonoBehaviour
{
    private Text doubloonsText;
    // Start is called before the first frame update
    void Start()
    {
        doubloonsText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        doubloonsText.text = GameController.instance.doubloons.ToString();
    }

}
