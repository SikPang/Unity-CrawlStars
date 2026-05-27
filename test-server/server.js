const express = require("express");
const cors = require("cors");
const { WebSocketServer } = require("ws");

const app = express();
const port = 3000;

app.use(cors());
app.use(express.json());

// 임시 JWT
const TEST_TOKEN = "test-jwt-token";

// REST: 헬스 체크
app.get("/health", (req, res) => {
  res.json({
    ok: true,
    message: "REST server is running",
    time: new Date().toISOString()
  });
});

// REST: 로그인
app.post("/auth/login", (req, res) => {
  const { email, password } = req.body;

  if (!email || !password) {
    return res.status(400).json({
      error: "email and password are required"
    });
  }

  res.json({
    accessToken: TEST_TOKEN,
    refreshToken: "test-refresh-token",
    userId: 1,
    nickname: "UnityTester"
  });
});

// REST: JWT 필요한 API
app.get("/users/me", (req, res) => {
  const authorization = req.headers.authorization;

  if (authorization !== `Bearer ${TEST_TOKEN}`) {
    return res.status(401).json({
      error: "Unauthorized"
    });
  }

  res.json({
    id: 1,
    email: "test@example.com",
    nickname: "UnityTester",
    level: 10
  });
});

const server = app.listen(port, () => {
  console.log(`REST server running: http://localhost:${port}`);
  console.log(`WebSocket server running: ws://localhost:${port}/ws`);
});

// WebSocket 서버
const wss = new WebSocketServer({
  server,
  path: "/ws"
});

wss.on("connection", (socket, request) => {
  console.log("WebSocket connected");

  socket.send(JSON.stringify({
    type: "CONNECTED",
    message: "WebSocket connected successfully"
  }));

  socket.on("message", (data) => {
    const text = data.toString();
    console.log("Received:", text);

    let parsed;

    try {
      parsed = JSON.parse(text);
    } catch {
      socket.send(JSON.stringify({
        type: "ERROR",
        message: "Invalid JSON"
      }));
      return;
    }

    if (parsed.type === "AUTH") {
      if (parsed.accessToken === TEST_TOKEN) {
        socket.send(JSON.stringify({
          type: "AUTH_SUCCESS",
          message: "WebSocket auth success"
        }));
      } else {
        socket.send(JSON.stringify({
          type: "AUTH_FAILED",
          message: "Invalid token"
        }));
      }

      return;
    }

    if (parsed.type === "PING") {
      socket.send(JSON.stringify({
        type: "PONG",
        time: new Date().toISOString()
      }));

      return;
    }

    if (parsed.type === "CHAT") {
      socket.send(JSON.stringify({
        type: "CHAT",
        senderId: "server",
        message: `echo: ${parsed.message}`
      }));

      return;
    }

    socket.send(JSON.stringify({
      type: "ECHO",
      received: parsed
    }));
  });

  socket.on("close", () => {
    console.log("WebSocket disconnected");
  });
});