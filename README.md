# TaskPlanner

A full-stack task management application built as a take-home interview exercise. Users can register, log in, and manage their own task items through a clean, functional UI backed by a secured REST API.

---

## Tech Stack

**Backend**
- .NET 9 / ASP.NET Core
- SQLite via `Microsoft.Data.Sqlite` — raw SQL only, no ORM (no Entity Framework, no Dapper)
- JWT authentication via `Microsoft.AspNetCore.Authentication.JwtBearer`
- Password hashing via `Microsoft.Extensions.Identity.Core`
- Input validation via FluentValidation
- Swagger / Swashbuckle for API documentation

**Frontend**
- Angular 21 (standalone components, no NgModules)
- Tailwind CSS v4
- TypeScript

---

## Architecture

### Backend — Clean Architecture

The backend is structured in four layers with strict dependency direction (outer layers depend on inner layers, never the reverse):

| Layer | Responsibility |
|---|---|
| `TaskPlanner.Api` | Controllers, middleware, DI composition root |
| `TaskPlanner.Application` | Services, DTOs, request/response models, validators, interfaces |
| `TaskPlanner.Domain` | Entities, domain interfaces |
| `TaskPlanner.Infrastructure` | SQLite repositories, JWT token service, password hashing, DB initializer and seeder |

The database schema is created and seeded on startup through an initializer registered in the infrastructure layer. There is no migration tooling — schema scripts are managed in code.

### Frontend — Angular

```
src/app/
  core/
    guards/         auth and guest route guards (functional, CanActivateFn)
    interceptors/   JWT token attachment and 401 error handling
    navbar/         app shell navbar component
    services/       AuthSession (reactive signal-based session state), TokenStorage
  features/
    auth/           login and register pages, auth API service, contracts
    task-items/     task item list page, task item API service, contracts
  shared/
    contracts/      shared API error type
```

Key frontend patterns used:
- Standalone components throughout — no NgModules
- Angular signals (`signal`, `computed`) for all reactive state — no plain mutable properties
- Functional route guards and HTTP interceptors
- Lazy-loaded routes via `loadComponent`
- Angular 17+ control flow syntax (`@if`, `@for`, `@else`) — no structural directives

---

## Setup Instructions

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS) and npm
- Angular CLI: `npm install -g @angular/cli`

### Backend

```bash
cd src/Server/TaskPlanner.Api
dotnet run
```

The API starts at `https://localhost:7074`. On first run, the SQLite database file (`taskplanner.db`) is created and seeded automatically — no manual setup required.

Swagger UI is available at `https://localhost:7074/swagger`.

### Frontend

```bash
cd src/Client/task-planner-web
npm install
ng serve
```

The app runs at `http://localhost:4200`.

---

## Demo Users and Seed Data

Two users and sample task items are seeded on startup.

| Display Name | Email | Password |
|---|---|---|
| Planner User | planner@taskplanner.dev | Planner@123 |
| Admin User | admin@taskplanner.dev | Admin@123 |

Each user has pre-seeded task items associated with their account.

---

## Key Endpoints

### Auth

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Create a new account |
| POST | `/api/auth/login` | Authenticate and receive a JWT |

### Task Items

All task item endpoints require a valid `Authorization: Bearer <token>` header. Each user only has access to their own items.

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/task-items` | List task items for the authenticated user |
| POST | `/api/task-items` | Create a task item |
| PUT | `/api/task-items/{id}` | Update a task item |
| DELETE | `/api/task-items/{id}` | Delete a task item |

### Frontend Routes

| Route | Access | Description |
|---|---|---|
| `/login` | Guest only | Login page |
| `/register` | Guest only | Register page |
| `/task-items` | Authenticated | Task management page |

---

## Testing

Tests are written with xUnit and organized across four projects mirroring the backend layers:

```
src/Tests/
  TaskPlanner.Domain.Tests/
  TaskPlanner.Application.Tests/     service logic and FluentValidation rules
  TaskPlanner.Infrastructure.Tests/  repository integration tests (in-memory SQLite), JWT, seeder
  TestPlanner.Api.Tests/             controller-level tests
```

To run all tests from the solution root:

```bash
dotnet test
```

---

## AI Usage and Reflection

Both Claude and OpenAI Codex (integrated in VS Code) were used throughout this project as development accelerators — for scaffolding structure, drafting initial implementations, and cross-checking patterns against Angular and .NET conventions.

The two tools were often consulted in parallel. When a recommendation from one did not hold up under scrutiny, the other was used as a second opinion — and vice versa. Neither was treated as authoritative. The workflow was deliberate: every suggestion was reviewed before being accepted, and AI output was treated as a starting point rather than a final answer. Where suggestions conflicted with my understanding of the framework, the domain, or the intended design, I pushed back, asked for alternatives, or discarded the suggestion entirely. Proposals that were technically valid but added unnecessary complexity for this scope were also rejected.

AI was not used to bypass understanding — it was used to move faster through work I had already reasoned about independently.
