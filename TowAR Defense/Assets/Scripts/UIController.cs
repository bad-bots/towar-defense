using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{

    public Button[] spawnUnitButtonsType;
    public Button spawnUnitButton;
    void Awake()
    {
        spawnUnitButtonsType = GetComponentsInChildren<Button>();
    }
    public void SpawnKnight()
    {
        GameController.instance.RequestSpawnUnit("Knight");
    }
    public void SpawnUnit(string unitType)
    {
        //Do something
        GameController.instance.RequestSpawnUnit(unitType);
        foreach(var button in spawnUnitButtonsType)
        {
            if((button.name.StartsWith("spawn" + unitType, true, System.Globalization.CultureInfo.CurrentCulture)) || (button.name.StartsWith("ATTACK" + unitType, true, System.Globalization.CultureInfo.CurrentCulture)))
            {
                button.interactable = false;
                StartCoroutine(ResetCooldown(2.0f, button));
            };
        }

    }

    public void DebugSpawnNight()
    {
        GameController.instance.RequestDebugSpawnUnit();
    }
    IEnumerator ResetCooldown(float cooldown, Button button)
    {
        yield return new WaitForSeconds(cooldown);
        button.interactable = true;
    }
}
