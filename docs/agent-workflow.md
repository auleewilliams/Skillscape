# Working with agents

## Entry points

[AGENTS.md](../AGENTS.md) is the shared project guidance.
[CLAUDE.md](../CLAUDE.md) imports it for Claude Code, and
[Copilot instructions](../.github/copilot-instructions.md) point to it for GitHub
Copilot. For another tool, explicitly provide `AGENTS.md` and the relevant spec;
do not assume the tool automatically loads a filename it does not support.

File conventions follow the official documentation for
[Codex](https://learn.chatgpt.com/docs/agent-configuration/agents-md),
[Claude Code](https://code.claude.com/docs/en/memory), and
[GitHub Copilot](https://docs.github.com/en/copilot/how-tos/copilot-on-github/customize-copilot/add-custom-instructions/add-repository-instructions).
No hooks, credentials, permission overrides, or tool installations are required by
these artifacts. To check loading, start a fresh agent session at the repository
root and ask it to name the instruction files it loaded and summarize the current
project state. File checks alone do not verify a particular client's behavior.

## Starting a task

Read the [constitution](../.specify/memory/constitution.md), then the relevant
feature spec, plan, and tasks. For the current feature these live in
[`specs/001-skillscape-app/`](../specs/001-skillscape-app/).

Useful inspection commands from the repository root:

```sh
git status --short
git branch --show-current
rg --files --hidden -g '!.git'
dotnet --info
dotnet sln Skillscape.sln list
```

The solution currently has no projects. There is no established application build,
test, lint, or CI pipeline. Do not scaffold the app simply to verify a documentation
change. Check installed templates with `dotnet new list` when doing setup rather
than blindly copying the template command from task T002. Preserve the existing
solution instead of recreating it just because T001 remains unchecked.

For an implementation request, select a bounded task or story and its acceptance
criteria. Write behavior tests first, observe their expected failure, implement,
then refactor and rerun checks. Use the planned Core/Data/Web test split in the
plan and tasks; the quickstart's single test-project tree is inconsistent with it.

## Commands after scaffolding

These are intended commands, not a claim that the required projects, tools, or
configuration exist yet. Confirm paths and SDK/tool versions after setup.

```sh
dotnet restore Skillscape.sln
dotnet build Skillscape.sln --no-restore
dotnet test Skillscape.sln --no-build
dotnet run --project src/Skillscape.Web
```

Once EF tooling, the Data project, startup configuration, and migrations exist,
apply migrations only to the intended local development database:

```sh
dotnet ef database update --project src/Skillscape.Data --startup-project src/Skillscape.Web
```

Check the connection string and working directory first: a relative SQLite path
depends on runtime configuration. Never delete or reset a database as a routine
check. Add a coverage command only after a coverage collector is configured and
verified; inspect test discovery and results, not only the process exit code.

## Existing Spec Kit integration

The repository already contains Spec Kit templates, PowerShell scripts, and Claude
skills. Prefer those artifacts over introducing a second planning system.

- Read the relevant `.claude/skills/speckit-*/SKILL.md` before running its workflow.
- `pwsh` is required by the configured integration. The shell wrapper at
  `.specify/integrations/claude/scripts/update-context.sh` delegates to
  `.specify/scripts/bash/update-agent-context.sh`, which is absent in this checkout.
- Context refreshes can edit instruction files. Inspect the script and its selected
  feature before running it; review the diff afterward and preserve the `AGENTS.md`
  import and curated guidance. Do not refresh context just to format documentation.
- Existing task IDs are the progress record. Avoid maintaining a competing task
  list in the agent instructions or checking tasks off without verification.

## Known document conflicts

These observations are not approved changes to the spec or constitution. Resolve
each conflict in the owning documents when the relevant work is undertaken; do not
quietly encode a new requirement in application code. Independent work can proceed.

| Topic | Evidence | Required handling |
| --- | --- | --- |
| Authentication and personal data | The constitution requires authentication/authorization and encryption of sensitive data. The spec and research intentionally omit authentication; the plan labels this a note. | Obtain a project decision and use the constitution's amendment process if changing policy. The plan's note is not an exemption. Do not deploy or expose the unauthenticated design in the meantime. |
| Core persistence dependencies | The plan keeps Core independent and Data dependent on Core, but T023 injects Data's `AppDbContext` into a service in Core; research recommends direct injection. | Reconcile service placement and persistence interfaces without creating a Core/Data dependency cycle. Document the chosen design before coding it. |
| Member IDs | The data model and tasks use `Guid`; the route contract describes integer IDs. | Align the contract and implementation plan before implementing routes. Do not mix identifier types. |
| Setup instructions | The solution already exists; T001 recreates it. T002 names a template that must be checked against the installed SDK. The quickstart shows one test project; the plan/tasks show three. | Verify actual SDK templates and paths during scaffolding, then update the affected instructions. |
| Archive metadata and restoration | Research mentions `ArchivedAt` and restoration; the data model has no `ArchivedAt` and explicitly defines no restore workflow. | Do not add restoration or fields from research alone; reconcile scope with the spec and model. |
| EF lifetime | Research assumes a circuit-scoped `DbContext` provides safe per-user access. | Verify current Blazor/EF guidance and define context ownership and concurrent-operation handling before implementing data access. |

## Handoffs and reviews

Use the [handoff template](templates/agent-handoff.md) for substantial unfinished
work. Keep handoff notes with the relevant feature or in the task conversation;
do not commit machine-specific paths, secrets, or speculative success claims.
For parallel work, agree on file ownership and dependencies first. Shared files
such as `Program.cs`, package properties, migrations, and task lists need an owner.

Before review, inspect the entire diff, run appropriate verification, update relevant
documentation, and list any checks that could not run. The
[PR template](../.github/pull_request_template.md) captures spec linkage,
constitution compliance, and evidence. Required CI checks and peer approval remain
merge gates even when an agent has reviewed the work.

Maintain these instructions when scaffolding, commands, architecture, or policy
changes. Keep project facts in `AGENTS.md`, longer procedures here, and tool-specific
loading details in the adapters.
