from fastapi import APIRouter, Depends, HTTPException, status

from app.schemas.match_schemas import ReceivedPayload, PayloadinDB
from shared.connections.firebase_connection import get_firestore_client

router = APIRouter(prefix="/match", tags=["match"])

@router.post("/session-summary", status_code=status.HTTP_201_CREATED)
def receive_match_summary(payload: ReceivedPayload):
    payload_dict = payload.dict()

    try:
        db = get_firestore_client()
        doc_ref = db.collection("match_summaries").add(payload_dict)
    except RuntimeError as exc:
        raise HTTPException(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            detail=str(exc),
        ) from exc

    doc_id = doc_ref[1].id

    return {"message": "Match summary received and stored successfully", "id": doc_id}
