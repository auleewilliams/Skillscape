# Skillscape – Architectural Research

Technical decisions for the Skillscape internal skills visibility web app (ASP.NET Core 10, Blazor Server, EF Core 10, SQLite, MudBlazor).

---

## Blazor Server vs Blazor WebAssembly

**Decision**: Blazor Server  
**Rationale**: The app runs on an internal network with a stable server connection, so the latency trade-off of Blazor Server is negligible. Server-side rendering keeps all data access logic on the server (no API layer needed), simplifies EF Core integration, and produces a fast initial load with no large WASM download. Up to 500 concurrent users is well within Blazor Server's connection scaling limits on a single host.  
**Alternatives considered**: Blazor WebAssembly requires a separate API, cannot use EF Core directly, and has a heavier initial payload — all unnecessary overhead for an intranet app with no offline requirement.

---

## MudBlazor vs Other Blazor Component Libraries

**Decision**: MudBlazor  
**Rationale**: MudBlazor is free, open-source (MIT), and offers a comprehensive Material Design component set including `MudAutocomplete`, `MudDataGrid`, and `MudSelect` — all needed for this app. It has the largest community of the OSS Blazor UI libraries, strong documentation, and active maintenance. No per-developer licensing cost matters for an internal tool.  
**Alternatives considered**: Radzen Blazor is also free but has a smaller component surface and less polish. Telerik UI for Blazor is production-quality but requires a paid license, unjustifiable for an internal app. Ant Design Blazor is a community port with slower release cadence and less complete documentation.

---

## EF Core + SQLite for This Scale

**Decision**: EF Core 8 with SQLite  
**Rationale**: A team of up to 500 members with self-managed skill profiles represents a small dataset (likely under 50,000 rows total). SQLite handles this comfortably with single-digit millisecond query times. The file-based deployment model eliminates a database server dependency, simplifying setup and backup. Indexing strategy: add a unique index on `TeamMember.Email`, a non-unique index on `Skill.NormalisedName`, and a composite index on `TeamMemberSkill(TeamMemberId, SkillId)`.  
**Alternatives considered**: SQL Server LocalDB adds operational complexity with no performance benefit at this scale. PostgreSQL is appropriate for larger deployments but introduces a server dependency that is not warranted here.

---

## SQLite Migrations Approach

**Decision**: EF Core Migrations (`dotnet ef migrations add`)  
**Rationale**: EF Core Migrations is already part of the stack, keeps schema changes version-controlled alongside the model, and supports `Database.MigrateAsync()` for automatic migration on startup — a good fit for a lightweight internal app with no DBA. The migration history table provides an audit trail of schema changes.  
**Alternatives considered**: FluentMigrator adds a second migration abstraction that duplicates EF Core functionality; only justified when targeting multiple database engines. Manual SQL scripts require discipline to keep in sync with the EF model and offer no advantage here.

---

## Autocomplete / Typeahead Pattern in Blazor Server

**Decision**: Server-side filtered query via `MudAutocomplete` `SearchFunc`  
**Rationale**: `MudAutocomplete` accepts an async `SearchFunc<string>` that fires on each keystroke with a configurable debounce. Because the app runs Blazor Server, the callback executes directly against EF Core with no HTTP round-trip overhead beyond the existing SignalR channel. A query such as `WHERE NormalisedName LIKE @prefix` against an indexed column returns results fast enough that client-side caching is unnecessary.  
**Alternatives considered**: Loading all skill names into the browser on page load and filtering client-side is simpler but does not scale cleanly and bypasses the normalisation layer on the server. A custom JS interop typeahead adds complexity with no benefit inside Blazor Server.

---

## Skill Name Normalisation Strategy

**Decision**: Trim whitespace, lowercase, and store a `NormalisedName` shadow column on `Skill`  
**Rationale**: User-entered skill names are trimmed and lowercased before comparison to detect duplicates (e.g. "c#", "C#", " C# " all resolve to the same record). The original casing supplied first is stored as `DisplayName`; subsequent matches reuse the existing record. A unique index on `NormalisedName` enforces deduplication at the database level.  
**Alternatives considered**: Case-insensitive collation on the `Name` column alone is SQLite-collation-dependent and unreliable for non-ASCII characters. Fuzzy/phonetic matching (e.g. Levenshtein) was considered but adds complexity and false-positive risk — exact normalised match is sufficient for skill names.

---

## Archive Pattern for Team Members

**Decision**: Soft-delete with `IsArchived` boolean flag on `TeamMember`  
**Rationale**: Archiving hides a team member from the directory and search results while retaining their skill data for historical reporting. A single `IsArchived` flag is the lowest-complexity approach: all queries add a `WHERE IsArchived = 0` filter (enforced via a global query filter in EF Core), and restoring a member is a single field update. An `ArchivedAt` timestamp is added for audit purposes.  
**Alternatives considered**: A separate `ArchivedTeamMembers` table requires data movement on archive/restore and complicates queries that need to span both sets. Hard-delete destroys skill history, which is explicitly out of scope.

---

## bUnit for Blazor Component Testing

**Decision**: bUnit  
**Rationale**: bUnit is the de-facto standard for unit-testing Blazor components, integrates naturally with xUnit (already in the stack), and supports both Razor and C# test syntax. It provides a `TestContext` that renders components in-memory, allowing parameter passing, event triggering, and DOM assertions without a browser. Moq can be used to mock injected services within bUnit's `Services` collection.  
**Alternatives considered**: Playwright or Selenium provide full browser-based integration tests but are slower, require a running host, and are disproportionate for component-level logic validation. There is no meaningful alternative to bUnit for in-process Blazor component testing.

---

## Repository Pattern vs Direct DbContext Injection

**Decision**: Direct `DbContext` injection into Blazor service classes (no Repository pattern)  
**Rationale**: EF Core's `DbContext` already implements the Unit of Work and Repository patterns internally. Adding a repository layer duplicates abstractions, increases boilerplate, and provides no practical benefit for a single-database app of this scale. Blazor Server's scoped `DbContext` lifetime (one per circuit) ensures safe per-user data access. Services are thin wrappers that handle business logic (normalisation, archiving) and delegate persistence directly to `AppDbContext`.  
**Alternatives considered**: A generic `IRepository<T>` layer is common in enterprise codebases targeting multiple data sources or requiring extensive mocking. Neither condition applies here — EF Core's in-memory provider or SQLite in-memory mode is sufficient for testing without a repository abstraction.

---

## No Authentication

**Decision**: Authentication is intentionally omitted  
**Rationale**: Skillscape is an internal intranet tool accessible only within the corporate network. All team members are trusted to self-manage their own profiles; there is no sensitive data (no PII beyond name and skills) and no role-based access control requirement. Omitting authentication eliminates identity provider dependencies, reduces infrastructure complexity, and removes onboarding friction.  
**Alternatives considered**: Windows Authentication (negotiate/Kerberos) was evaluated as a zero-friction option for corporate environments but adds IIS/Kesteros configuration complexity and was deemed unnecessary given the low-sensitivity data. ASP.NET Core Identity was ruled out as it introduces a user-management surface that duplicates HR system data without adding value.

---

## NuGet Central Package Management

**Decision**: Use NuGet Central Package Management (CPM) with `Directory.Packages.props` at the solution root  
**Rationale**: CPM ensures all three source projects and three test projects reference the same package versions without repetition or drift. Version bumps happen in one file, eliminating the risk of different projects silently running different versions of EF Core, MudBlazor, or xUnit. This is a low-cost practice with high long-term maintainability benefit.  
**Alternatives considered**: Per-project `Version` attributes on `<PackageReference>` elements is the default approach but becomes error-prone as the solution grows. A `Directory.Build.props` `<PackageVersion>` approach is effectively what CPM formalises — CPM is the official Microsoft-supported mechanism for this.

---
