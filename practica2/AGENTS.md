# Repository Guidelines

## Project Structure & Module Organization
- `SistemaGuia.sln` is the main solution entry point.
- `Guia.API/` contains the ASP.NET Core API (`net8.0`).
- `Guia.API/Controllers/` exposes domain endpoints (`Personas`, `Bitacora`, `Retos`, `Numerologia`, `Astro`).
- `Guia.API/Models/` contains entities and DTOs; `Data/` contains `ApplicationDbContext` and seeders.
- `Guia.API/Migrations/` keeps EF Core schema history.
- `Guia.API/wwwroot/` serves static assets and default files.
- `Script/` stores SQL bootstrap scripts (for example `CreatTable.sql` plus dated seed scripts).
- `Guia.Web/` currently contains a minimal placeholder (`index.html`).

## Build, Test, and Development Commands
- `dotnet restore SistemaGuia.sln`: restore NuGet dependencies.
- `dotnet build SistemaGuia.sln -c Debug`: compile all projects in debug mode.
- `dotnet run --project Guia.API/Guia.API.csproj`: run API locally (Kestrel listens on port `1651`).
- `dotnet watch --project Guia.API/Guia.API.csproj run`: run API with hot reload.
- `dotnet ef database update --project Guia.API`: apply pending EF migrations.
- Optional SQL bootstrap: execute scripts from `Script/` when setting a fresh database.

## Coding Style & Naming Conventions
- Follow C# defaults: 4-space indentation, braces on new lines, `PascalCase` for types/methods/properties, `camelCase` for locals.
- Keep controllers focused on HTTP orchestration; move data/query logic to `Data/` or services.
- Use naming patterns like `LoginRequestDto`, `PersonaDetalleDto`, and `CreateXRequest`.
- Do not commit generated artifacts (`bin/`, `obj/`) or local user files (`*.csproj.user`).

## Testing Guidelines
- No test project is currently present.
- Add tests under `tests/` (for example, `tests/Guia.API.Tests`) using `xUnit`.
- Use test names in `MethodName_State_ExpectedResult` format.
- Run all tests from repo root with `dotnet test`.

## Commit & Pull Request Guidelines
- No usable git history is available in this workspace, so use a standard imperative style.
- Example commit: `Add ownership validation in Bitacora endpoints`.
- Keep commits scoped by concern (API logic, migrations, scripts, docs).
- PRs should include: objective, changed endpoints/files, migration impact, and sample request/response for API changes.

## Security & Configuration Tips
- Keep secrets out of `appsettings*.json`; use environment variables or Secret Manager.
- Treat `Guia.API/Program.cs` changes (CORS, seeding, middleware order) as security-sensitive.
- Review `SECURITY_HARDENING_PLAN.md` before modifying authentication, authorization, or transport settings.
