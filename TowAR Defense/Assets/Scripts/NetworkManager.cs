using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class NetworkManager : MonoBehaviour
{
    #region Public Members
    public static NetworkManager instance = null;
    public Action onSpawnKnight = null;
    #endregion

    #region Public Properties
    public bool isConnected
    {
        get
        {
            return m_isConnected;
        }
    }
    #endregion

    #region Private Members
    private SocketIOComponent socket;

    private Action<string> m_createRoomAcks;

    private bool autoJoinDebug = false;
    private bool m_isConnected = false;
    #endregion

    #region Socket Events
    public event Action<string> InitRoomEvent;
    public event Action<AttackedPlayerHealth> UpdateCastleHealthEvent;
    public event Action<UpdateDoubloons> UpdateDoubloonsEvents;
    public event Action<UnitSpawnData> SpawnUnitEvent;
    public event Action<PlayerJSON> StartGameEvent;
    public event Action<UnitHealthJSON> UpdateUnitHealthEvent;
    public event Action IncorrectRoomCodeEvent;
    public event Action<WinningPlayer> EndGameEvent;
    
    // Notifies all listeners that a game state has changed
    // This prevents classes from using Update to fetch game state
    // on every scene
    public event Action UpdateGameStateEvent;

    #endregion

    #region MONOBEHAVIOUR_METHODS

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);


        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        this.socket = this.GetComponent<SocketIOComponent>();

        socket.On("connect", OnConnect);
        socket.On("disconnect", OnDisconnect);

        socket.On("init", HandleInit);
        socket.On("spawn", HandleSpawnUnit);
        socket.On("incorrectGameToken", HandleIncorrectRoomCode);
        socket.On("start", HandleStartGame);
        socket.On("damageCastle", HandleDamageCastle);
        socket.On("damageUnit", HandleUpdateUnitHealth);
        socket.On("updatePlayerDoubloons", HandleUpdateDoubloons);
        socket.On("endGame", HandleEndGame);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene")
        {
            Debug.LogWarning("Started Game in Game Scene. Joining debug room");
            autoJoinDebug = true;
        }
    }

    #endregion /* MONOBEHAVIOUR_METHODS */

    #region Event Handlers

    private void OnConnect(SocketIOEvent obj)
    {
        m_isConnected = true;
        Debug.Log("Connected to server");
        if (autoJoinDebug)
        {
            Debug.Log("Joining debug room");
            CommandJoinRoom("debugAI");
        }
    }

    private void OnDisconnect(SocketIOEvent obj)
    {
        m_isConnected = false;
    }

    public void HandleInit(SocketIOEvent obj)
    {
        string joinToken = JoinToken.CreateFromJSON(obj.data.ToString()).joinToken;
        InitRoomEvent(joinToken);
        UpdateGameStateEvent();
    }

    private void HandleSpawnUnit(SocketIOEvent obj)
    {
        string data = obj.data.ToString();
        var json = UnitJSON.CreateFromJSON(data);
        var isPlayer1 = json.playerNo == 1;
        var unitType = json.unitType != null ?
             json.unitType[0].ToString().ToUpper() + json.unitType.Substring(1) :
             "Knight";
        var spawnData = new UnitSpawnData(json.playerNo, json.position, json.rotation,
            unitType, json.unitId, json.spawnTime);
        SpawnUnitEvent(spawnData);
    }

    private void HandleStartGame(SocketIOEvent obj)
    {
        Debug.Log("Received game start");
        var playerData = PlayerJSON.CreateFromJSON(obj.data.ToString());
        if (autoJoinDebug)
            GameController.instance.Initialize(playerData);
        else
            StartGameEvent(playerData);
    }

    private void HandleIncorrectRoomCode(SocketIOEvent obj)
    {
        IncorrectRoomCodeEvent();
    }

    private void HandleDamageCastle(SocketIOEvent obj)
    {
        var attackedPlayer = AttackedPlayerHealth.CreateFromJSON(obj.data.ToString());
        UpdateCastleHealthEvent(attackedPlayer);
    }

    private void HandleUpdateUnitHealth(SocketIOEvent obj)
    {
        var unitHealthJSON = UnitHealthJSON.CreateFromJSON(obj.data.ToString());
        UpdateUnitHealthEvent(unitHealthJSON);
    }

    private void HandleUpdateDoubloons(SocketIOEvent obj)
    {
        var newDoubloons = UpdateDoubloons.CreateFromJSON(obj.data.ToString());
        UpdateDoubloonsEvents(newDoubloons);
    }

    private void HandleEndGame(SocketIOEvent obj)
    {
        var winningPlayer = WinningPlayer.CreateFromJSON(obj.data.ToString());
        EndGameEvent(winningPlayer);
    }

    #endregion

    #region Commands

    public void CommandCreateRoom(string roomName, Action<string> ack)
    {
        m_createRoomAcks += ack;
        this.socket.Emit("create", JSONObject.CreateStringObject(roomName), AckCreateRoom);
    }

    public void CommandJoinRoom(string roomCode)
    {
        this.socket.Emit("join", JSONObject.CreateStringObject(roomCode));
    }

    public void CommandLeaveRoom()
    {
        socket.Emit("leave");
    }
    public void CommandSpawn(string unitType)
    {
        this.socket.Emit("spawn", JSONObject.CreateStringObject(unitType.ToLower()));
    }

    public void CommandDebugSpawn(Vector3 pos, Quaternion rot)
    {
        string data = JsonUtility.ToJson(new PointJSON(pos, rot));
        this.socket.Emit("spawn debug", new JSONObject(data));
    }

    public void CommandTakeTowerDamage(string unitType, int attackedPlayerNo)
    {
        string data = JsonUtility.ToJson(new TowerDamageJSON(unitType, attackedPlayerNo));
        this.socket.Emit("damageCastle", new JSONObject(data));
    }

    public void CommandTakeUnitDamage(int attackerId, int defenderId)
    {
        string data = JsonUtility.ToJson(new UnitDamageJSON(attackerId, defenderId));
        Debug.Log("Sending data: " + data);
        this.socket.Emit("damageUnit", new JSONObject(data));
    }

    #endregion

    #region Command Acknowledgments

    private void AckCreateRoom(JSONObject obj)
    {
        m_createRoomAcks(obj.list[0].str);
        m_createRoomAcks = null;
        UpdateGameStateEvent();
    }
    #endregion

    #region JSON Message Classes

    public class UnitSpawnData
    {
        public int playerNo;
        public Vector3 position;
        public Quaternion rotation;
        public string unitType;
        public int unitId;
        public int spawnTime;

        public UnitSpawnData(int _playerNo, float[] _position, float[] _rotation, string _unitType, int _unitId, int _spawnTime)
        {
            playerNo = _playerNo;
            position = new Vector3(_position[0], _position[1], _position[2]);
            rotation = Quaternion.Euler(_rotation[0], _rotation[1], _rotation[2]);
            unitType = _unitType;
            unitId = _unitId;
            spawnTime = _spawnTime;
        }
    }

    [Serializable]
    public class PointJSON
    {
        public float[] position;
        public float[] rotation;

        public PointJSON(GameObject obj) : this(obj.transform.position, obj.transform.rotation) { }

        public PointJSON(Vector3 _position, Quaternion _rotation)
        {
            position = new float[] {
                _position.x,
                _position.y,
                _position.z
            };

            rotation = new float[] {
                _rotation.eulerAngles.x,
                _rotation.eulerAngles.y,
                _rotation.eulerAngles.z
            };
        }
    }

    [Serializable]
    public class TowerDamageJSON
    {
        public string unitType;
        public int attackedPlayerNo;

        public TowerDamageJSON(string _unitType, int _attackedPlayerNo)
        {
            unitType = _unitType;
            attackedPlayerNo = _attackedPlayerNo;
        }

        public static TowerDamageJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<TowerDamageJSON>(data);
        }
    }

    [Serializable]
    public class JoinToken
    {
        public string joinToken;
        public static JoinToken CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<JoinToken>(data);
        }
    }

    [Serializable]
    public class AttackedPlayerHealth
    {
        public int playerNo;
        public int castleHealth;

        public static AttackedPlayerHealth CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<AttackedPlayerHealth>(data);
        }
    }

    [Serializable]
    public class UnitDamageJSON
    {
        public int attackerId;
        public int defenderId;

        public UnitDamageJSON(int _attackerId, int _defenderId)
        {
            attackerId = _attackerId;
            defenderId = _defenderId;
        }
    }
    
    [Serializable]
    public class WinningPlayer
    {
        public int winningPlayer;

        public static WinningPlayer CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<WinningPlayer>(data);
        }
    }

    [Serializable]
    public class UnitHealthJSON
    {
        public int playerNo;
        public int unitId;
        public float health;

        public static UnitHealthJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UnitHealthJSON>(data);
        }
    }

    [Serializable]
    public class UpdateDoubloons
    {
        public int playerNo;
        public int doubloons;

        public static UpdateDoubloons CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UpdateDoubloons>(data);
        }
    }


    [Serializable]
    public class UnitJSON
    {
        public int playerNo;
        public float[] position;
        public float[] rotation;
        public string unitType;
        public int unitId;
        public int spawnTime;

        public static UnitJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UnitJSON>(data);
        }
    }

    [Serializable]
    public class PlayerJSON
    {
        public int playerNo;
        public int castleHealth;
        public int enemyCastleHealth;
        public int doubloons;
        public CoolDownsJSON coolDowns;

        public static PlayerJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<PlayerJSON>(data);
        }
    }

    [Serializable]
    public class CoolDownsJSON
    {
        public int knight;

        public static CoolDownsJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<CoolDownsJSON>(data);
        }
    }

    #endregion

}
