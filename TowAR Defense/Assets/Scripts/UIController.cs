using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private bool cooldown = false;

    public void SpawnKnight()
    {
        GameController.instance.RequestSpawnUnit("Knight");
    }

    public void SpawnUnit(string unitType)
    {
        if ( cooldown == false ) {
            //Do something
            GameController.instance.RequestSpawnUnit(unitType);
            Invoke("ResetCooldown",2.0f);
            cooldown = true;
     }
    }

    public void DebugSpawnNight()
    {
        GameController.instance.RequestDebugSpawnUnit();
    }
    void ResetCooldown(){
     cooldown = false;
 }
}
