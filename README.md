# Skillscape

Team skills management software for discovering technical skills and domain or
business-application knowledge across a team.

The repository is currently in the specification stage: `Skillscape.sln` is empty,
and application/test projects have not been scaffolded. The planned stack is
.NET 10, ASP.NET Core / Blazor Server, MudBlazor, and EF Core with SQLite.

## Working in this repository

- [Agent instructions](AGENTS.md): shared guidance for coding agents.
- [Agent workflow](docs/agent-workflow.md): setup status, verification, known
  specification conflicts, and handoffs.
- [Claude Code instructions](CLAUDE.md) and
  [GitHub Copilot instructions](.github/copilot-instructions.md): tool entry points.
- [Project constitution](.specify/memory/constitution.md): quality and governance.
- [Feature specification](specs/001-skillscape-app/spec.md),
  [implementation plan](specs/001-skillscape-app/plan.md), and
  [tasks](specs/001-skillscape-app/tasks.md): intended application and delivery plan.

The existing [quickstart](specs/001-skillscape-app/quickstart.md) describes the
intended scaffolded application. Its commands and paths still need validation
during implementation; consult the agent workflow before using them.
