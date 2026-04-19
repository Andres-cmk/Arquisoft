# RTS ArquiSoft — Monolith

Backend del juego construido con FastAPI y PostgreSQL, contenerizado con Docker.

---

## Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

---

## Configuración inicial


### 1. Crear el archivo `.env`

Copia el archivo de ejemplo y rellena los valores:

```bash
cp .env.example .env
```

El `.env` debe quedar así:

```
POSTGRES_USER=postgres
POSTGRES_PASSWORD=admin
POSTGRES_DB=arqui
DATABASE_URL=postgresql://postgres:admin@db:5432/arqui
```

> **Nota:** No cambies `@db` por `@localhost`. Dentro de Docker, `db` es el nombre del contenedor de la base de datos.

---

## Levantar el proyecto

```bash
docker-compose up --build
```

Esto levanta dos contenedores:
- `monolith-api-1` — la API de FastAPI en el puerto `8000`
- `monolith-db-1` — PostgreSQL en el puerto `5432`

Para correrlo en segundo plano:

```bash
docker compose up --build -d
```

Para detenerlo:

```bash
docker compose down
```

---

## Verificar que funciona

### API

Abre en el navegador:

```
http://localhost:8000/docs
```

Deberías ver la documentación interactiva de FastAPI con los endpoints disponibles.

### Endpoints disponibles

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/auth/users` | Crear un usuario |
| `POST` | `/auth/login` | Iniciar sesión |
| `GET` | `/home` | Página de registro |

---

## Probar con Postman

### Crear usuario

- **Método:** `POST`
- **URL:** `http://localhost:8000/auth/users`
- **Body (JSON):**

```json
{
    "username": "testuser",
    "password": "123456"
}
```

### Iniciar sesión

- **Método:** `POST`
- **URL:** `http://localhost:8000/auth/login`
- **Body (JSON):**

```json
{
    "username": "testuser",
    "password": "123456"
}
```

---

## Verificar la base de datos

Para conectarte a la base de datos del contenedor y revisar los datos:

```bash
docker exec -it monolith-db-1 psql -U postgres -d arqui
```

Comandos útiles dentro de psql:

```sql
-- Ver las tablas creadas
\dt

-- Ver los usuarios registrados
SELECT * FROM users;

-- Salir
\q
```

---

## Conexión desde Unity

La URL base para conectarse desde Unity es:

```
http://localhost:8000
```

No es necesario cambiar nada mientras se desarrolle en la misma máquina. Al desplegar en un servidor, reemplazar `localhost` por la IP o dominio del servidor.

---

## Estructura del proyecto

```
Monolith/
├── app/
│   ├── connections/
│   │   └── postgresql_connection.py
│   ├── models/
│   │   └── user.py
│   ├── routers/
│   │   └── auth.py
│   ├── schemas/
│   │   └── user_schemas.py
│   └── main.py
├── static/
│   └── styles.css
├── templates/
│   └── home.html
├── .env               # No subir al repositorio
├── .env.example
├── .gitignore
├── docker-compose.yml
├── Dockerfile
└── requirements.txt
```