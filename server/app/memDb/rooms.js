const db = require("./db");

const Room = db.addCollection("rooms");
const RoomId = db.addCollection("roomIds");
const JoinToken = db.addCollection("joinTokens");

// Don't use arrow to define new methods

Room.getRoomId = function(roomName) {
  let roomId;
  while (!roomId) {
    const temp =
      "rId:" +
      Math.random()
        .toString(36)
        .substring(2);
    const roomIdExists = RoomId.findOne({ roomId: temp });
    if (!roomIdExists) {
      roomId = temp;
    }
  }
  RoomId.insert({ roomId, roomName });
  return roomId;
};

Room.genJoinToken = function(roomName) {
  let joinToken;
  while (!joinToken) {
    const temp = Math.random()
      .toString(36)
      .substring(8);
    const tokenFound = JoinToken.findOne({ token: temp });
    if (!tokenFound) {
      joinToken = temp;
    }
  }
  JoinToken.insert({ token: joinToken, roomName });
  return joinToken;
};

Room.createGameRoom = function(roomName) {
  // Do not create debug room  because it is created on server init.
  if (roomName === "debug" || roomName === "debugAI") {
    return;
  }
  const gameRoom = this._createGameRoom(roomName);
  return gameRoom;
};

Room._createGameRoom = function(roomName, joinToken = null) {
  const roomId = this.getRoomId(roomName);
  joinToken = joinToken || this.genJoinToken(roomName);

  return this.insert({
    roomId,
    joinToken,
    roomName,
    gameStatus: "pending",
    winner: null,
    player1: null,
    player2: null,
    spectators: [],
    units: [],
    interval: null,
    autoSpawnInterval: null
  });
};

Room.getRoomByToken = function(joinToken) {
  return Room.findOne({ joinToken });
};

Room.getRoomByRoomId = function(roomId) {
  return Room.findOne({ roomId });
};

Room.destroyRoom = function(roomId) {
  Room.findAndRemove({ roomId });
};

module.exports = Room;
