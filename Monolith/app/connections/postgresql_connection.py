from sqlalchemy import create_engine
from sqlalchemy.orm import declarative_base, sessionmaker

# Me dio pereza poner variables de entorno xd
# Descomenten la linea de aca abjo y rellen la informacion jaja perdon
#DATABASE_URL = "postgresql://rellenar1@rellnar2"

# 1.                     postgres:admin
# 2.                                    localhost:5432/arqui
engine = create_engine(DATABASE_URL)
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
Base = declarative_base()

def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()