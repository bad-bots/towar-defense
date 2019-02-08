const gameState = require("../memDb");

const getAllGameRoomIds = rooms => {
  return Object.keys(rooms).filter(room => room.includes("rId:"));
};

const getLatestRoom = rooms => {
  const roomIds = getAllGameRoomIds(rooms);
  let latestGame;
  let createdTime = -Infinity;

  roomIds.forEach(roomId => {
    const gameRoom = gameState.getRoomByRoomId(roomId);
    if (gameRoom && gameRoom.meta.created > createdTime) {
      latestGame = gameRoom;
      createdTime = gameRoom.meta.created;
    }
  });

  return latestGame;
};

const createRoom = (socketId, roomName, phonePosition) => {
  // Do not create debug room  because it is created on server init.
  if (roomName === "debug" || roomName === "debugAI") {
    return;
  }
  const gameRoom = gameState.createGameRoom(roomName);
  return gameRoom;
};

const clientAlreadyJoined = (gameRoom, socketId) => {
  return (
    (gameRoom.player1 && gameRoom.player1.socketId === socketId) ||
    (gameRoom.player2 && gameRoom.player2.socketId === socketId)
  );
};

const addPlayer = (gameRoom, socketId) => {
  let playerAdded;
  if (gameRoom.player1 === null) {
    playerAdded = gameState.createPlayer(1, socketId, [0, 0, 0]);
    gameRoom.player1 = playerAdded;
  } else if (gameRoom.player2 === null) {
    playerAdded = gameState.createPlayer(2, socketId, [1, 1, 1]);
    gameRoom.player2 = playerAdded;
  } else {
    playerAdded = gameState.createSpectator(socketId);
    gameRoom.spectators.push(playerAdded);
  }
  return playerAdded;
};

const autoSpawnUnits = (io, gameRoom, playerNo, spawnInterval) => {
  const unitTypes = ["knight", "archer", "phallanx"];

  gameRoom.autoSpawnInterval = setInterval(() => {
    const [position, rotation] = getSpawnPosAndRot(playerNo);
    const unit = gameState.createUnit(
      playerNo,
      unitTypes[Math.floor(Math.random() * 3)],
      position,
      rotation
    );

    io.to(gameRoom.roomId).emit("spawn", unit);
  }, spawnInterval);
};

const getSpawnPosAndRot = playerNo => {
  const position = player.playerNo === 1 ? [0, 0, 3.5] : [0, 0, -3.5];
  const rotation = player.PlayerNo === 1 ? [0, 180, 0] : [0, 0, 0];
  return [position, rotation];
};

const spawnUnit = playerNo => {
  const [postion, rotation] = getSpawnPosAndRot(playerNo);
  const unit = gameState.createUnit(
    player.playerNo,
    unitType,
    position,
    rotation
  );
  unit.unitId = unit["$loki"];
  return unit;
};

const autoGenDoubloons = (io, gameRoom) => {
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
};

const endGameCleanUp = gameRoom => {
  clearInterval(gameRoom.interval);
  gameRoom.interval = null;

  if (gameRoom.roomName === "debug" || gameRoom.roomName === "debugAi") {
    gameState.resetDebugRoom(gameRoom.roomName);
  } else {
    // persist to db/delete game room here
    gameState.destroyPlayer(gameRoom.player1);
    gameState.destroyPlayer(gameRoom.player2);
    gameRoom.player1 = null;
    gameRoom.player2 = null;
  }
};

const clientLeave = (rooms, socket) => {
  gameState.destroyPlayer(socket.id);
  gameState.destorySpectator(socket.id);

  const roomIds = getAllGameRoomIds(rooms);
  roomIds.forEach(roomId => {
    socket.leave(roomId);
    const gameRoom = gameState.getRoomByRoomId(roomId);
    if (gameRoom) {
      clientLeaveCleanUp(gameRoom, socket.id);
    }
  });
};

const clientLeaveCleanUp = (gameRoom, socketId) => {
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
      gameState.resetDebugRoom(gameRoom.roomName);
    } else {
      gameState.destroyGame(gameRoom.roomId);
    }
  }
};

module.exports = {
  getAllGameRoomIds,
  getLatestRoom,
  createRoom,
  clientAlreadyJoined,
  addPlayer,
  autoSpawnUnits,
  spawnUnit,
  autoGenDoubloons,
  endGameCleanUp,
  clientLeave,
  clientLeaveCleanUp
};
