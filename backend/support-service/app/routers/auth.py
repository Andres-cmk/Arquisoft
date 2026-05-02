from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.orm import Session

from app.schemas.user_schemas import LoginResponse, UserCreate, UserLogin, UserResponse
from shared.connections.postgresql_connection import get_db
from shared.models.user import User
from shared.security import create_access_token

router = APIRouter(prefix="/auth", tags=["auth"])

@router.post("/users", response_model=UserResponse, status_code=status.HTTP_201_CREATED)
def create_user(user: UserCreate, db: Session = Depends(get_db)):
    existing_user = db.query(User).filter(User.username == user.username).first()
    if existing_user:
        raise HTTPException(
            status_code=status.HTTP_400_BAD_REQUEST,
            detail="Username already exists",
        )

    new_user = User(username=user.username, password=user.password)
    db.add(new_user)
    db.commit()
    db.refresh(new_user)

    return new_user

@router.post("/login", response_model=LoginResponse)
def login(user: UserLogin, db: Session = Depends(get_db)):
    existing_user = db.query(User).filter(User.username == user.username).first()
    if not existing_user or existing_user.password != user.password:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid username or password",
        )
    access_token = create_access_token(existing_user.user_id, existing_user.username)
    return {
        "message": "Login successful",
        "user_id": existing_user.user_id,
        "username": existing_user.username,
        "access_token": access_token,
        "token_type": "bearer",
    }
