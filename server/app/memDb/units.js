const db = require("./db");

const Unit = db.addCollection("units");

Unit.unitCost = function(type) {
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
};

Unit.unitDamage = function(type) {
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
};

Unit.createUnit = function(playerNo, unitType, position, rotation) {
  return this.insert({
    health: 200,
    position,
    rotation,
    unitType,
    playerNo,
    currentTarget: null,
    spawnTime: 5000
  });
};

Unit.destroyUnit = function(unitId) {
  this.findAndRemove({ id: unitId });
};

module.exports = Unit