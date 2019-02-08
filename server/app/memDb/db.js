const loki = require("lokijs");
const inMemDb = new loki("games.json");

module.exports = inMemDb;