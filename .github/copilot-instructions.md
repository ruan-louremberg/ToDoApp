# Copilot instructions for ToDoApp

## Build, test, lint, and run

The repository targets .NET 10 and uses Node.js for the React frontend. Run commands from the repository root unless noted.

### Backend

```powershell
dotnet restore
dotnet build ToDoApp.slnx
dotnet test
```

Run one test by fully qualified name, class, or display-name substring:

```powershell
dotnet test tests/Todo.Tests/Todo.Tests.csproj --filter "FullyQualifiedName~CreateTaskUseCaseTests"
dotnet test tests/Todo.Tests/Todo.Tests.csproj --filter "DisplayName~Cria tarefa"
```

The SQLite schema is managed by versioned EF Core migrations. Install the EF tool once if needed, then update the database from the repository root:

```powershell
dotnet tool install --global dotnet-ef
dotnet ef database update --project src/Todo.Infrastructure --startup-project src/Todo.Api
```

Run the API with `dotnet run --project src/Todo.Api`. Its startup code applies pending migrations as well. Configuration uses `ConnectionStrings:DefaultConnection`, with `Data Source=todo.db` as the fallback.

### Frontend

```powershell
cd frontend
npm install
npm run build
npm run lint
npm run dev
```

There is currently no frontend test script or frontend test suite. The frontend API base URL must be supplied through `VITE_API_URL`; do not hard-code the API host in components or API helpers.

The repository also contains Dockerfiles and `docker-compose.yml`; the direct .NET/Node commands above are the primary local development flow documented by the project.

## Architecture

This is a full-stack task-management application split into four .NET projects and a React SPA:

- `Todo.Domain` contains entities, enums, domain exceptions, the `Optional<T>` patch-field type, and repository abstractions. It must remain independent of ASP.NET Core and EF Core.
- `Todo.Application` contains request/response DTOs, FluentValidation validators, and use cases. Use cases validate input, coordinate repositories, and return `FluentResults` for expected failures.
- `Todo.Infrastructure` implements repository interfaces with EF Core/SQLite, owns `TodoDbContext`, and stores all schema changes as migrations under `Migrations`.
- `Todo.Api` is the HTTP composition root. Controllers map `/api/tasks` and `/api/categories` to use cases, `Program.cs` configures dependency injection/CORS/JSON/OpenAPI, and `ExceptionHandlingMiddleware` converts unexpected/domain/JSON failures to `ProblemDetails`.
- `frontend` is a React 19 + TypeScript + Vite SPA. `api/` contains fetch clients, `hooks/` coordinates API state and refresh behavior, `types/` mirrors API contracts, `components/` contains reusable UI, and `pages/` composes screens. `App.tsx` currently composes the summary panel and task list page.

The normal request path is controller -> application use case -> domain/repository interface -> infrastructure repository -> EF Core/SQLite. Keep HTTP and persistence details out of domain/application code, and keep UI components from owning raw API contract logic.

## Repository-specific conventions

- Use the existing task and category routes: `/api/tasks` and `/api/categories`. Task updates are partial `PATCH` operations; status changes use `/api/tasks/{id}/status`, and restore/trash endpoints are separate routes.
- Expected application failures should be returned as `Result` errors with a `statusCode` metadata value so `ApiControllerBase` can map them to HTTP responses. Do not replace this with ad-hoc controller exception handling.
- Input rules belong in the corresponding FluentValidation validator. Validators are registered by assembly scanning in `Program.cs`; use cases still invoke their validator before changing state.
- API errors use the `ProblemDetails` contract (`application/problem+json`). Frontend API helpers should parse that shape and expose the server-provided `detail` through `ApiError`.
- Deletes are soft deletes. Tasks and categories retain `IsDeleted`/`DeletedAt`; normal queries exclude deleted records, while trash and restore use explicit repository/use-case paths. Deleting a category must preserve its tasks by clearing the task relationship rather than deleting tasks.
- Task listing is filtered, sorted, and paginated in the repository. `ListTasksUseCase` clamps page values (`page >= 1`, `pageSize` between 1 and 100) and returns pagination metadata; preserve those bounds when changing list behavior.
- Partial task updates use `Optional<T>` to distinguish an omitted field from an explicitly supplied null for nullable fields such as `DueDate` and `CategoryId`. Preserve this distinction in DTOs, JSON conversion, validators, and entity update methods.
- Domain entities expose private setters and mutate through methods such as `SetUpdate`, `ChangeStatus`, `SoftDelete`, and `Restore`. Prefer those methods over setting entity properties directly.
- Repository implementations should honor cancellation tokens and use `AsNoTracking()` for read-only queries where the existing pattern does so. Repository interfaces live in `Todo.Domain`; implementations belong in `Todo.Infrastructure`.
- Frontend search is debounced before fetching. Hooks own loading/error/data state and refresh after writes; screens must represent loading, error, empty, and populated states.
- Frontend request/response types belong in `frontend/src/types`, API calls in `frontend/src/api`, and reusable asynchronous behavior in hooks. Use the existing numeric enum mapping for task status and priority when translating filter controls to API query parameters.
- Frontend linting is Oxlint through `npm run lint`, with React and TypeScript plugins configured in `frontend/.oxlintrc.json`. Keep component-specific styles in CSS modules when adding new component styles.
