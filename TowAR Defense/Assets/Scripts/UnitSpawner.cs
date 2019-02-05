using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public ReadOnlyCollection<UnitData> p1_units
    {
        get;
        private set;
    }

    public ReadOnlyCollection<UnitData> p2_units
    {
        get;
        private set;
    }

    public List<UnitData> m_p1_units = new List<UnitData>();
    public List<UnitData> m_p2_units = new List<UnitData>();

    private Dictionary<string, UnitType> m_unitTypes = new Dictionary<string, UnitType>();
    private Transform m_gameBoard;

    #region MonoBehaviour Methods

    void Awake()
    {
        p1_units = m_p1_units.AsReadOnly();
        p2_units = m_p2_units.AsReadOnly();
    }

    // Start is called before the first frame update
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

        m_gameBoard = GameObject.FindGameObjectWithTag("Game Board").transform;
    }

    #endregion

    #region Public Methods

    public void SpawnUnit(string unitTypeName, Vector3 position, Quaternion rotation, bool isPlayer1, int unitId = 0)
    {
        Debug.Log("Spawning");
        UnitType unitType = null;
        if (m_unitTypes.TryGetValue(unitTypeName, out unitType))
        {
            var spawnedUnit = Instantiate(unitType.unitPrefab, m_gameBoard) as GameObject;
            spawnedUnit.transform.localPosition = position;
            spawnedUnit.transform.localRotation = rotation;
            spawnedUnit.tag = "Unit";
            var unitData = spawnedUnit.GetComponent<UnitData>();
            unitData.type = unitType;
            unitData.playerNo = isPlayer1 ? 1 : 2;
            unitData.unitId = unitId;

            // Add to list of created units
            (isPlayer1 ? m_p1_units : m_p2_units).Add(unitData);
        }
        else
        {
            Debug.LogWarning("Unit Type has no prefab attached");
        }
    }

    public bool RemoveUnit(UnitData unit)
    {
        if (unit == null)
        {
            Debug.LogWarning("Attempted to remove null unit");
            return false;
        }

        if (unit.playerNo == 1)
        {
            return m_p1_units.Remove(unit);
        }
        else if (unit.playerNo == 2)
        {
            return m_p2_units.Remove(unit);
        }
        else
        {
            Debug.LogWarning("Invalid player number: " + unit.playerNo);
            return false;
        }
    }

    public ReadOnlyCollection<UnitData> GetAllyUnits(int playerNo)
    {
        if (playerNo == 1)
        {
            return p1_units;
        }
        else if (playerNo == 2)
        {
            return p2_units;
        }
        else
        {
            return null;
        }
    }

    public ReadOnlyCollection<UnitData> GetEnemyUnits(int playerNo)
    {
        if (playerNo == 1)
        {
            return p2_units;
        }
        else if (playerNo == 2)
        {
            return p1_units;
        }
        else
        {
            return null;
        }
    }

    #endregion
}
