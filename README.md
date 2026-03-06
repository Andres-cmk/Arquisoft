# Prototype1 - Login con API (Windows)

Repositorio con cliente Unity y backend Node.js/Express + Prisma (SQLite).

## Estructura

- `My project/`: cliente Unity.
- `backend/`: API de autenticacion.

## Requisitos

- Windows 10/11
- PowerShell
- Node.js 18+ (incluye `npm`)
- Unity Hub + version de Unity del proyecto

## Backend desde cero (solo clonando el repo)

1. Clona el repositorio y entra a la carpeta `backend`:

```powershell
cd "<ruta-del-repo>\backend"
```

2. Instala dependencias:

```powershell
cmd /c npm.cmd install
```

3. Crea el archivo `.env` (si no existe):

```powershell
Copy-Item .env.example .env
```

Si `Copy-Item` falla por no existir `.env.example`, crea `.env` manual con:

```env
DATABASE_URL="file:./dev.db"
JWT_SECRET="cambia_esto_por_un_secreto_largo"
PORT=3000
```

4. Sincroniza base de datos con Prisma:

```powershell
cmd /c npx prisma db push
```

5. Inicia el backend:

```powershell
cmd /c npm.cmd run start
```

Debe quedar corriendo en `http://localhost:3000`.

## Probar API rapido desde terminal

Abre otra ventana de PowerShell (deja backend corriendo en la primera).

### Registro

```powershell
Invoke-RestMethod -Method Post -Uri 'http://localhost:3000/auth/register' -ContentType 'application/json' -Body '{"username":"user_test_1","password":"123456"}'
```

Si ese usuario ya existe, cambia `user_test_1` por otro nombre.

### Login

```powershell
$login = Invoke-RestMethod -Method Post -Uri 'http://localhost:3000/auth/login' -ContentType 'application/json' -Body '{"username":"user_test_1","password":"123456"}'
$login.accessToken
```

Debe imprimir un JWT.

### Validar token (`/auth/me`)

```powershell
Invoke-RestMethod -Method Get -Uri 'http://localhost:3000/auth/me' -Headers @{ Authorization = "Bearer $($login.accessToken)" }
```

Esperado: `success: true` y `user` con `id` y `username`.

## Probar flujo en Unity

1. Abre el proyecto `My project` en Unity.
2. Abre `LoginScene`.
3. Selecciona el GameObject con `LoginController`.
4. En Inspector, confirma que `loginButton` esta asignado.
5. Presiona Play.
6. Haz login con un usuario valido.
7. Verifica que entra a `MainMenuScene`.
8. Entra a `GameScene` y valida:
   - Si hay token valido y `/auth/me` responde 200, permanece en `GameScene`.
   - Si no hay token o es invalido, `GameSessionGuard` redirige a `LoginScene`.

## Validar seguridad minima

- Backend no devuelve `passwordHash` ni `salt` al cliente en respuestas de auth.
- Cliente no persiste contrasena en sesion; solo usa `CurrentUser` y `AccessToken`.
- Para produccion, usa `https://` en la API.
- En desarrollo local se permite `http://localhost` por conveniencia.

## Resultado esperado del smoke test

Flujo completo de backend:

- `register` -> `success: true`
- `login` -> `success: true` + `accessToken`
- `/auth/me` con Bearer token -> `success: true`

Ejemplo:

```json
{"RegisteredSuccess":true,"LoginSuccess":true,"HasAccessToken":true,"MeSuccess":true}
```

## Troubleshooting

- Error `npm.ps1 cannot be loaded because running scripts is disabled`:
  usa `cmd /c npm.cmd ...` como en este README.

- Error `The datasource.url property is required in your Prisma config file when using prisma db push`:
  - confirma que existe `backend/.env`.
  - confirma que `DATABASE_URL` esta definida.
  - ejecuta `cmd /c npx prisma db push` desde `backend`.

- Error `Cannot find module 'dotenv/config'`:
  - ejecuta `cmd /c npm.cmd install` en `backend`.
  - si persiste: `cmd /c npm.cmd install dotenv --save`.

- `Error interno del servidor` al registrar:
  ejecuta de nuevo `cmd /c npx prisma db push`.

- `Token invalido o expirado` en `/auth/me`:
  vuelve a hacer login y usa el token nuevo.
