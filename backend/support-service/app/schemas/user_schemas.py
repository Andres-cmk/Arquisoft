from datetime import datetime
from pydantic import BaseModel, Field


class UserCreate(BaseModel):
    username: str = Field(..., min_length=1, max_length=255)
    password: str = Field(..., min_length=1, max_length=255)

class UserLogin(BaseModel):
    username: str = Field(..., min_length=1, max_length=255)
    password: str = Field(..., min_length=1, max_length=255)


class GoogleTokenIn(BaseModel):
    id_token: str = Field(..., min_length=1)

class UserResponse(BaseModel):
    user_id: int
    username: str
    created_at: datetime

    class Config:
        from_attributes = True