using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public void SpawnKnight()
    {
        GameController.instance.RequestSpawnUnit("Knight");
    }

    public void SpawnUnit(string unitType)
    {
        GameController.instance.RequestSpawnUnit(unitType);
    }

    public void DebugSpawnNight()
    {
        GameController.instance.RequestDebugSpawnUnit();
    }
}
