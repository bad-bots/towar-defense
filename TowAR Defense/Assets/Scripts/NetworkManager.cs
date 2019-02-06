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

    #region Private Members
    private SocketIOComponent socket;

    private Action<string> m_createRoomAcks;
    #endregion

    #region Socket Events
    public event Action<AttackedPlayerHealth> UpdateCastleHealth;
    public event Action<string, Vector3, Quaternion, bool> SpawnUnitEvent;
    public event Action<PlayerJSON> StartGameEvent;
    public event Action IncorrectRoomCodeEvent;
    public event Action<WinningPlayer> EndGameEvent;

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

        socket.On("spawn", HandleSpawnUnit);
        socket.On("incorrectGameToken", HandleIncorrectRoomCode);
        socket.On("start", HandleStartGame);
        socket.On("damageCastle", HandleDamageCastle);
        socket.On("endGame", HandleEndGame);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GameScene")
        {
            Debug.LogWarning("Started Game in Game Scene. Joining debug room");
            StartCoroutine(JoinDebugRoom());
        }
    }

    #endregion /* MONOBEHAVIOUR_METHODS */

    #region Event Handlers

    private IEnumerator JoinDebugRoom()
    {
        yield return new WaitForEndOfFrame();
        CommandJoinRoom("debug");
    }

    private void HandleSpawnUnit(SocketIOEvent obj)
    {
        string data = obj.data.ToString();
        var json = UnitJSON.CreateFromJSON(data);
        var pos = new Vector3(json.position[0], json.position[1], json.position[2]);
        var rot = Quaternion.Euler(json.rotation[0], json.rotation[1], json.rotation[2]);
        var isPlayer1 = json.playerNo == 1;
        var unitType = json.unitType != null ?
             json.unitType[0].ToString().ToUpper() + json.unitType.Substring(1) :
             "Knight";
        SpawnUnitEvent(unitType, pos, rot, isPlayer1);
    }

    private void HandleStartGame(SocketIOEvent obj)
    {
        var playerData = PlayerJSON.CreateFromJSON(obj.data.ToString());
        StartGameEvent(playerData);
    }

    private void HandleIncorrectRoomCode(SocketIOEvent obj)
    {
        IncorrectRoomCodeEvent();
    }

    private void HandleDamageCastle(SocketIOEvent obj)
    {
        var attackedPlayer = AttackedPlayerHealth.CreateFromJSON(obj.data.ToString());
        UpdateCastleHealth(attackedPlayer);
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

    public void CommandLeaveRoom()
    {
        socket.Emit("leave");
    }

    public void CommandRestartGame()
    {
        socket.Emit("restart");
    }

    #endregion

    #region Command Acknowledgments

    private void AckCreateRoom(JSONObject obj)
    {
        m_createRoomAcks(obj.list[0].str);
        m_createRoomAcks = null;
    }

    #endregion

    #region JSON Message Classes

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
    public class WinningPlayer
    {
        public int winningPlayer;

        public static WinningPlayer CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<WinningPlayer>(data);
        }
    }


    [Serializable]
    public class UnitJSON
    {
        public int playerNo;
        public float[] position;
        public float[] rotation;
        public string unitType;

        public UnitJSON(Vector3 _position, Quaternion _rotation, string _unitType)
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

            unitType = _unitType;
        }

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
