from fastapi import APIRouter, Depends, HTTPException, status

from app.schemas.match_schemas import ReceivedPayload
from shared.connections.firebase_connection import get_firestore_client
from shared.security import AuthPrincipal, get_current_user

router = APIRouter(prefix="/support", tags=["support"])


@router.post("/session-summary", status_code=status.HTTP_201_CREATED)
def receive_session_summary(
    payload: ReceivedPayload,
    current_user: AuthPrincipal = Depends(get_current_user),
):
    payload_dict = payload.dict()
    payload_dict["userId"] = current_user.user_id
    payload_dict["username"] = current_user.username

    try:
        db = get_firestore_client()
        doc_ref = db.collection("match_summaries").add(payload_dict)
    except RuntimeError as exc:
        raise HTTPException(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            detail=str(exc),
        ) from exc

    doc_id = doc_ref[1].id
    return {"message": "Session summary received and stored successfully", "id": doc_id}
