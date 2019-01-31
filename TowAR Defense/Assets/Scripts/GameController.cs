using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
  #region Public Members
  // [HideInInspector]
  public float doubloons;
  // [HideInInspector]
  public bool isPlayer1;
  #endregion

  #region Private Members
  private SpawnKnight spawnKnight;
  #endregion

  public static GameController instance = null;
  #region MonoBehaviour Methods

  void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else if (instance != this)
    {
      Destroy(gameObject);
    }

    DontDestroyOnLoad(gameObject);
  }

  void Start()
  {
    // Grab component references
    spawnKnight = GetComponent<SpawnKnight>();

    // Register network handlers
    NetworkManager.instance.SpawnKnightEvent += OnUnitSpawn;
  }

  #endregion

  #region Public Methods

  public void RequestSpawnUnit()
  {
    var pos = new Vector3(0, 0, isPlayer1 ? 4 : -4);
    var rot = Quaternion.Euler(0, isPlayer1 ? 180 : 0, 0);
    NetworkManager.instance.CommandSpawn(pos, rot);
  }

  public void RequestDebugSpawnUnit()
  {
    var pos = new Vector3(0, 0, 4);
    var rot = Quaternion.Euler(0, 180,0);
    NetworkManager.instance.CommandDebugSpawn(pos, rot);
   }

  public void RequestTowerDamage(string unitType)
  {
        NetworkManager.instance.CommandTakeTowerDamage(unitType);
  }

  #endregion

  #region Event Handlers

  private void OnUnitSpawn(Vector3 position, Quaternion rotation, bool _isPlayer1)
  {
    Debug.Log("Spawning");
    spawnKnight.Spawn(position, rotation, _isPlayer1);
  }

  #endregion


}
