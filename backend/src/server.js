require("dotenv").config();
const express = require("express");
const cors = require("cors");
const bcrypt = require("bcrypt");
const jwt = require("jsonwebtoken");
const { PrismaClient } = require("@prisma/client");
const { PrismaBetterSqlite3 } = require("@prisma/adapter-better-sqlite3");

const adapter = new PrismaBetterSqlite3({
  url: process.env.DATABASE_URL || "file:./dev.db",
});

const prisma = new PrismaClient({ adapter });
const app = express();
const jwtSecret = process.env.JWT_SECRET;

if (!jwtSecret) {
  throw new Error("JWT_SECRET es obligatorio");
}

app.enable("trust proxy");

app.use(cors());
app.use(express.json());
app.use((req, res, next) => {
  const host = (req.hostname || "").toLowerCase();
  const isLocalHost = host === "localhost" || host === "127.0.0.1";
  const isHttps = req.secure || req.headers["x-forwarded-proto"] === "https";

  if (isHttps || isLocalHost) {
    return next();
  }

  return res.status(400).json({ success: false, message: "HTTPS requerido" });
});

function requireAuth(req, res, next) {
  const authHeader = req.headers.authorization || "";
  const [scheme, token] = authHeader.split(" ");

  if (scheme !== "Bearer" || !token) {
    return res.status(401).json({ success: false, message: "Token no enviado" });
  }

  try {
    req.auth = jwt.verify(token, jwtSecret);
    return next();
  } catch (_error) {
    return res.status(401).json({ success: false, message: "Token invalido o expirado" });
  }
}

app.post("/auth/register", async (req, res) => {
  try {
    const { username, password } = req.body;
    if (!username || !password) {
      return res.status(400).json({ success: false, message: "username y password son obligatorios" });
    }

    const existing = await prisma.user.findUnique({ where: { username } });
    if (existing) {
      return res.status(409).json({ success: false, message: "El usuario ya existe" });
    }

    const passwordHash = await bcrypt.hash(password, 12);
    const user = await prisma.user.create({
      data: { username, passwordHash },
      select: { id: true, username: true },
    });

    return res.status(201).json({
      success: true,
      message: "Usuario creado correctamente",
      user,
    });
  } catch (e) {
    return res.status(500).json({ success: false, message: "Error interno del servidor" });
  }
});

app.post("/auth/login", async (req, res) => {
  try {
    const { username, password } = req.body;
    if (!username || !password) {
      return res.status(400).json({ success: false, message: "username y password son obligatorios" });
    }

    const user = await prisma.user.findUnique({ where: { username } });
    if (!user) {
      return res.status(401).json({ success: false, message: "Usuario o contraseña inválidos" });
    }

    const ok = await bcrypt.compare(password, user.passwordHash);
    if (!ok) {
      return res.status(401).json({ success: false, message: "Usuario o contraseña inválidos" });
    }

    const accessToken = jwt.sign(
      { sub: user.id, username: user.username },
      jwtSecret,
      { expiresIn: "1h" }
    );

    return res.status(200).json({
      success: true,
      message: "Login exitoso",
      user: { id: user.id, username: user.username },
      accessToken,
      refreshToken: null
    });
  } catch (e) {
    return res.status(500).json({ success: false, message: "Error interno del servidor" });
  }
});

app.get("/auth/me", requireAuth, async (req, res) => {
  try {
    const userId = Number(req.auth.sub);
    if (!Number.isInteger(userId)) {
      return res.status(401).json({ success: false, message: "Token invalido" });
    }

    const user = await prisma.user.findUnique({
      where: { id: userId },
      select: { id: true, username: true },
    });

    if (!user) {
      return res.status(401).json({ success: false, message: "Usuario no encontrado" });
    }

    return res.status(200).json({
      success: true,
      message: "Token valido",
      user,
    });
  } catch (_error) {
    return res.status(500).json({ success: false, message: "Error interno del servidor" });
  }
});

app.listen(process.env.PORT, () => {
  console.log(`API running on http://localhost:${process.env.PORT}`);
});
