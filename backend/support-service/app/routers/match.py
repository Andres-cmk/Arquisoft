from fastapi import APIRouter, status

from app.connections.firebase_connection import db
from app.schemas.match_schemas import ReceivedPayload

router = APIRouter(tags=["match"])

@router.post("/support/session-summary", status_code=status.HTTP_201_CREATED)
@router.post("/match/session-summary", status_code=status.HTTP_201_CREATED)
def receive_match_summary(payload: ReceivedPayload):
    payload_dict = payload.dict()

    doc_ref = db.collection("match_summaries").add(payload_dict)
    doc_id = doc_ref[1].id

    return {"message": "Match summary received and stored successfully", "id": doc_id}
