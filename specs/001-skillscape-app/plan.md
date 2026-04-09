# Implementation Plan: Skillscape — Team Skills Visibility Application

**Branch**: `001-skillscape-app` | **Date**: 2026-04-09 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-skillscape-app/spec.md`

## Summary

Build a browser-based internal web application that replaces a team skills Excel spreadsheet. Team members self-register, maintain their own skill profiles (technical and domain/application knowledge), and the full team's skills are immediately browsable and searchable by anyone with access. No authentication is required. Built with ASP.NET Core 10 + Blazor Server + SQLite via Entity Framework Core.

## Technical Context

**Language/Version**: C# / .NET 10
**Primary Dependencies**: ASP.NET Core 10, Blazor Server, Entity Framework Core 10 (SQLite provider), MudBlazor (component library), xUnit, bUnit
**Package Management**: NuGet Central Package Management — all package versions declared in `Directory.Packages.props` at solution root; individual project files omit `Version` attributes
**Storage**: SQLite (file-based, zero-configuration; appropriate for up to 500 team members, single-tenant)
**Testing**: xUnit (unit/integration), bUnit (Blazor component tests), Moq (test doubles)
**Target Platform**: Web browser — desktop/laptop (Blazor Server, served from ASP.NET Core host)
**Project Type**: Web application (internal, single-tenant)
**Performance Goals**: Directory page loads in <1s for 500 members; skill search returns results within 200ms
**Constraints**: API/query responses <200ms p95; perceived UI interaction <100ms; DB queries <50ms; no external network calls required at runtime
**Scale/Scope**: ~500 team members, ~50 skills per member, single organisation, single deployment

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I — Code Quality | PASS | XML docs on all domain types and public services; proficiency/category as enums (no magic strings); SRP enforced via layered architecture |
| II — TDD (NON-NEGOTIABLE) | PASS | xUnit + bUnit; Red-Green-Refactor enforced; 80% coverage target for Domain and Application layers; integration tests for EF Core repositories |
| III — UX Consistency | PASS | MudBlazor provides unified design system with built-in WCAG 2.1 AA accessibility; no bespoke one-off components unless documented |
| IV — Performance | PASS | EF Core with indexed SQLite queries; autocomplete uses server-side filtered queries; 200ms p95 target met by design at this scale |
| V — Clean Architecture | PASS | Three-layer split: Core (domain + application) -> Data (EF Core infrastructure) -> Web (Blazor UI); domain has zero infrastructure dependencies |
| Security & Data | NOTE | Authentication intentionally omitted per spec (replacing zero-security Excel sheet); input validation MUST be enforced at Blazor form boundary and service layer; no secrets in source control |

**Complexity Tracking**: No constitution violations requiring justification.

## Project Structure

### Documentation (this feature)

```
specs/001-skillscape-app/
+-- plan.md              # This file
+-- research.md          # Phase 0 output
+-- data-model.md        # Phase 1 output
+-- quickstart.md        # Phase 1 output
+-- contracts/           # Phase 1 output
|   +-- ui-routes.md
+-- tasks.md             # Phase 2 output (/speckit.tasks - not created here)
```

### Source Code (repository root)

```
src/
+-- Skillscape.Core/             # Domain entities, enums, application services, interfaces
|   +-- Entities/
|   +-- Enums/
|   +-- Services/
|   +-- Interfaces/
+-- Skillscape.Data/             # EF Core DbContext, migrations, repository implementations
|   +-- Context/
|   +-- Migrations/
|   +-- Repositories/
+-- Skillscape.Web/              # Blazor Server app - pages, components, DI wiring
    +-- Components/
    |   +-- Layout/
    |   +-- Directory/
    |   +-- Profile/
    |   +-- Skills/
    +-- Pages/
    +-- Program.cs

tests/
+-- Skillscape.Core.Tests/       # Unit tests for domain logic and application services
+-- Skillscape.Data.Tests/       # Integration tests for EF Core repositories (SQLite in-memory)
+-- Skillscape.Web.Tests/        # bUnit component tests for Blazor UI

Directory.Packages.props         # Central Package Management - all NuGet versions declared here
Directory.Build.props            # Shared MSBuild properties (TreatWarningsAsErrors, Nullable, etc.)
Skillscape.sln
```

**Structure Decision**: Three-project source layout (Core / Data / Web) maps directly to Clean Architecture layers. Tests mirror the source projects one-to-one. Blazor Server is chosen over Blazor WASM to avoid a separate API project and to keep server-side data access simple and fast.
