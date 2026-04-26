# RTS ArquiSoft

Proyecto RTS en Unity con backend FastAPI separado por servicios.

## Estructura actual

```text
backend/
  support-service/       # Autenticacion, usuarios, Firebase y resumenes
  matchmaking-service/   # Cola, ready state y metadata Relay/Lobby
  shared/                # CORS, seguridad, conexiones y modelos comunes
UnityProject/
  Assets/Scripts/
    Support/             # Cliente de support y estado de sesion autenticada
    Multiplayer/         # Cliente y runtime multiplayer en Unity
```

Servicios backend:

- `support`: `http://localhost:8000`
- `matchmaking`: `http://localhost:8001`

Para ejecutar:

```bash
cd backend
docker compose up --build
```

Unity usa `ApiClient` para login/resumenes, `MatchmakingClient` para cola y ready state, y `RelayLobbyClient` para crear/unirse a sesiones de Unity Multiplayer Services con Relay.

## Multiplayer fase 2

La segunda fase agrega una primera capa host-authoritative para gameplay:

- `RtsNetworkCommandBus`: bus de comandos sobre Netcode custom messages.
- `RtsNetworkEntity`: identidad local estable para unidades, edificios y recursos.
- `RtsMultiplayerWorldInitializer`: crea bases iniciales por jugador en zonas libres.
- Movimiento, recoleccion y produccion pasan por el host cuando hay sesion multiplayer activa.

Fuera de una sesion multiplayer, los scripts mantienen el flujo singleplayer anterior.

Limitaciones actuales:

- Combate y fog of war siguen fuera de multiplayer.
- Las entidades no son todavia `NetworkObject` individuales; se sincronizan por comandos e IDs deterministas.
- El host es autoridad total y si se cae la partida no migra de host.
