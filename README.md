# Enterprise AI Document Processing - Scaffold

Production-oriented scaffolding for an enterprise AI document processing platform using:

- **.NET 10 (ASP.NET Core Web API)**
- **PostgreSQL 16**
- **Angular 19**
- **Clean Architecture**

> This repository currently provides the project skeleton and foundational plumbing (API setup, data access setup, UI shell, Docker setup), not the full business workflow implementation yet.

---

## 1) Repository structure

```text
.
├── Directory.Build.props
├── EnterpriseAiDocumentProcessing.sln
├── docker-compose.yml
├── database
│   └── scripts
│       └── 001_initial_document_processing.sql
├── src
│   ├── Api
│   ├── Application
│   ├── Domain
│   └── Infrastructure
└── frontend
    └── enterprise-ai-docs
```

- `src/Api`: ASP.NET Core Web API entry point and configuration.
- `src/Application`: application layer contracts + result pattern.
- `src/Domain`: domain entities and enums.
- `src/Infrastructure`: EF Core persistence and repository implementations.
- `frontend/enterprise-ai-docs`: Angular application.
- `docker-compose.yml`: orchestrates API + PostgreSQL + frontend containers.

---

## 2) Prerequisites

Choose **one** way to run the solution:

### Option A (recommended): Docker-based run
Install:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (or Docker Engine + Docker Compose plugin)

Verify:

```bash
docker --version
docker compose version
```

### Option B: Run services manually
Install:

- **Git**
- **.NET SDK 10.0 preview** (the API targets `net10.0`)
- **Node.js 20+** and **npm**
- **PostgreSQL 16**

Verify:

```bash
git --version
dotnet --version
node --version
npm --version
psql --version
```

---

## 3) Download (clone) the source code

From your preferred parent directory:

```bash
git clone <YOUR_REPOSITORY_URL> CCaptureDemo
cd CCaptureDemo
```

If this repository has multiple branches, switch to your target branch:

```bash
git branch -a
git checkout <branch-name>
```

---

## 4) Build and run with Docker (fastest full-stack setup)

This starts **PostgreSQL**, **API**, and **Web UI** together.

### 4.1 Build and start containers

From repository root:

```bash
docker compose up --build
```

### 4.2 Access running services

- Web UI: `http://localhost:4200`
- API health endpoint: `http://localhost:8080/api/v1/health`
- PostgreSQL: `localhost:5432`
  - user: `postgres`
  - password: `postgres`
  - database: `enterprise_ai_docs`

### 4.3 Stop services

In the terminal where compose is running, press `Ctrl + C`.

To stop/remove containers later:

```bash
docker compose down
```

To stop and also remove persisted PostgreSQL volume data:

```bash
docker compose down -v
```

---

## 5) Build and run manually (without Docker)

Use this if you prefer local SDK/runtime execution.

## 5.1 Start PostgreSQL

Ensure a PostgreSQL instance is running and create/use database:

- Database: `enterprise_ai_docs`
- Username: `postgres`
- Password: `postgres`
- Port: `5432`

You can initialize tables using:

```bash
psql -h localhost -U postgres -d enterprise_ai_docs -f database/scripts/001_initial_document_processing.sql
```

## 5.2 Build and run backend API

From repository root:

```bash
dotnet restore EnterpriseAiDocumentProcessing.sln
dotnet build EnterpriseAiDocumentProcessing.sln
```

Run API:

```bash
dotnet run --project src/Api/Api.csproj
```

By default, development settings use local PostgreSQL connection (`localhost:5432`) via `src/Api/appsettings.Development.json`.

Health check:

```bash
curl http://localhost:8080/api/v1/health
```

## 5.3 Build and run frontend

Open a second terminal:

```bash
cd frontend/enterprise-ai-docs
npm install
npm run build
npm start
```

Then open:

- `http://localhost:4200`

The frontend development environment points API calls to:

- `http://localhost:8080/api/v1`

---

## 6) Entity Framework Core migrations

If you need schema migration workflow instead of SQL scripts:

```bash
# from repository root
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate \
  --project src/Infrastructure \
  --startup-project src/Api \
  --output-dir Persistence/Migrations

dotnet ef database update \
  --project src/Infrastructure \
  --startup-project src/Api
```

---

## 7) Common troubleshooting

- **Port already in use (4200/5432/8080)**
  - Stop conflicting processes or change local port mappings.

- **`dotnet` build fails due to SDK version**
  - Confirm .NET 10 SDK preview is installed (`dotnet --list-sdks`).

- **Frontend cannot reach API**
  - Ensure API is running on `http://localhost:8080`.
  - Verify `frontend/enterprise-ai-docs/src/environments/environment.ts` has correct `apiBaseUrl`.

- **Database connection errors**
  - Confirm PostgreSQL is running and credentials/db name match API config.

---

## 8) Quick command reference

### Full stack with Docker

```bash
docker compose up --build
```

### Manual local run (3 terminals)

Terminal 1 (database): start PostgreSQL service.

Terminal 2 (API):

```bash
dotnet run --project src/Api/Api.csproj
```

Terminal 3 (frontend):

```bash
cd frontend/enterprise-ai-docs
npm install
npm start
```

---

## 9) Current project status

This repository is intentionally a **scaffold/base architecture**. It includes foundational setup for API, infrastructure, UI shell, and deployment orchestration so domain-specific processing features can be implemented incrementally.
