using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public float doubloons;
    // [HideInInspector]
    public int castleHealth;
    public int enemyCastleHealth;
    #endregion

    #region Private Members
    // Game state
    private bool m_initalized = false;
    private bool m_isPlayer1;

    // internal data
    private Dictionary<string, UnitType> m_unitTypes = new Dictionary<string, UnitType>();
    private List<UnitData> m_allies = new List<UnitData>();
    private List<UnitData> m_enemies = new List<UnitData>();
    private Transform gameBoard;
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
        // Find all Unit Types
        var unitTypes = new List<UnitType>(Resources.LoadAll<UnitType>(""));
        foreach (var type in unitTypes)
        {
            Debug.Log("Found Unit Type: " + type.name);
            m_unitTypes.Add(type.name, type);
        }
        Debug.Log("Found " + unitTypes.Count + " unit types");

        gameBoard = GameObject.FindGameObjectWithTag("Game Board").transform;

        // Register network handlers
        NetworkManager.instance.SpawnKnightEvent += OnUnitSpawn;
        NetworkManager.instance.UpdateCastleHealth += updateCastleHealth;
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

    public void RequestTowerDamage(string unitType, int attackedPlayer)
    {
        CheckInitialized();
        NetworkManager.instance.CommandTakeTowerDamage(unitType, attackedPlayer);
    }

    #endregion

    #region Event Handlers

    private void OnUnitSpawn(Vector3 position, Quaternion rotation, bool _isPlayer1)
    {
        CheckInitialized();
        Debug.Log("Spawning");
        UnitType unitType = null;
        if (m_unitTypes.TryGetValue("Knight", out unitType))
        {
            var spawnedUnit = Instantiate(unitType.unitPrefab, gameBoard) as GameObject;
            spawnedUnit.transform.localPosition = position;
            spawnedUnit.transform.localRotation = rotation;
            spawnedUnit.tag = _isPlayer1 ? "Player1" : "Player2";
            var unitData = spawnedUnit.GetComponent<UnitData>();
            unitData.type = unitType;
            unitData.playerNo = _isPlayer1 ? 1 : 2;
        }
        else
        {
            Debug.LogWarning("Unit Type has no prefab attached");
        }
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
            throw new System.InvalidOperationException("Attempted to use GameController before initializing");
    }
    #endregion
}
