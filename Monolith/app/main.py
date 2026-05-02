import os
from dotenv import load_dotenv
from fastapi import FastAPI, Request
from fastapi.middleware.cors import CORSMiddleware
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates
from app.connections.postgresql_connection import Base, engine
from app.models.user import User
from app.routers.auth import router as auth_router
from app.routers.match import router as match_router

app = FastAPI()


load_dotenv()

frontend_origin = os.getenv("FRONTEND_ORIGIN")
if frontend_origin:
    app.add_middleware(
        CORSMiddleware,
        allow_origins=[frontend_origin],
        allow_credentials=True,
        allow_methods=["*"] ,
        allow_headers=["*"],
    )

app.include_router(auth_router)
app.include_router(match_router)

@app.on_event("startup")
def on_startup():
    Base.metadata.create_all(bind=engine)