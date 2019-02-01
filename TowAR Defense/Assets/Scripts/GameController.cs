using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
  #region Public Members
  [HideInInspector]
  public float doubloons;
  [HideInInspector]
  public bool isPlayer1;
  // [HideInInspector]
  public int castleHealth;
  public int enemyCastleHealth;
  #endregion

  #region Private Members
  private bool initalized = false;
  private SpawnKnight spawnKnight;
  #endregion

  public static GameController instance = null;
  #region MonoBehaviour Methods

  void Awake()
  {
    if (instance == null)
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

    public void Initialize(NetworkManager.PlayerJSON playerData)
    {
        doubloons = playerData.doubloons;
        isPlayer1 = playerData.playerNo == 1;
        initalized = true;
    }

    public void RequestSpawnUnit()
    {
        CheckInitialized();
        var pos = new Vector3(0, 0, isPlayer1 ? 4 : -4);
        var rot = Quaternion.Euler(0, isPlayer1 ? 180 : 0, 0);
        NetworkManager.instance.CommandSpawn(pos, rot);
    }

    public void RequestDebugSpawnUnit()
    {
        CheckInitialized();
        var pos = new Vector3(0, 0, isPlayer1 ? 4 : -4);
        var rot = Quaternion.Euler(0, isPlayer1 ? 180 : 0, 0);
        NetworkManager.instance.CommandDebugSpawn(pos, rot);
    }

    public void RequestTowerDamage(string unitType)
    {
        CheckInitialized();
        NetworkManager.instance.CommandTakeTowerDamage(unitType);
    }

    #endregion

    #region Event Handlers

    private void OnUnitSpawn(Vector3 position, Quaternion rotation, bool _isPlayer1)
    {
        Debug.Log("Spawning");
        spawnKnight.Spawn(position, rotation, _isPlayer1);
    }

<<<<<<< HEAD
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

  public void Initialize(NetworkManager.PlayerJSON playerData)
  {
    doubloons = playerData.doubloons;
    isPlayer1 = playerData.playerNo == 1;
    castleHealth = playerData.castleHealth;
    enemyCastleHealth = playerData.enemyCastleHealth;
    initalized = true;
  }

  public void RequestSpawnUnit()
  {
    CheckInitialized();
    var pos = new Vector3(0, 0, isPlayer1 ? 4 : -4);
    var rot = Quaternion.Euler(0, isPlayer1 ? 180 : 0, 0);
    NetworkManager.instance.CommandSpawn(pos, rot);
  }

  public void RequestDebugSpawnUnit()
  {
    CheckInitialized();
    var pos = new Vector3(0, 0, isPlayer1 ? 4 : -4);
    var rot = Quaternion.Euler(0, isPlayer1 ? 180 : 0, 0);
    NetworkManager.instance.CommandDebugSpawn(pos, rot);
  }

  public void RequestTowerDamage(string unitType)
  {
    CheckInitialized();
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

  #region Private Methods
  private void CheckInitialized()
  {
    if (!initalized)
      throw new System.InvalidOperationException("Attempted to use GameController before initializing");
  }
  #endregion
=======
    #endregion

    #region Private Methods
    private void CheckInitialized()
    {
        if (!initalized)
            throw new System.InvalidOperationException("Attempted to use GameController before initializing");
    }
    #endregion
>>>>>>> 5ec6d43cee3debefff049a028fbc56bc05275820
}
