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

## Development

```sh
docker compose up -d --build db api   # rerun after API changes
cd client && npm install && npm run dev   # http://localhost:5173
```

Regenerate the API client: `npm run gen:api` (API must be running).

Database changes: add a new SQL file to `api/Data/Migrations/`, e.g. `0001_employees.sql`. It runs on startup.

## Security policies

| Endpoint | Access |
|---|---|
| `GET /api/health` | Anyone |

## Current state

Scaffold only.

## Testing strategy

TDD.

- Unit: `api/UnitTests`, `client` (`npm test`)
- Integration: `api/IntegrationTests` (needs Docker)
- E2E: `e2e` (against `docker compose`)
