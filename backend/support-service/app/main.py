from fastapi import FastAPI, Request
from fastapi.staticfiles import StaticFiles
from fastapi.templating import Jinja2Templates

from app.routers.auth import router as auth_router
from app.routers.match import router as legacy_match_router
from app.routers.session_summary import router as session_summary_router
from shared.connections.postgresql_connection import Base, engine
from shared.cors import configure_cors
from shared.models.user import User

app = FastAPI(title="RTS Support API")
templates = Jinja2Templates(directory="templates")

configure_cors(app)
app.include_router(auth_router)
app.include_router(session_summary_router)
app.include_router(legacy_match_router)
app.mount("/static", StaticFiles(directory="static"), name="static")


@app.on_event("startup")
def on_startup():
    Base.metadata.create_all(bind=engine)


@app.get("/health")
def health():
    return {"status": "ok", "component": "support"}


@app.get("/home")
def home(request: Request):
    return templates.TemplateResponse(request, "home.html")
