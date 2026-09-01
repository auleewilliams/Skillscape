# Agent guidance for Skillscape

## Project and current state

Skillscape is an internal team skills directory: browse members, maintain skill
profiles, and find people by technical or domain/application knowledge.

This repository currently contains specifications, Spec Kit tooling, and an empty
`Skillscape.sln`. There are no application or test projects, shared build/package
properties, migrations, or CI workflows yet. Check the actual tree at the start of
each task; planned files and unchecked tasks are not implemented features.

The planned stack is C# / .NET 10, ASP.NET Core with Blazor Server, MudBlazor,
EF Core 10 with SQLite, and xUnit / bUnit / Moq. Do not introduce a separate SPA,
API project, database platform, or external runtime service without a requirement.

## Read before making changes

- [Constitution](.specify/memory/constitution.md): project quality and governance rules.
- [Feature specification](specs/001-skillscape-app/spec.md): scope and acceptance criteria.
- [Implementation plan](specs/001-skillscape-app/plan.md): proposed architecture.
- [Task list](specs/001-skillscape-app/tasks.md): task IDs, dependencies, and progress.
- [Data model](specs/001-skillscape-app/data-model.md) and
  [UI routes](specs/001-skillscape-app/contracts/ui-routes.md): relevant contracts.
- [Agent workflow](docs/agent-workflow.md): commands, known conflicts, and handoffs.

This file is the shared agent entry point; tool-specific files should refer here
instead of maintaining separate project rules. The constitution governs repository
policy; these instructions do not amend it. When documents conflict, identify the
conflict and resolve it in the affected documents before implementing that decision.
Continue independent work while an unresolved decision is pending.

## Architecture and implementation

The planned source projects are `src/Skillscape.Core`, `src/Skillscape.Data`, and
`src/Skillscape.Web`, with matching projects under `tests/`.

- Core owns domain entities, business rules, and application interfaces. It must
  not reference Data, EF Core, Blazor, or other infrastructure.
- Data owns EF Core mappings, migrations, and persistence implementations; it may
  depend on Core. Web owns UI and dependency-injection composition.
- Follow inward dependencies. Some current tasks incorrectly inject Data's
  `AppDbContext` into Core; reconcile those tasks before implementing them.
- Use NuGet Central Package Management when scaffolding: versions belong in
  `Directory.Packages.props`, not individual `PackageReference` elements.
- Use nullable reference types, meaningful names, focused classes, enums for
  category/proficiency, and XML documentation for public APIs and domain types.
- Use MudBlazor and consistent accessible interactions. Include loading, empty,
  validation, not-found, and recoverable error states where applicable.
- Keep database operations asynchronous. Choose and document a safe DbContext
  lifetime for Blazor before implementing persistence; do not assume that a
  circuit-scoped context is safe for overlapping operations.

## Domain rules to preserve

- `TeamMember`, `Skill`, and `TeamMemberSkill` form the planned domain model.
- Categories are `Technical` and `DomainKnowledge`; business application is an
  optional field, not a third category.
- Proficiency is ordered `Beginner`, `Intermediate`, `Expert`; skill-search results
  rank Expert first.
- Normalize skill names by trimming and lower-casing for matching; preserve display
  casing separately. Prevent duplicate assignments of a skill to a member.
- Duplicate member names are allowed; role and department help distinguish people.
- Archive members rather than deleting their history. Exclude archived members
  from ordinary directory and search results.
- Validate at the UI boundary and in application logic before persistence.

## Working and verifying

1. Inspect the branch and working tree. Preserve existing user changes; do not
   reset, clean, or overwrite unrelated work. Reuse an appropriate existing worktree.
2. Tie implementation to its spec and task IDs. Keep changes focused and respect
   prerequisites; do not begin dependent stories before their foundation exists.
3. Follow Red-Green-Refactor for behavior changes. Test business rules with xUnit,
   persistence with SQLite integration tests, and components with bUnit. The
   constitution requires at least 80% business-logic coverage and passing CI.
4. Run relevant checks and report their actual results. A successful command on an
   empty solution or zero discovered tests is not application verification.
   Documentation-only work needs link, consistency, and diff checks, not dummy tests.
5. Update task checkboxes only for completed work, and distinguish local verification
   from CI results. Finish with changed files, checks, limitations, and next steps.

See the workflow guide for current inspection commands and commands that become
usable after scaffolding. Do not claim build, test, coverage, accessibility, or
performance success without evidence. Performance targets in the plan are targets,
not measured results.

## Safety and review

- Never commit credentials, real personnel data, local databases, or private logs.
  Use synthetic test/seed data. Do not assume names and skills are non-sensitive.
- Do not expose or deploy the app while the authentication policy conflict in the
  workflow guide is unresolved. This setup does not grant deployment permission.
- Do not disable safeguards, rewrite history, reset a database, publish, or send
  external messages without authorization covering that action.
- Feature work belongs on a spec-linked branch, not directly on `main`. The
  constitution requires peer approval before merge and prohibits self-merges.
- If multiple agents are used, give each a bounded scope and clear file ownership;
  coordinate changes to shared files and verify the combined result. An agent
  handoff or automated review does not replace required peer approval.

## Code Review Rules

Flag behavior that violates the spec, loses archived history, duplicates skills,
includes archived members in ordinary results, bypasses validation, or introduces
outward Core dependencies. Check that tests exercise observable behavior and real
SQLite constraints where relevant. Report concrete findings with file locations,
impact, and evidence; distinguish existing document conflicts from new regressions.
