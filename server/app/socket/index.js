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
      createdTime = gameRoom.meta.created
    }
  });

  return latestGame;
};

module.exports = io => {
  io.on("connection", socket => {
    console.log(
      `A socket connection to npm server has been made: ${socket.id}`
    );

    // Create room and add room creator as P1
    socket.on("create", (roomName, ack) => {
      // Do not create debug room  because it is created on server init.
      if (roomName === "debug" || roomName === "debugAI") {
        return;
      }

      const gameRoom = gameState.createGameRoom(roomName);

      const player1 = gameState.createPlayer(1, socket.id, [1, 1, 1]);
      gameRoom.player1 = player1;

      socket.join(gameRoom.roomId);

      ack(gameRoom.joinToken);
      console.log(`Client ${socket.id} has created room "${roomName}"`);
    });

    socket.on("join", joinToken => {
      console.log(
        `Client ${socket.id} attempting to join game with token: ${joinToken}`
      );

      const gameRoom = gameState.getRoomByToken(joinToken);
      if (!gameRoom) {
        socket.emit("incorrectGameToken");
        return;
      }

      // Prevent client from joining game twice
      if (
        (gameRoom.player1 && gameRoom.player1.socketId === socket.Id) ||
        (gameRoom.player2 && gameRoom.player2.socketId === socket.id)
      ) {
        socket.emit("alreadyJoinedGame");
        console.log(
          `Client ${socket.id} attempting to join alreadyJoinedGame.`
        );
        return;
      }

      let playerAdded;

      if (gameRoom.player1 === null) {
        const player = gameState.createPlayer(1, socket.id, [0, 0, 0]);
        gameRoom.player1 = player;
        playerAdded = player;
      } else if (gameRoom.player2 === null) {
        const player = gameState.createPlayer(2, socket.id, [1, 1, 1]);
        gameRoom.player2 = player;
        playerAdded = player;
      } else {
        const specator = gameState.createSpectator(socket.id);
        gameRoom.spectators.push(specator);
        playerAdded = specator;
      }

      socket.join(gameRoom.roomId);

      // If gameRoom is the debug game room, then add a player2, start the game
      // Otherwise start the game if both p1 and p2 have joined.
      if (joinToken === "debug" || joinToken === "debugAI") {
        if (gameRoom.player2 === null) {
          const player = gameState.createPlayer(2, null, [1, 1, 1]);
          gameRoom.player2 = player;
        }
        io.to(socket.id).emit("start", {
          enemyCastleHealth: gameRoom.player2.castleHealth,
          ...playerAdded
        });
        if (joinToken === "debugAI") {
          const unitTypes = ["knight", "archer", "phallanx"];
          setInterval(() => {
            const position = [0, 0, -3.5];
            const rotation = [0, 0, 0];
            const unit = gameState.createUnit(
              2,
              unitTypes[Math.floor(Math.random() * 3)],
              position,
              rotation
            );
            io.to(gameRoom.roomId).emit("spawn", unit);
          }, 1500);
        }
      } else if (gameRoom.player1 && gameRoom.player2) {
        io.to(gameRoom.player1.socketId).emit('start', { enemyCastleHealth: gameRoom.player2.castleHealth, ...gameRoom.player1 })
        io.to(gameRoom.player2.socketId).emit('start', { enemyCastleHealth: gameRoom.player1.castleHealth, ...gameRoom.player2 })

        gameRoom.interval = setInterval(() => {
          gameRoom.player1.doubloons += 100;
          gameRoom.player2.doubloons += 100;
          io.to(gameRoom.player1.socketId).emit('updatePlayerDoubloons', { playerNo: 1, doubloons: gameRoom.player1.doubloons })
          io.to(gameRoom.player2.socketId).emit('updatePlayerDoubloons', { playerNo: 2, doubloons: gameRoom.player2.doubloons })
        }, 5000)
      }
      console.log(`Client ${socket.id} has joined game. Game has started.`);
    });

    socket.on("spawn", unitType => {
      // Get latest game room
      const gameRoom = getLatestRoom(socket.rooms);
      if (!gameRoom) {
        socket.emit("matchNotFound");
        console.log("room not found");
        return;
      }

      // Get player requesting unit spawn from socket.id
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
      const cost = gameState.unitCost(unitType);
      if (player.doubloons >= cost) {
        // Set position if it is not provided
        const position = player.playerNo === 1 ? [0, 0, 3.5] : [0, 0, -3.5];
        const rotation = player.PlayerNo === 1 ? [0, 180, 0] : [0, 0, 0];

        const unit = gameState.createUnit(
          player.playerNo,
          unitType,
          position,
          rotation
        );
        unit.unitId = unit["$loki"];
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
      // Get latest game room
      console.log(socket.rooms);
      const gameRoom = getLatestRoom(socket.rooms);
      if (!gameRoom) {
        socket.emit("match not found");
        console.log("room not found");
        return;
      }

      const damage = gameState.unitDamage(unitType);

      const attackedPlayer = gameRoom["player" + attackedPlayerNo];
      attackedPlayer.castleHealth -= damage;

      io.to(gameRoom.roomId).emit("damageCastle", {
        playerNo: attackedPlayer.playerNo,
        castleHealth: attackedPlayer.castleHealth
      });

      // Check to see if the game is over
      // Otherwise emit new castleHealth
      if (attackedPlayer.castleHealth <= 0) {
        const winningPlayer = 3 - attackedPlayerNo;
        io.to(gameRoom.roomId).emit("endGame", { winningPlayer });
        clearInterval(gameRoom.interval)
        if (gameRoom.roomName === 'debug') {
          gameState.resetDebugRoom();
        } // else persist to db/delete game room
        else {
          gameRoom.player1 = null;
          gameRoom.player2 = null;
        }
      }
    });

    socket.on("damageUnit", ({ attackerId, defenderId  }) => {
      // Get latest game room
      const gameRoom = getLatestRoom(socket.rooms);
      if (!gameRoom) {
        socket.emit("match not found");
        console.log("room not found");
        return;
      }

      // find attacker and defender
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

      // calculate new unit health
      const damage = gameState.unitDamage(attacker.unitType);
      defender.health -= damage;

      console.log(
        `${attackerId} attacked ${defenderId} for ${damage} leaving it with ${
        defender.health
        } hp`
      );

      // emit new unit health
      io.to(gameRoom.roomId).emit("damageUnit", {
        playerNo: defender.playerNo,
        health: defender.health,
        unitId: defender.$loki
      });

      // remove unit if it's health is 0 or less
    });

    socket.on("leave", () => {
      // Remove every instance of the client in gameState
      // Client could be a player or a spectator
      // gameState.destroyPlayer(socket.id);
      // gameState.destorySpectator(socket.id);

      const roomNames = getAllGameRoomIds(socket.rooms);
      roomNames.forEach(roomName => {
        socket.leave(roomName);
        const gameRoom = gameState.getRoomByRoomId(roomName);

        // Destroy gameRoom if there are no players left;
      });
    });

    socket.on("disconnect", () => {
      const roomIds = getAllGameRoomIds(socket.rooms);
      roomIds.forEach(roomId => {
        socket.leave(roomId);
        const gameRoom = gameState.getRoomByRoomId(roomId);
        if (gameRoom.roomName === 'debug') {
          gameState.resetDebugRoom();
        }

        // Destroy gameRoom if there are no players left;
      });
      
      console.log(`Connection ${socket.id} has left the building`);
    });
  });
};
