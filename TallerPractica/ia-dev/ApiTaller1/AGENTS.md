# Repository Guidelines

## Project Structure & Module Organization
This repository is a single ASP.NET Core Web API project (`ApiTaller1.csproj`, target `net8.0`). Main folders:
- `Controllers/`: HTTP endpoints (`OrdenReciboController`, sample `WeatherForecastController`).
- `Service/`: business service layer (`ServicesOrdenRecibo`).
- `DA/`: data-access queries and EF Core usage (`DataRepositoryOrdenRecibo`).
- `Interface/`: service contracts (`IOrdenRecibo`).
- `Model/dto/`: local DTOs; `Model/database/ormdb.dll` is an external model/data assembly reference.
- `Properties/launchSettings.json`: local run profiles (`http`, `IIS Express`).

Keep generated artifacts (`bin/`, `obj/`) out of code changes.

## Build, Test, and Development Commands
Run from repo root:
- `dotnet restore ApiTaller1.sln` - restore NuGet packages.
- `dotnet build ApiTaller1.sln -c Debug` - compile locally.
- `dotnet run --project ApiTaller1.csproj` - start API (default `http://localhost:5062`, Swagger at `/swagger`).
- `dotnet watch run --project ApiTaller1.csproj` - run with hot reload for endpoint iteration.

## Coding Style & Naming Conventions
Use C# conventions already present in the codebase:
- 4-space indentation, braces on new lines, `nullable` enabled.
- PascalCase for public types/methods/properties (`ConsultaOrdenRevisadaObject`).
- `I` prefix for interfaces (`IOrdenRecibo`).
- Controller names end with `Controller`; DTOs use `Dto*` naming.

Prefer constructor injection over creating dependencies inline when extending services/repositories.

## Testing Guidelines
There is currently no dedicated test project in this repository snapshot. For new work:
- Create `ApiTaller1.Tests` with xUnit.
- Name test files as `<ClassName>Tests.cs` and methods as `Method_Scenario_ExpectedResult`.
- Run tests with `dotnet test` from solution root.

## Commit & Pull Request Guidelines
Git history is not available in this workspace snapshot, so follow a consistent convention going forward:
- Commit format: `type(scope): short imperative summary` (example: `feat(controller): add ORProductosRevisadosSP filter`).
- Keep commits focused to one logical change.
- PRs should include: purpose, key changes, affected endpoints, and manual test evidence (request/response examples or Swagger screenshots).

## Security & Configuration Tips
Do not hardcode connection strings or secrets in source files. Store environment-specific values in `appsettings.Development.json` or user-secrets/environment variables, and avoid committing sensitive values.
