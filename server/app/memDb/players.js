const db = require("./db");

const Player = db.addCollection("player");

Player.createPlayer = function(playerNo, socketId, phonePosition) {
  if (playerNo === 3) {
    return this.insert({
      socketId,
      playerNo,
      phonePosition
    });
  } else {
    return this.insert({
      socketId,
      playerNo,
      castleHealth: 1000,
      doubloons: 10000,
      phonePosition,
      coolDowns: {
        knight: 0
      }
    });
  }
};

Player.getPlayer = function(socketId) {
  return this.findOne({ socketId });
};

Player.destroyPlayer = function(socketId) {
  this.findAndRemove({ socketId });
};

module.exports = Player;
