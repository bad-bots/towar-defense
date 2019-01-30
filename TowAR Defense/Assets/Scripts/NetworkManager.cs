using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance = null;

    public Action onSpawnKnight = null;

    #region Private Members
    private SocketIOComponent socket;

    private Action<Vector3, Quaternion> m_spawnKnightListeners;
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

        socket.On("spawn unit", HandleSpawnKnight);
    }

    #endregion /* MONOBEHAVIOUR_METHODS */

    #region SOCKET_METHODS

    [Obsolete("Direct emitting is deprecated, please use command methods instead")]
    public void SendEvent(string evt)
    {
        socket.Emit(evt);
    }

    [Obsolete("Direct emitting is deprecated, please use command methods instead")]
    public void SendEvent(string evt, JSONObject data)
    {
        socket.Emit(evt, data);
    }

    [Obsolete("Direct emitting is deprecated, please use command methods instead")]
    public void SendEvent(string evt, Action<JSONObject> action)
    {
        socket.Emit(evt, action);
    }

    [Obsolete("Direct emitting is deprecated, please use command methods instead")]
    public void SendEvent(string evt, JSONObject data, Action<JSONObject> action)
    {
        socket.Emit(evt, data, action);
    }

    #endregion /* SOCKET_METHODS */

    #region Event Registration
    public void RegisterSpawnKnightListener(Action<Vector3, Quaternion> handler)
    {
        m_spawnKnightListeners += handler;
    }
    public void UnregisterSpawnKnightListener(Action<Vector3, Quaternion> handler)
    {
        m_spawnKnightListeners -= handler;
    }
    #endregion

    #region Event Handlers

    private void HandleSpawnKnight(SocketIOEvent obj)
    {
        string data = obj.data.ToString();
        var json = UnitJSON.CreateFromJSON(data);
        var pos = new Vector3(json.position[0], json.position[1], json.position[2]);
        var rot = Quaternion.Euler(json.rotation[0], json.rotation[1], json.rotation[2]);
        m_spawnKnightListeners( pos, rot );
    }

    #endregion 

    #region Commands

    public void CommandCreateRoom(Action<JSONObject> ack)
    {
        this.socket.Emit("create room", ack);
    }

    public void CommandJoinRoom(string roomCode, Action<JSONObject> ack)
    {
        string data = JsonUtility.ToJson(new JoinRoomJSON(roomCode));
        this.socket.Emit("join room", new JSONObject(data), ack);
    }

    public void CommandSpawn(Vector3 pos)
    {
        CommandSpawn(pos, Quaternion.identity);
    }

    public void CommandSpawn(Vector3 pos, Quaternion rot)
    {
        string data = JsonUtility.ToJson(new PointJSON(pos, rot));
        this.socket.Emit("spawn unit", new JSONObject(data));
    }

    #endregion

    #region JSON Message Classes

    [Serializable]
    public class JoinRoomJSON
    {
        public string roomCode;

        public JoinRoomJSON(string _roomCode)
        {
            roomCode = _roomCode;
        }

        public static JoinRoomJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<JoinRoomJSON>(data);
        }
    }

    [Serializable]
    public class PlayerJSON
    {
        public string name;

        public PlayerJSON(string _name)
        {
            name = _name;
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
    public class PositionJSON
    {
        public float[] position;
        public PositionJSON(Vector3 _position)
        {
            position = new float[] { _position.x, _position.y, _position.z };
        }
    }

    [Serializable]
    public class RotationJSON
    {
        public float[] rotation;
        public RotationJSON(Quaternion _rotation)
        {
            rotation = new float[] { _rotation.eulerAngles.x, _rotation.eulerAngles.y, _rotation.eulerAngles.z };
        }
    }

    [Serializable]
    public class UnitJSON
    {
        public float[] position;
        public float[] rotation;

        public static UnitJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UnitJSON>(data);
        }
    }
    #endregion

}
