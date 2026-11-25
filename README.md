# Absence Planner

Holiday planning for Business Esbjerg.

- `api/` – .NET Web API in three layers: `Api` → `Business` → `Data` (linq2db, PostgreSQL)
- `client/` – React client
- `e2e/` – Playwright tests

## Running

```sh
cp .env.example .env
docker compose up --build
```

Client: http://localhost:8080 · Swagger: http://localhost:5000/swagger

Log in with `ADMIN_EMAIL` / `ADMIN_PASSWORD` from `.env`.

## Development

```sh
docker compose up -d --build db api   # rerun after API changes
cd client && npm install && npm run dev   # http://localhost:5173
```

Regenerate the API client: `npm run gen:api` (API must be running).

Database changes: add a new SQL file to `api/Data/Migrations/` with the next number, e.g. `0002_employees.sql`. It runs on startup.

## Security policies

Login sets a cookie. Not logged in gives `401`, wrong role gives `403`.

| Endpoint | Access |
|---|---|
| `GET /api/health` | Anyone |
| `/swagger` (API docs) | Anyone |
| `POST /api/auth/login` | Anyone |
| `POST /api/auth/logout` | Logged in |
| `GET /api/auth/me` | Logged in |
| `GET /api/users` | Admin |
| Everything else | Logged in |

Roles: `Employee`, `Accounting`, `Admin`.

## Current state

Users, roles and login (API). Next: login page.

## Testing strategy

TDD: write a failing test first, then the code to make it pass.

- Unit: `api/UnitTests`, `client` (`npm test`)
- Integration: `api/IntegrationTests` (needs Docker)
- E2E: `e2e` (against `docker compose`)
