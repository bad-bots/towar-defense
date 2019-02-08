const { gameState, Room, Player, Unit } = require("../memDb");
const utils = require("./socketUtils");

module.exports = io => {
  io.on("connection", socket => {
    console.log(
      `A socket connection to npm server has been made: ${socket.id}`
    );

    // Create room given a roomName and add room creator as P1
    socket.on("create", roomName => {
      const phonePosition = [1, 1, 1]; // <- not correct. Position comes from client
      const gameRoom = Room.createGameRoom(roomName);

      const player1 = Player.createPlayer(1, socket.id, phonePosition);
      gameRoom.player1 = player1;
      socket.join(gameRoom.roomId);
      socket.emit("init", { joinToken: gameRoom.joinToken });
    });

    // Join room with joinToken
    socket.on("join", joinToken => {
      const gameRoom = Room.getRoomByToken(joinToken);
      if (!gameRoom) {
        socket.emit("incorrectGameToken");
        return;
      }

      // Prevent client from joining game twice
      if (gameState.clientAlreadyJoined(gameRoom, socket.id)) {
        socket.emit("alreadyJoinedGame");
        return;
      }

      // Add player to game state and client to room.
      const playerNo = gameState.getPlayerNo(gameRoom);
      const phonePosition = [0, 0, 0]; // Not correct
      const player = Player.createPlayer(playerNo, socket.id, phonePosition);
      // Set player and join room
      if (playerNo === 3) {
        gameRoom.spectators.push(player);
      } else {
        gameRoom["player" + playerNo] = player;
      }
      socket.join(gameRoom.roomId);

      // Start room
      if (joinToken === "debug" || joinToken === "debugAI") {
        // Fill enemy player with bot.
        const enemyPlayerNo = gameState.getPlayerNo(gameRoom);
        if (enemyPlayerNo !== 3) {
          const debugEnemyPlayer = Player.createPlayer(enemyPlayerNo, 'debugAI');
          gameRoom["player" + enemyPlayerNo] = debugEnemyPlayer;
          if (joinToken === "debugAI") {
            gameState.autoSpawnUnits(io, gameRoom, debugEnemyPlayer.playerNo, 2000);
          }
        }

        io.to(socket.id).emit("start", {
          enemyCastleHealth: 1000,
          ...player
        });
        gameState.autoGenDoubloons(io, gameRoom);
      } // Start game if 2 players have joined
      else if (gameRoom.player1 && gameRoom.player2) {
        io.to(gameRoom.player1.socketId).emit("start", {
          enemyCastleHealth: gameRoom.player2.castleHealth,
          ...gameRoom.player1
        });
        io.to(gameRoom.player2.socketId).emit("start", {
          enemyCastleHealth: gameRoom.player1.castleHealth,
          ...gameRoom.player2
        });
        gameState.autoGenDoubloons(io, gameRoom);
      }
    });

    socket.on("spawn", unitType => {
      // Get latest game room
      const gameRoom = gameState.getLatestRoom(socket.rooms);
      if (!gameRoom) {
        socket.emit("matchNotFound");
        console.log("room not found");
        return;
      }

      // Get player requesting attack
      let player;
      if (socket.id === gameRoom.player1.socketId) {
        player = gameRoom.player1;
      } else if (socket.id === gameRoom.player2.socketId) {
        player = gameRoom.player2;
      } else {
        socket.emit("unauthorizedPlayer");
        console.log("player not found");
        return;
      }

      // Spawn unit if player has enough doubloons
      const cost = Unit.unitCost(unitType);
      if (player.doubloons >= cost) {
        unit = gameState.spawnUnit(player.playerNo, unitType);
        player.doubloons -= cost;
        gameRoom.units.push(unit);
        // Update player doubloons
        // Spawn unit
        socket.emit("updatePlayerDoubloons", {
          playerNo: player.playerNo,
          doubloons: player.doubloons
        });
        io.to(gameRoom.roomId).emit("spawn", unit);
      } else {
        socket.emit("insufficientDoubloons");
      }
    });

    socket.on("damageCastle", ({ unitType, attackedPlayerNo }) => {
      const gameRoom = gameState.getLatestRoom(socket.rooms);
      if (!gameRoom) {
        socket.emit("matchNotFound");
        console.log("room not found");
        return;
      }
      // Apply damage
      const damage = Unit.unitDamage(unitType);
      const attackedPlayer = gameRoom["player" + attackedPlayerNo];
      attackedPlayer.castleHealth -= damage;

      // Emit new castle health
      io.to(gameRoom.roomId).emit("damageCastle", {
        playerNo: attackedPlayer.playerNo,
        castleHealth: attackedPlayer.castleHealth
      });

      // Check to see if the game is over
      if (attackedPlayer.castleHealth <= 0) {
        const winningPlayer = 3 - attackedPlayerNo;
        io.to(gameRoom.roomId).emit("endGame", { winningPlayer });
        gameState.endGameCleanUp(gameRoom);
      }
    });

    socket.on("damageUnit", ({ attackerId, defenderId }) => {
      const gameRoom = gameState.getLatestRoom(socket.rooms);
      if (!gameRoom) {
        socket.emit("match not found");
        console.log("room not found");
        return;
      }
      console.log(attackerId);
      // Find attacker and defender
      const attacker = gameRoom.units.find(unit => unit.$loki === +attackerId);
      if (!attacker) {
        console.log("Attacking unit not found");
        return;
      }
      const defender = gameRoom.units.find(unit => unit.$loki === +defenderId);
      if (!defender) {
        console.log("Defending unit not found");
        return;
      }

      // Calculate new unit health
      const damage = Unit.unitDamage(attacker.unitType);
      defender.health -= damage;

      console.log(
        `${attackerId} attacked ${defenderId} for ${damage} leaving it with ${
          defender.health
        } hp`
      );

      // Emit new unit health
      io.to(gameRoom.roomId).emit("damageUnit", {
        playerNo: defender.playerNo,
        health: defender.health,
        unitId: defender.$loki
      });

      // Destroy unit
      if (defender.health <= 0) {
        Unit.destroyUnit(defender.$loki);
        gameRoom.units = gameRoom.units.filter(
          unit => unit.$loki !== defender.$loki
        );
      }
    });

    // Remove every instance of the client in gameState
    // Client could be a player or a spectator
    socket.on("leave", () => {
      // Remove player from database
      gameState.clientLeave(socket.rooms, socket);
      console.log(`Connection ${socket.id} has left a game`);
    });

    socket.on("disconnect", () => {
      gameState.clientLeave(socket.rooms, socket);
      console.log(`Connection ${socket.id} has left the building`);
    });
  });
};
