const loki = require("lokijs");
const inMemDb = new loki("games.json");

/**
 *  GameRoom = {
 *  id: 1 //auto created
 *  player1: {
 *      id: 1 //auto created
 *      socketId: sockdeId
 *      castleHealth: 1000,
 *      doubloons: 10000,
 *      phonePosition: [1,2,3],
 *      coolDowns: {
 *          unitType: time
 *      }
 *  },
 *  player2: { ... },
 *  roomId: roomId,
 *  gameStatus: 'inPlay', // or ['paused','finished']
 *  units: [
 *      {
 *          id: 1
 *          health: 100,
 *          position: [1,12,3],
 *          rotation: [0,0,0].
 *          playerId: 1,
 *          unitType: 'archer',
 *          spawnTime: 100000
 *          currentTarget: 2
 *      },
 *      { ... }
 *      ],
 * })
 * }
 *
 * Insert
 * rooms.insert({roomId: 'asdfa', joinToken: 1})
 *
 * Find
 * rooms.find({player1: 55});
 * rooms.findOne({player2: 23});
 *
 * Update
 * rooms.findAndUpdate({roomId: 234}, )
 * rooms.update({})
 * rooms.updateWhere(obj => {
 *  obj.gameId === 55
 * }, {...})
 */

class MemDB {
  constructor() {
    this.Player = inMemDb.addCollection("player");
    this.Spectator = inMemDb.addCollection("spectators");
    this.Room = inMemDb.addCollection("rooms");
    this.RoomId = inMemDb.addCollection("roomIds");
    this.Unit = inMemDb.addCollection("units");
    this.JoinToken = inMemDb.addCollection("joinTokens");

    this.debugRoom = this.createGameRoom("debug", "debug");
    this.debugAIRoom = this.createGameRoom("debugAI", "debugAI");
  }

  resetDebugRoom() {
    this.Room.findAndRemove({roomId: this.debugRoom.roomId});
    this.debugRoom = this.createGameRoom('debug', 'debug');
  }

  // Room Methods
  getRoomId(roomName) {
    let roomId;
    while (!roomId) {
      const temp =
        "rId:" +
        Math.random()
          .toString(36)
          .substring(2);
      const roomIdExists = this.RoomId.findOne({ roomId: temp });
      if (!roomIdExists) {
        roomId = temp;
      }
    }
    this.RoomId.insert({ roomId, roomName });
    return roomId;
  }

  genJoinToken(roomName) {
    let joinToken;
    while (!joinToken) {
      const temp = Math.random()
        .toString(36)
        .substring(8);
      const tokenFound = this.JoinToken.findOne({ token: temp });
      if (!tokenFound) {
        joinToken = temp;
      }
    }
    this.JoinToken.insert({ token: joinToken, roomName });
    return joinToken;
  }

  createGameRoom(roomName, joinToken = null) {
    const roomId = this.getRoomId(roomName);
    joinToken = joinToken || this.genJoinToken(roomName);

    return this.Room.insert({
      roomId,
      joinToken,
      roomName,
      gameStatus: "pending",
      winner: null,
      player1: null,
      player2: null,
      spectators: [],
      units: [],
      interval: null
    });
  }

  getRoomByToken(joinToken) {
    return this.Room.findOne({ joinToken });
  }

  getRoomByRoomId(roomId) {
    return this.Room.findOne({ roomId });
  }

  // Player Methods
  createPlayer(playerNo, socketId, phonePosition) {
    return this.Player.insert({
      socketId,
      playerNo,
      castleHealth: 100,
      doubloons: 500,
      phonePosition,
      coolDowns: {
        knight: 0
      }
    });
  }

  createSpectator(socketId) {
    return this.Spectator.insert({
      socketId
    });
  }

  getPlayer(socketId) {
    return this.Player.findOne({ socketId });
  }

  destroyPlayer(socketId) {
    this.Player.findAndRemove({ socketId });
  }

  destorySpectator(socketId) {
    this.Spectator.findAndRemove({ socketId });
  }

  // Game methods
  startGame(gameRoom) {
    gameRoom.gameStatus = "inPlay";
  }

  pauseGame(gameRoom) {
    gameRoom.gameStatus = "paused";
  }

  endGame(gameRoom, playerNo) {
    gameRoom.gameStatus = "finished";
    gameRoom.winner = playerNo
  }

  destroyGame(roomId) {
    this.Room.findAndRemove({ roomId });
  }

  // Unit methods
  unitCost(type) {
    switch (type) {
      case "archer":
        return 100;
      case "knight":
        return 150;
      case "phallanx":
        return 200;
      default:
        return 100;
    }
  }

  unitDamage(type) {
    switch (type) {
      case "archer":
        return 100;
      case "knight":
        return 150;
      case "phallanx":
        return 200;
      default:
        return 100;
    }
  }

  createUnit(playerNo, unitType, position, rotation) {
    return this.Unit.insert({
      health: 200,
      position,
      rotation,
      unitType,
      playerNo,
      currentTarget: null,
      spawnTime: 5000
    });
  }

  destroyUnit(unitId) {
    this.Unit.findAndRemove({ id: unitId });
  }
}

const db = new MemDB();

module.exports = db;
