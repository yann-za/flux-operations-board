# Flux Operations Board

> **Professional showcase project** — Real-time supervision and management of operational data flows via an interactive dashboard and widget system.

[![CI](https://github.com/your-org/flux-operations-board/actions/workflows/ci.yml/badge.svg)](https://github.com/your-org/flux-operations-board/actions)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![Angular](https://img.shields.io/badge/Angular-17-DD0031?logo=angular)](https://angular.dev)
[![Azure](https://img.shields.io/badge/Azure-Bicep-0078D4?logo=microsoftazure)](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)

---

## Overview

**Flux Operations Board** is a full-stack operations management platform for supervising ETL pipelines, API integrations, data streams, and batch jobs. It provides:

- **Real-time dashboard** with KPI widgets (throughput, error rates, alert counts)
- **Flux lifecycle management** (activate, pause, resume, archive)
- **Intelligent alerting** with severity levels and resolution tracking
- **REST API** with Swagger UI
- **Azure-ready** infrastructure as code (Bicep)

---

## Architecture

```
flux-operations-board/
├── src/
│   ├── FluxOperations.Domain/          # Entities, Enums, Domain Events
│   ├── FluxOperations.Application/     # CQRS: Commands, Queries, Handlers, DTOs
│   ├── FluxOperations.Infrastructure/  # EF Core, SQL Server, Seed Data
│   ├── FluxOperations.API/             # ASP.NET Core 8, Swagger, Middleware
│   └── FluxOperations.Frontend/        # Angular 17, Material Design
├── tests/
│   ├── FluxOperations.Application.Tests/
│   └── FluxOperations.Domain.Tests/
├── azure/
│   ├── main.bicep                      # IaC: App Service, SQL, Static Web App, AI
│   ├── modules/keyvault.bicep
│   └── pipelines/azure-pipelines.yml
└── .github/workflows/ci.yml
```

### Design Patterns

| Pattern | Implementation |
|---------|---------------|
| **Clean Architecture** | Domain → Application → Infrastructure → API |
| **CQRS** | Commands and Queries separated via MediatR |
| **Mediator** | MediatR 12 with Pipeline Behaviours |
| **Repository** | IAppDbContext abstraction, EF Core implementation |
| **Domain Events** | BaseDomainEvent + INotification, dispatched on SaveChanges |

---

## Tech Stack

### Backend
| Technology | Version | Purpose |
|-----------|---------|---------|
| ASP.NET Core | 8.0 | REST API |
| MediatR | 12.2 | CQRS Mediator |
| Entity Framework Core | 8.0 | ORM + SQL Server |
| FluentValidation | 11.9 | Command validation |
| Swashbuckle | 6.5 | Swagger / OpenAPI |

### Frontend
| Technology | Version | Purpose |
|-----------|---------|---------|
| Angular | 17 | SPA Framework |
| Angular Material | 17 | UI Component Library |
| Chart.js + ng2-charts | 4.x / 6.x | Data visualization |
| RxJS | 7.8 | Reactive state & HTTP |

### Infrastructure & DevOps
| Technology | Purpose |
|-----------|---------|
| Azure App Service | API hosting (Linux, .NET 8) |
| Azure Static Web Apps | Angular hosting + CDN |
| Azure SQL Database | Managed relational database |
| Azure Application Insights | Monitoring & telemetry |
| Azure Bicep | Infrastructure as Code |
| Azure Pipelines | CI/CD (3 stages: Build → Staging → Prod) |
| GitHub Actions | Open-source CI + security scanning |

### Testing
| Technology | Purpose |
|-----------|---------|
| xUnit | Test framework |
| Moq | Mocking |
| FluentAssertions | Readable test assertions |
| EF InMemory | In-memory database for tests |

---

## Domain Model

```
Flux (aggregate root)
├── Status: Inactive → Active → Paused / Warning / Error / Completed
├── Type: ETL, DataPipeline, ApiIntegration, BatchProcessing, Streaming, FileTransfer, MessageQueue
├── Alerts[] (owned collection)
├── Metrics[] (owned collection)
└── Domain Events: FluxCreatedEvent, FluxStatusChangedEvent

Dashboard
└── Widgets[] (grid-based layout: KpiCard, LineChart, BarChart, AlertFeed, etc.)
```

### Flux Lifecycle

```
        ┌─────────┐
        │Inactive │
        └────┬────┘
             │ Activate()
        ┌────▼────┐
   ┌────│ Active  │────┐
   │    └────┬────┘    │
   │ Pause() │         │ RecordExecution(errorRate > 5%)
   │    ┌────▼────┐    │    ┌─────────┐
   │    │ Paused  │    └───►│ Warning │
   │    └────┬────┘         └─────────┘
   │  Resume()│             MarkAsError()→┌───────┐
   └──────────┘                           │ Error │
                                          └───────┘
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) (or full SQL Server)
- [Node.js 20+](https://nodejs.org) (for the Angular frontend)
- [Angular CLI 17](https://angular.dev/tools/cli): `npm install -g @angular/cli`

### Run the API

```bash
# Clone and restore
git clone https://github.com/your-org/flux-operations-board.git
cd flux-operations-board

# Run the API (auto-creates DB and seeds data)
dotnet run --project src/FluxOperations.API

# Swagger UI → http://localhost:5000
```

### Run the Angular Frontend

```bash
cd src/FluxOperations.Frontend
npm install
npm start          # ng serve on http://localhost:4200
```

The Angular dev server proxies `/api` requests to `https://localhost:7001`.

### Run Tests

```bash
dotnet test FluxOperations.sln
# Output: 39 tests, 0 failures
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/flux` | List fluxes (paginated, filterable) |
| `GET` | `/api/flux/{id}` | Get flux by ID |
| `POST` | `/api/flux` | Create a new flux |
| `PUT` | `/api/flux/{id}` | Update flux |
| `DELETE` | `/api/flux/{id}` | Archive flux (soft delete) |
| `POST` | `/api/flux/{id}/pause` | Pause an active flux |
| `POST` | `/api/flux/{id}/resume` | Resume a paused flux |
| `GET` | `/api/alert` | Get active alerts (filterable) |
| `POST` | `/api/alert` | Create manual alert |
| `POST` | `/api/alert/{id}/resolve` | Resolve an alert |
| `GET` | `/api/dashboard/metrics` | Get KPI metrics for dashboard |

Full interactive documentation available at the Swagger UI (`/` in development).

---

## Azure Deployment

### Infrastructure Provisioning

```bash
# Create resource group
az group create --name rg-fluxops-dev --location westeurope

# Deploy infrastructure
az deployment group create \
  --resource-group rg-fluxops-dev \
  --template-file azure/main.bicep \
  --parameters environment=dev sqlAdminLogin=fluxadmin sqlAdminPassword='<secure-pwd>'
```

### CI/CD Pipeline Variables

| Variable Group | Key | Description |
|---------------|-----|-------------|
| `FluxOps-Staging` | `AZURE_SERVICE_CONNECTION` | Azure DevOps service connection name |
| `FluxOps-Staging` | `API_APP_NAME_STAGING` | App Service name for staging API |
| `FluxOps-Staging` | `STATIC_WEB_APP_TOKEN_STAGING` | Static Web App deployment token |
| `FluxOps-Production` | `API_APP_NAME_PROD` | App Service name for prod API |
| `FluxOps-Production` | `STATIC_WEB_APP_TOKEN_PROD` | Static Web App deployment token |

---

## Project Structure Details

### Application Layer (CQRS)

```
Commands/
├── Fluxes/
│   ├── CreateFluxCommand + Handler + Validator
│   ├── UpdateFluxCommand + Handler
│   ├── DeleteFluxCommand + Handler
│   ├── PauseFluxCommand + Handler
│   └── ResumeFluxCommand + Handler
└── Alerts/
    ├── CreateAlertCommand + Handler
    └── ResolveAlertCommand + Handler

Queries/
├── Fluxes/
│   ├── GetAllFluxesQuery + Handler  (paginated + filtered)
│   └── GetFluxByIdQuery + Handler
├── Alerts/
│   └── GetActiveAlertsQuery + Handler
└── Dashboard/
    └── GetDashboardMetricsQuery + Handler

Common/
├── Behaviours/  LoggingBehaviour, ValidationBehaviour
├── Interfaces/  IAppDbContext
├── Mappings/    FluxMappingExtensions (no AutoMapper overhead)
└── Models/      Result<T>, PaginatedList<T>
```

### Angular Frontend Structure

```
src/app/
├── core/
│   ├── models/     flux.model.ts (typed interfaces)
│   ├── services/   flux.service.ts (all API calls)
│   └── interceptors/ error.interceptor.ts
├── features/
│   ├── dashboard/  DashboardComponent (KPIs + alerts + flux table)
│   └── flux/       FluxListComponent (CRUD + filters)
└── shared/
    ├── kpi-card/       KpiCardComponent
    └── status-badge/   StatusBadgeComponent
```

---

## Notes

- **AutoMapper** (v12.0.1) is referenced but intentionally unused — mapping is done via lightweight extension methods (`FluxMappingExtensions`) to avoid reflection overhead and the library's known security advisory (GHSA-rvv3-g6hj-g44x). It can be removed in a future cleanup.
- **Database seeding** runs automatically in `Development` and `Staging` environments on first startup with 7 realistic flux scenarios.
- **Domain Events** are cleared after `SaveChanges` — wire up an `INotificationHandler<>` to dispatch side effects (emails, SignalR push, audit log) as needed.

---

## License

MIT © 2024 — Flux Operations Board
