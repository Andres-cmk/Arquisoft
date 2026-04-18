from fastapi import APIRouter, Depends, HTTPException, status
from sqlalchemy.orm import Session

from app.connections.postgresql_connection import get_db
from app.models.user import User
from app.schemas.user_schemas import UserCreate, UserLogin, UserResponse

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

@router.post("/login")
def login(user: UserLogin, db: Session = Depends(get_db)):
    existing_user = db.query(User).filter(User.username == user.username).first()
    if not existing_user or existing_user.password != user.password:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Invalid username or password",
        )
    return {"message": "Login successful"}