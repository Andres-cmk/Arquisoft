# RTS ArquiSoft Backend

Backend separado en dos servicios FastAPI y un paquete compartido:

- `support-service`: autenticacion, usuarios, pagina `/home`, resumenes de partida y Firebase.
- `matchmaking-service`: cola multiplayer, ciclo de partida, ready state y metadata Relay/Lobby.
- `shared`: CORS, seguridad, conexiones y modelos comunes.

## Estructura

```text
backend/
  support-service/
    app/
    static/
    templates/
    config/
  matchmaking-service/
    app/
  shared/
    connections/
    models/
  docker-compose.yml
  Dockerfile
```

## Requisitos

- Docker Desktop
- Archivo `.env` basado en `.env.example`
- Credenciales Firebase en `support-service/config/serviceAccountKey.json`, o la ruta configurada por `FIREBASE_CREDENTIALS`

## Configuracion

`.env` minimo:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=admin
POSTGRES_DB=arqui
DATABASE_URL=postgresql://postgres:admin@db:5432/arqui
FIREBASE_CREDENTIALS=config/serviceAccountKey.json
AUTH_TOKEN_SECRET=replace-this-dev-secret
ALLOWED_ORIGINS=*
```

## Levantar servicios

```bash
docker compose up --build
```

Servicios:

- `support`: `http://localhost:8000`
- `matchmaking`: `http://localhost:8001`
- `db`: PostgreSQL expuesto en `localhost:5433`

## Endpoints principales

Support:

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| `GET` | `/health` | Health check |
| `POST` | `/auth/users` | Crear usuario |
| `POST` | `/auth/login` | Login. Devuelve `user_id` y `access_token` |
| `POST` | `/support/session-summary` | Guardar resumen en Firebase |
| `POST` | `/match/session-summary` | Ruta legacy para resumen |
| `GET` | `/home` | Pagina HTML existente |

Matchmaking:

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| `GET` | `/health` | Health check |
| `POST` | `/matchmaking/queue/join` | Entrar a cola |
| `GET` | `/matchmaking/matches/{match_id}` | Consultar partida |
| `POST` | `/matchmaking/matches/{match_id}/relay` | Publicar metadata Relay/Lobby. Solo host |
| `POST` | `/matchmaking/matches/{match_id}/ready` | Marcar jugador listo |
| `POST` | `/matchmaking/matches/{match_id}/leave` | Salir de partida |

Los endpoints de matchmaking requieren:

```http
Authorization: Bearer <access_token>
```

## Flujo multiplayer

1. Unity hace login contra `support`.
2. `support` responde `user_id`, `username` y `access_token`.
3. Unity llama `matchmaking/queue/join`.
4. El primer jugador queda como `host`.
5. Cuando entra otro jugador, la partida pasa a `full` para `maxPlayers=2`.
6. El host publica metadata de Relay/Lobby en `/relay`.
7. Cada jugador marca ready en `/ready`.

La cola esta en memoria para esta fase. Si el contenedor de `matchmaking` se reinicia, las partidas activas se pierden.
