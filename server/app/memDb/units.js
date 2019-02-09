const db = require("./db");

const Unit = db.addCollection("units");

Unit.unitCost = function(type) {
  switch (type) {
    case "archer":
      return 150;
    case "knight":
      return 125;
    case "phallanx":
      return 175;
    default:
      return 100;
  }
};

Unit.unitDamage = function(type) {
  switch (type) {
    case "archer":
      return 140;
    case "knight":
      return 125;
    case "phallanx":
      return 105;
    default:
      return 100;
  }
};

getMaxHealth = function(type) {
  switch (type) {
    case "archer":
      return 600;
    case "knight":
      return 950;
    case "phallanx":
      return 1250;
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