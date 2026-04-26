from datetime import datetime, timezone
from threading import Lock
from uuid import uuid4

from fastapi import HTTPException, status

from app.schemas import JoinQueueRequest, RelaySessionData
from shared.security import AuthPrincipal

_lock = Lock()
_matches: dict[str, dict] = {}


def _now_iso() -> str:
    return datetime.now(timezone.utc).isoformat()


def _as_response(match: dict) -> dict:
    players = list(match["players"].values())
    players.sort(key=lambda player: 0 if player["role"] == "host" else 1)

    return {
        "matchId": match["matchId"],
        "status": match["status"],
        "minPlayers": match["minPlayers"],
        "maxPlayers": match["maxPlayers"],
        "gameMode": match["gameMode"],
        "region": match["region"],
        "hostUserId": match["hostUserId"],
        "players": players,
        "relay": match["relay"],
        "createdAtUtc": match["createdAtUtc"],
        "updatedAtUtc": match["updatedAtUtc"],
    }


def _find_existing_player_match(user_id: int) -> dict | None:
    for match in _matches.values():
        if match["status"] == "closed":
            continue
        if user_id in match["players"]:
            return match
    return None


def _find_waiting_match(request: JoinQueueRequest) -> dict | None:
    for match in _matches.values():
        if match["status"] not in {"waiting", "matched"}:
            continue
        if match["gameMode"] != request.gameMode or match["region"] != request.region:
            continue
        if match["minPlayers"] != request.minPlayers or match["maxPlayers"] != request.maxPlayers:
            continue
        if len(match["players"]) >= match["maxPlayers"]:
            continue
        return match
    return None


def join_queue(request: JoinQueueRequest, principal: AuthPrincipal) -> dict:
    if request.minPlayers > request.maxPlayers:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail="minPlayers cannot be greater than maxPlayers",
        )

    with _lock:
        existing_match = _find_existing_player_match(principal.user_id)
        if existing_match is not None:
            response = _as_response(existing_match)
            response["role"] = existing_match["players"][principal.user_id]["role"]
            return response

        match = _find_waiting_match(request)
        if match is None:
            now = _now_iso()
            match = {
                "matchId": str(uuid4()),
                "status": "waiting",
                "minPlayers": request.minPlayers,
                "maxPlayers": request.maxPlayers,
                "gameMode": request.gameMode,
                "region": request.region,
                "hostUserId": principal.user_id,
                "players": {
                    principal.user_id: {
                        "userId": principal.user_id,
                        "username": principal.username,
                        "role": "host",
                        "ready": False,
                    }
                },
                "relay": None,
                "createdAtUtc": now,
                "updatedAtUtc": now,
            }
            _matches[match["matchId"]] = match
            response = _as_response(match)
            response["role"] = "host"
            return response

        match["players"][principal.user_id] = {
            "userId": principal.user_id,
            "username": principal.username,
            "role": "client",
            "ready": False,
        }
        if len(match["players"]) >= match["minPlayers"]:
            match["status"] = "matched"
        if len(match["players"]) >= match["maxPlayers"]:
            match["status"] = "full"
        match["updatedAtUtc"] = _now_iso()

        response = _as_response(match)
        response["role"] = "client"
        return response


def get_match(match_id: str, principal: AuthPrincipal | None = None) -> dict:
    with _lock:
        match = _matches.get(match_id)
        if match is None:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Match not found")
        if principal is not None and principal.user_id not in match["players"]:
            raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Player is not in this match")
        return _as_response(match)


def set_relay_data(match_id: str, relay_data: RelaySessionData, principal: AuthPrincipal) -> dict:
    with _lock:
        match = _matches.get(match_id)
        if match is None:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Match not found")
        if match["hostUserId"] != principal.user_id:
            raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Only the host can publish relay data")

        match["relay"] = relay_data.dict()
        if match["status"] in {"matched", "full"}:
            match["status"] = "readying"
        match["updatedAtUtc"] = _now_iso()
        return _as_response(match)


def set_ready(match_id: str, ready: bool, principal: AuthPrincipal) -> dict:
    with _lock:
        match = _matches.get(match_id)
        if match is None:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Match not found")
        player = match["players"].get(principal.user_id)
        if player is None:
            raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Player is not in this match")

        player["ready"] = ready
        ready_players = [player_data for player_data in match["players"].values() if player_data["ready"]]
        if match["relay"] is not None and len(match["players"]) >= match["minPlayers"] and len(ready_players) == len(match["players"]):
            match["status"] = "starting"
        elif len(match["players"]) >= match["minPlayers"]:
            match["status"] = "readying" if match["relay"] is not None else "matched"
        else:
            match["status"] = "waiting"

        match["updatedAtUtc"] = _now_iso()
        return _as_response(match)


def leave_match(match_id: str, principal: AuthPrincipal) -> dict:
    with _lock:
        match = _matches.get(match_id)
        if match is None:
            raise HTTPException(status_code=status.HTTP_404_NOT_FOUND, detail="Match not found")
        if principal.user_id not in match["players"]:
            raise HTTPException(status_code=status.HTTP_403_FORBIDDEN, detail="Player is not in this match")

        del match["players"][principal.user_id]
        if not match["players"]:
            match["status"] = "closed"
            match["updatedAtUtc"] = _now_iso()
            return _as_response(match)

        if match["hostUserId"] == principal.user_id:
            next_host_id = next(iter(match["players"]))
            match["hostUserId"] = next_host_id
            match["players"][next_host_id]["role"] = "host"
            match["relay"] = None

        if len(match["players"]) < match["minPlayers"]:
            match["status"] = "waiting"
        else:
            match["status"] = "matched"

        match["updatedAtUtc"] = _now_iso()
        return _as_response(match)
