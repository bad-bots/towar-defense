const app = require("./app");
const socketio = require("socket.io");

const PORT = process.env.SERVER_PORT || 8080;

(() => {
    try {
        const server = app.listen(PORT, () => {
            let address = PORT;
            if (process.env.NODE_ENV === "development") {
              address = `${require("ip").address("public", "ipv4")}:${PORT}`;
            }
            console.log("Listening on port", address);
          });
          
          const io = socketio(server);
          require("./app/socket")(io);
    } catch (err) {
        console.error("Unable to start server", err);
    }
})()
