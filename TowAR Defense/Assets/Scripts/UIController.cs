using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public void SpawnKnight()
    {
        GameController.instance.RequestSpawnUnit();
    }

    public void DebugSpawnNight()
    {
        GameController.instance.RequestDebugSpawnUnit();
    }
}
