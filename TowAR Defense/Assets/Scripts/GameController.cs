using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitSpawner))]
public class GameController : MonoBehaviour
{
    #region Public Members
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
        NetworkManager.instance.UpdateCastleHealth += updateCastleHealth;
        NetworkManager.instance.UpdateDoubloons += updateDoubloons;

    }

    #endregion

    #region Public Methods

    public void Initialize(NetworkManager.PlayerJSON playerData)
    {
        doubloons = playerData.doubloons;
        m_isPlayer1 = playerData.playerNo == 1;
        castleHealth = playerData.castleHealth;
        enemyCastleHealth = playerData.enemyCastleHealth;
        m_initalized = true;
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

    #endregion

    #region Event Handlers

    private void OnUnitSpawn(string unitTypeName, Vector3 pos, Quaternion rot, bool _isPlayer1)
    {
        Debug.Log("Spawning unit from controller");
        CheckInitialized();
        Debug.Log("passed init");
        unitSpawner.SpawnUnit(unitTypeName, pos, rot, _isPlayer1);
    }

    // Update Doubloons
    void updateDoubloons(NetworkManager.DecrementDoubloons playerData)
    {
        doubloons = playerData.doubloons;
    }

    void updateCastleHealth(NetworkManager.AttackedPlayerHealth attackedPlayerHealth)
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
