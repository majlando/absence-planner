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
