import os
import secrets

from fastapi import APIRouter, Depends, HTTPException, status
from google.auth.transport import requests as google_requests
from google.oauth2 import id_token as google_id_token
from sqlalchemy.orm import Session

from app.connections.postgresql_connection import get_db
from app.models.user import User
from app.schemas.user_schemas import GoogleTokenIn, UserCreate, UserLogin, UserResponse

router = APIRouter(prefix="/auth", tags=["auth"])

@router.post("/google", response_model=UserResponse)
def login_with_google(payload: GoogleTokenIn, db: Session = Depends(get_db)):
    google_client_id = os.getenv("GOOGLE_CLIENT_ID")
    if not google_client_id:
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail="Missing GOOGLE_CLIENT_ID in backend environment",
        )

    try:
        info = google_id_token.verify_oauth2_token(
            payload.id_token,
            google_requests.Request(),
            google_client_id,
        )
    except Exception:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid Google id_token",
        )

    email = info.get("email")
    name = info.get("name", "")    

    if not email:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail="Google token has no email",
        )

    user = db.query(User).filter(User.email == email).first()

    if not user:
        
        user = User(
            username=name,
            email=email,
            password=secrets.token_urlsafe(32)
        )
        db.add(user)
        db.commit()
        db.refresh(user)

    return user