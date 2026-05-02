# Web Frontend (Next.js)

Guía rápida para que cualquier persona del equipo pueda levantar el frontend en su máquina.

## Requisitos

- Node.js 20+ (recomendado)
- npm (viene con Node)

Opcional:

- Docker Desktop (si quieres levantarlo en contenedor)

## 1) Variables de entorno

Este proyecto usa NextAuth + Google y llama al backend.

1. Copia el archivo de ejemplo:

	 - macOS/Linux:
		 - `cp .env.example .env`
	 - Windows PowerShell:
		 - `Copy-Item .env.example .env`

2. Edita `.env` y completa (mínimo) estas variables:

- `AUTH_SECRET`: un secreto largo aleatorio
- `AUTH_GOOGLE_ID`: Client ID de Google OAuth
- `AUTH_GOOGLE_SECRET`: Client Secret de Google OAuth
- `PY_BACKEND_URL`: URL del backend (por defecto `http://localhost:8000`)
- `NEXTAUTH_URL`: URL del frontend (por defecto `http://localhost:3000`)

Notas:

- `.env` NO se sube a git (solo es local). No pongas secretos en `.env.example`.
- Si no tienes `AUTH_SECRET`, para dev puedes generar uno así:
	- macOS/Linux: `openssl rand -base64 32`
	- Windows PowerShell: `[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))`

## 2) Levantar en modo desarrollo (recomendado)

Desde esta carpeta (`web-frontend/`):

1. Instala dependencias:

	 - `npm ci`

2. Levanta el servidor:

	 - `npm run dev`

3. Abre:

- http://localhost:3000

## 3) Levantar con Docker (imagen de producción)

Desde esta carpeta (`web-frontend/`):

- `docker compose up --build`

Luego abre:

- http://localhost:3000

## Troubleshooting rápido

- Si el login con Google falla, revisa que `AUTH_GOOGLE_ID`/`AUTH_GOOGLE_SECRET` sean correctos y que el backend en `PY_BACKEND_URL` esté levantado.
- Si el frontend no puede conectar al backend, confirma `PY_BACKEND_URL` (por ejemplo `http://localhost:8000`).
