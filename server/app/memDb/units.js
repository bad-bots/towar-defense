const db = require("./db");

const Unit = db.addCollection("units");

Unit.unitCost = function(type) {
  switch (type) {
    case "archer":
      return 150;
    case "knight":
      return 100;
    case "phallanx":
      return 125;
    default:
      return 100;
  }
};

Unit.unitDamage = function(type) {
  switch (type) {
    case "archer":
      return 150;
    case "knight":
      return 125;
    case "phallanx":
      return 100;
    default:
      return 100;
  }
};

getMaxHealth = function(type) {
  switch (type) {
    case "archer":
      return 500;
    case "knight":
      return 750;
    case "phallanx":
      return 1050;
    default:
      return 500;
  }
};

Unit.createUnit = function(playerNo, unitType, position, rotation) {
  let health = getMaxHealth(unitType);
  return this.insert({
    health,
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