from fastapi import APIRouter, Depends, status

from app.schemas import JoinQueueRequest, MatchResponse, QueueJoinResponse, ReadyRequest, RelaySessionData
from app.store import get_match, join_queue, leave_match, set_ready, set_relay_data
from shared.security import AuthPrincipal, get_current_user

router = APIRouter(prefix="/matchmaking", tags=["matchmaking"])


@router.post("/queue/join", response_model=QueueJoinResponse, status_code=status.HTTP_200_OK)
def join_matchmaking_queue(
    request: JoinQueueRequest,
    current_user: AuthPrincipal = Depends(get_current_user),
):
    return join_queue(request, current_user)


@router.get("/matches/{match_id}", response_model=MatchResponse)
def read_match(match_id: str, current_user: AuthPrincipal = Depends(get_current_user)):
    return get_match(match_id, current_user)


@router.post("/matches/{match_id}/relay", response_model=MatchResponse)
def publish_relay_data(
    match_id: str,
    relay_data: RelaySessionData,
    current_user: AuthPrincipal = Depends(get_current_user),
):
    return set_relay_data(match_id, relay_data, current_user)


@router.post("/matches/{match_id}/ready", response_model=MatchResponse)
def mark_ready(
    match_id: str,
    request: ReadyRequest,
    current_user: AuthPrincipal = Depends(get_current_user),
):
    return set_ready(match_id, request.ready, current_user)


@router.post("/matches/{match_id}/leave", response_model=MatchResponse)
def leave(match_id: str, current_user: AuthPrincipal = Depends(get_current_user)):
    return leave_match(match_id, current_user)
