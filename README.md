# Enterprise AI Document Processing - Scaffold

Initial production-oriented scaffolding for an enterprise AI document processing platform using **.NET 10 + ASP.NET Core Web API + PostgreSQL + Angular** following **Clean Architecture**.

## Folder structure

```text
.
├── Directory.Build.props
├── EnterpriseAiDocumentProcessing.sln
├── docker-compose.yml
├── src
│   ├── Api
│   │   ├── Api.csproj
│   │   ├── Configuration
│   │   │   ├── LoggingOptions.cs
│   │   │   └── ServiceCollectionExtensions.cs
│   │   ├── Controllers
│   │   │   └── HealthController.cs
│   │   ├── Dockerfile
│   │   ├── Program.cs
│   │   ├── appsettings.Development.json
│   │   └── appsettings.json
│   ├── Application
│   │   ├── Application.csproj
│   │   ├── Common
│   │   │   ├── Interfaces
│   │   │   │   ├── IRepository.cs
│   │   │   │   └── IUnitOfWork.cs
│   │   │   └── Models
│   │   │       ├── Error.cs
│   │   │       └── Result.cs
│   │   └── DependencyInjection.cs
│   ├── Domain
│   │   ├── Common
│   │   │   ├── AuditableEntity.cs
│   │   │   └── BaseEntity.cs
│   │   ├── Domain.csproj
│   │   ├── Entities
│   │   │   └── Document.cs
│   │   └── Enums
│   │       └── DocumentStatus.cs
│   └── Infrastructure
│       ├── DependencyInjection.cs
│       ├── Infrastructure.csproj
│       ├── Persistence
│       │   ├── ApplicationDbContext.cs
│       │   └── Configurations
│       │       └── DocumentConfiguration.cs
│       └── Repositories
│           ├── BaseRepository.cs
│           └── UnitOfWork.cs
└── frontend
    └── enterprise-ai-docs
        ├── Dockerfile
        ├── angular.json
        ├── package.json
        ├── tsconfig.app.json
        ├── tsconfig.json
        └── src
            ├── app
            │   ├── app.component.ts
            │   ├── app.routes.ts
            │   ├── core
            │   │   ├── layout
            │   │   │   ├── main-layout.component.css
            │   │   │   ├── main-layout.component.html
            │   │   │   └── main-layout.component.ts
            │   │   └── sidebar
            │   │       ├── sidebar.component.css
            │   │       ├── sidebar.component.html
            │   │       └── sidebar.component.ts
            │   └── pages
            │       ├── document-details
            │       │   ├── document-details-page.component.css
            │       │   ├── document-details-page.component.html
            │       │   └── document-details-page.component.ts
            │       ├── processed-documents
            │       │   ├── processed-documents-page.component.css
            │       │   ├── processed-documents-page.component.html
            │       │   └── processed-documents-page.component.ts
            │       ├── processing-queue
            │       │   ├── processing-queue-page.component.css
            │       │   ├── processing-queue-page.component.html
            │       │   └── processing-queue-page.component.ts
            │       └── upload
            │           ├── upload-page.component.css
            │           ├── upload-page.component.html
            │           └── upload-page.component.ts
            ├── environments
            │   ├── environment.prod.ts
            │   └── environment.ts
            ├── favicon.ico
            ├── index.html
            ├── main.ts
            └── styles.css
```

## What is included

### Backend (Clean Architecture)
- **Domain**: base entities and domain primitives.
- **Application**: result pattern (`Result`, `Error`), repository/unit-of-work contracts, DI extension.
- **Infrastructure**: EF Core `ApplicationDbContext`, PostgreSQL provider wiring, base repository implementation, migration-ready setup.
- **API**: ASP.NET Core Web API composition root, Serilog logging, appsettings configuration, health endpoint, API versioning.

### Frontend (Angular)
- Angular standalone app with routing.
- Dark theme base styling.
- Main layout + sidebar navigation.
- Scaffold pages:
  - Upload
  - Processing Queue
  - Processed Documents
  - Document Details

### Containers
- `docker-compose.yml` orchestrates:
  - `api` (.NET API)
  - `postgres` (PostgreSQL)
  - `web` (Angular served by Nginx)

## Run locally

### Docker (recommended)
```bash
docker compose up --build
```

- API: `http://localhost:8080/api/v1/health`
- Web: `http://localhost:4200`
- PostgreSQL: `localhost:5432`

### Backend manually
```bash
# Requires .NET 10 SDK preview
cd src/Api
dotnet restore
dotnet run
```

### EF Core migrations
```bash
# From repository root (requires dotnet-ef)
dotnet ef migrations add InitialCreate --project src/Infrastructure --startup-project src/Api --output-dir Persistence/Migrations
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

### Frontend manually
```bash
cd frontend/enterprise-ai-docs
npm install
npx ng serve --host 0.0.0.0 --port 4200
```

## Notes
- This is **scaffolding only**: no business processing logic is implemented yet.
- Update package versions as .NET 10/Angular releases advance.
