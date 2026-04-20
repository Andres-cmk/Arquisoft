from fastapi import FastAPI, Request
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates
from app.connections.postgresql_connection import Base, engine
from app.models.user import User
from app.routers.auth import router as auth_router
from app.routers.match import router as match_router

app = FastAPI()
templates = Jinja2Templates(directory="templates")

app.include_router(auth_router)
app.include_router(match_router)
app.mount("/static", StaticFiles(directory="static"), name="static")

@app.on_event("startup")
def on_startup():
    Base.metadata.create_all(bind=engine)

@app.get("/home")
def home(request: Request):
    return templates.TemplateResponse(request, "home.html")