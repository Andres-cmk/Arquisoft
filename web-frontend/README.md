# Web Frontend (Next.js)

Guia rapida para levantar el frontend localmente.

## Requisitos

- Node.js 20+
- npm
- Docker Desktop, opcional si se usa contenedor

## Variables de entorno

Este proyecto usa NextAuth + Google y llama al backend.

Crea o edita `.env` con estas variables:

```env
AUTH_SECRET=replace-with-a-long-random-secret
AUTH_GOOGLE_ID=google-oauth-client-id
AUTH_GOOGLE_SECRET=google-oauth-client-secret
PY_BACKEND_URL=http://localhost:8000
NEXTAUTH_URL=http://localhost:3000
```

`.env` no se sube a git. Para generar `AUTH_SECRET` en desarrollo:

- macOS/Linux: `openssl rand -base64 32`
- Windows PowerShell: `[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))`

## Desarrollo local

Desde `web-frontend/`:

```bash
npm ci
npm run dev
```

Luego abre `http://localhost:3000`.

## Docker

Desde `web-frontend/`:

```bash
docker compose up --build
```

Luego abre `http://localhost:3000`.

## Login desde Unity

Unity abre `/unity-login` con un callback local. Tras Google, esta ruta devuelve a Unity el `access_token` emitido por el backend.

Para pruebas multiplayer en la misma maquina, Unity envia `select_account=1`; la web fuerza el selector de cuenta de Google y cada instancia recibe el token de la cuenta elegida.

Para desarrollo local deben estar levantados:

- Frontend: `http://localhost:3000`
- Backend support: `http://localhost:8000`

El backend debe tener `AUTH_GOOGLE_ID` o `GOOGLE_CLIENT_ID` con el mismo Client ID de Google usado por el frontend.

## Troubleshooting rapido

- Si Google falla, revisa `AUTH_GOOGLE_ID`, `AUTH_GOOGLE_SECRET`, `AUTH_SECRET` y los redirect URIs configurados en Google Cloud.
- Si el frontend no puede conectar al backend, confirma `PY_BACKEND_URL`.
