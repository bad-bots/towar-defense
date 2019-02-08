const Room = require("./rooms");
const Player = require("./players");
const Unit = require("./units");
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

class GameState {
  constructor() {
    this.debugRoom = Room._createGameRoom("debug", "debug");
    this.debugAIRoom = Room._createGameRoom("debugAI", "debugAI");
  }

  resetDebugRoom(type) {
    const roomType = type === "AI" ? "debugAIRoom" : "debugRoom";
    Room.findAndRemove({ roomId: this[roomType].roomId });
    const name = type === "AI" ? "debugAI" : "debug";
    if (type === "AI") {
      clearInterval(this[roomType].autoSpawnInterval);
      this.autoSpawnInterval = null;
    }
    this[roomType] = Room._createGameRoom(name, name);
  }

  getAllGameRoomIds(rooms) {
    return Object.keys(rooms).filter(room => room.includes("rId:"));
  }

  getLatestRoom(rooms) {
    const roomIds = this.getAllGameRoomIds(rooms);
    let latestGame;
    let createdTime = -Infinity;

    roomIds.forEach(roomId => {
      const gameRoom = Room.getRoomByRoomId(roomId);
      if (gameRoom && gameRoom.meta.created > createdTime) {
        latestGame = gameRoom;
        createdTime = gameRoom.meta.created;
      }
    });

    return latestGame;
  }

  startGame(gameRoom) {
    gameRoom.gameStatus = "inPlay";
  }

  pauseGame(gameRoom) {
    gameRoom.gameStatus = "paused";
  }

  endGame(gameRoom, playerNo) {
    gameRoom.gameStatus = "finished";
    gameRoom.winner = playerNo;
  }

  clientAlreadyJoined(gameRoom, socketId) {
    return (
      (gameRoom.player1 && gameRoom.player1.socketId === socketId) ||
      (gameRoom.player2 && gameRoom.player2.socketId === socketId)
    );
  }

  getPlayerNo(gameRoom) {
    if (gameRoom.player1 === null) {
      return 1;
    } else if (gameRoom.player2 === null) {
      return 2;
    } else {
      return 3;
    }
  }

  autoSpawnUnits(io, gameRoom, playerNo, spawnInterval = 1500) {
    const unitTypes = ["knight", "archer", "phallanx"];

    gameRoom.autoSpawnInterval = setInterval(() => {
      const [position, rotation] = this.getSpawnPosAndRot(playerNo);
      const unitType = unitTypes[Math.floor(Math.random() * 3)];
      const unit = this.spawnUnit(playerNo, unitType)
      gameRoom.units.push(unit);
      io.to(gameRoom.roomId).emit("spawn", unit);
    }, spawnInterval);
  }

  getSpawnPosAndRot(playerNo) {
    const position = playerNo === 1 ? [0, 0, 3.5] : [0, 0, -3.5];
    const rotation = playerNo === 1 ? [0, 180, 0] : [0, 0, 0];
    return [position, rotation];
  }

  spawnUnit(playerNo, unitType) {
    const [position, rotation] = this.getSpawnPosAndRot(playerNo);
    const unit = Unit.createUnit(playerNo, unitType, position, rotation);
    unit.unitId = unit["$loki"];
    return unit;
  }

  autoGenDoubloons(io, gameRoom) {
    gameRoom.interval = setInterval(() => {
      gameRoom.player1.doubloons += 100;
      gameRoom.player2.doubloons += 100;
      io.to(gameRoom.player1.socketId).emit("updatePlayerDoubloons", {
        playerNo: 1,
        doubloons: gameRoom.player1.doubloons
      });
      io.to(gameRoom.player2.socketId).emit("updatePlayerDoubloons", {
        playerNo: 2,
        doubloons: gameRoom.player2.doubloons
      });
    }, 5000);
  }

  endGameCleanUp(gameRoom) {
    clearInterval(gameRoom.interval);
    gameRoom.interval = null;

    if (gameRoom.roomName === "debug" || gameRoom.roomName === "debugAI") {
      this.resetDebugRoom(gameRoom.roomName);
    } else {
      // persist to db/delete game room here
    }
  }

  clientLeave(rooms, socket) {
    Player.destroyPlayer(socket.id);

    const roomIds = this.getAllGameRoomIds(rooms);
    roomIds.forEach(roomId => {
      socket.leave(roomId);
      const gameRoom = Room.getRoomByRoomId(roomId);
      if (gameRoom) {
        this.clientLeaveCleanUp(gameRoom, socket.id);
      }
    });
  }

  clientLeaveCleanUp(gameRoom, socketId) {
    if (gameRoom.player1 && gameRoom.player1.socketId === socketId) {
      gameRoom.player1 = null;
    }
    if (gameRoom.player2 && gameRoom.player2.socketId === socketId) {
      gameRoom.player2 = null;
    }
    gameRoom.spectators = gameRoom.spectators.filter(
      spectator => spectator.socketId !== socketId
    );

    if (
      gameRoom.player1 === null &&
      gameRoom.player2 === null &&
      gameRoom.spectators.length === 0
    ) {
      if (gameRoom.roomName === "debug" || gameRoom.roomName === "debugAI") {
        this.resetDebugRoom(gameRoom.roomName);
      } else {
        Room.destroyRoom(gameRoom.roomId);
      }
    }
  }
}

const gameState = new GameState();

module.exports = { gameState, Room, Player, Unit };
