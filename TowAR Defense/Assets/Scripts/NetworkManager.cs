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
    public event Action<Vector3, Quaternion, bool> SpawnKnightEvent;
    public event Action<PlayerJSON> StartGameEvent;
    public event Action IncorrectRoomCodeEvent;
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

        socket.On("spawn", HandleSpawnKnight);
        socket.On("incorrectGameToken", HandleIncorrectRoomCode);
        socket.On("start", HandleStartGame);
    }

    #endregion /* MONOBEHAVIOUR_METHODS */

    #region Event Handlers

    private void HandleSpawnKnight(SocketIOEvent obj)
    {
        string data = obj.data.ToString();
        var json = UnitJSON.CreateFromJSON(data);
        var pos = new Vector3(json.position[0], json.position[1], json.position[2]);
        var rot = Quaternion.Euler(json.rotation[0], json.rotation[1], json.rotation[2]);
        var isPlayer1 = json.playerNo == 1;
        SpawnKnightEvent(pos, rot, isPlayer1);
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

    #endregion

    #region Commands

    public void CommandCreateRoom(string roomName, Action<string> ack)
    {
        m_createRoomAcks += ack;
        this.socket.Emit("init", JSONObject.CreateStringObject(roomName), AckCreateRoom);
    }

    public void CommandJoinRoom(string roomCode)
    {
        this.socket.Emit("start", JSONObject.CreateStringObject(roomCode));
    }

    public void CommandSpawn(Vector3 pos)
    {
        CommandSpawn(pos, Quaternion.identity);
    }

    public void CommandSpawn(Vector3 pos, Quaternion rot)
    {
        string data = JsonUtility.ToJson(new PointJSON(pos, rot));
        this.socket.Emit("spawn", new JSONObject(data));
    }

    public void CommandDebugSpawn(Vector3 pos, Quaternion rot)
    {
        string data = JsonUtility.ToJson(new PointJSON(pos, rot));
        this.socket.Emit("spawn debug", new JSONObject(data));
    }

    public void CommandTakeTowerDamage(string unitType)
    {
        this.socket.Emit("damage castle", JSONObject.CreateStringObject(unitType));
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

    [Serializable]
    public class TowerDamageJSON
    {
        public int damage;
        public string playerName;

        public TowerDamageJSON(int _damage = 1)
        {
            damage = _damage;
        }

        public static TowerDamageJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<TowerDamageJSON>(data);
        }
    }
    #endregion

}
