using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitSpawner))]
public class GameController : MonoBehaviour
{
    #region Public Properties
    public bool isPlayer1
    {
        get
        {
            return m_isPlayer1;
        }
    }

    public bool isInitialized
    {
        get
        {
            return m_initalized;
        }
    }
    #endregion

    #region Public Members

    public float doubloons;
    // [HideInInspector]
    public int castleHealth;
    public int enemyCastleHealth;

    [HideInInspector]
    public UnitSpawner unitSpawner;
    #endregion

    #region Private Members
    // Game state
    private bool m_initalized = false;
    private bool m_isPlayer1;

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
        }
    }

    void Start()
    {
        unitSpawner = GetComponent<UnitSpawner>();

        // Register network handlers
        NetworkManager.instance.SpawnUnitEvent += OnUnitSpawn;
        NetworkManager.instance.UpdateCastleHealthEvent += UpdateCastleHealth;
        NetworkManager.instance.UpdateUnitHealthEvent += UpdateUnitHealth;
        NetworkManager.instance.UpdateDoubloonsEvents += updateDoubloons;

    }

    #endregion

    #region Public Methods

    /**
     * @brief Initialize the game controller with given player data
     *
     * @par This is a bunch of text describing what the funciton does
     *
     * @param playerData Information about this client's player
     */
    public void Initialize(NetworkManager.PlayerJSON playerData)
    {
        doubloons = playerData.doubloons;
        m_isPlayer1 = playerData.playerNo == 1;
        castleHealth = playerData.castleHealth;
        enemyCastleHealth = playerData.enemyCastleHealth;
        m_initalized = true;
    }

    public void UnInitialize()
    {
        m_initalized = false;
    }

    public void RequestSpawnUnit(string unitType)
    {
        CheckInitialized();
        NetworkManager.instance.CommandSpawn(unitType);
    }

    public void RequestDebugSpawnUnit()
    {
        CheckInitialized();
        var pos = new Vector3(0, 0, isPlayer1 ? 4 : -4);
        var rot = Quaternion.Euler(0, isPlayer1 ? 180 : 0, 0);
        NetworkManager.instance.CommandDebugSpawn(pos, rot);
    }

    public void RequestTowerDamage(string unitType, int attackedPlayer)
    {
        CheckInitialized();
        NetworkManager.instance.CommandTakeTowerDamage(unitType, attackedPlayer);
    }

    public void RequestUnitDamage(int attackerId, int defenderId)
    {
        CheckInitialized();
        NetworkManager.instance.CommandTakeUnitDamage(attackerId, defenderId);
    }

    #endregion

    #region Event Handlers

    private void OnUnitSpawn(NetworkManager.UnitSpawnData spawnData)
    {
        Debug.Log("Spawning unit from controller");
        CheckInitialized();
        Debug.Log("passed init");
        unitSpawner.SpawnUnit(spawnData);
    }

    // Update Doubloons
    void updateDoubloons(NetworkManager.UpdateDoubloons playerData)
    {
        doubloons = playerData.doubloons;
    }

    private void UpdateCastleHealth(NetworkManager.AttackedPlayerHealth attackedPlayerHealth)
    {
        if (attackedPlayerHealth.playerNo == 1 && isPlayer1)
        {
            castleHealth = attackedPlayerHealth.castleHealth;
        }
        else if (attackedPlayerHealth.playerNo == 1 && !isPlayer1)
        {
            enemyCastleHealth = attackedPlayerHealth.castleHealth;
        }
        else if (attackedPlayerHealth.playerNo == 2 && isPlayer1)
        {
            enemyCastleHealth = attackedPlayerHealth.castleHealth;
        }
        else
        {
            castleHealth = attackedPlayerHealth.castleHealth;
        }
    }

    private void UpdateUnitHealth(NetworkManager.UnitHealthJSON unitHealthJSON)
    {
        var allUnits = unitHealthJSON.playerNo == 1 ? unitSpawner.p1_units : unitSpawner.p2_units;
        foreach (var unit in allUnits)
        {
            if (unit.unitId == unitHealthJSON.unitId)
            {
                unit.currentHealth = unitHealthJSON.health;
            }
        }
    }
    #endregion

    #region Private Methods
    private void CheckInitialized()
    {
        if (!m_initalized)
        {
            Debug.LogError("Failed to initialized game controller");
            throw new System.InvalidOperationException("Attempted to use GameController before initializing");
        }
    }
    #endregion
}
