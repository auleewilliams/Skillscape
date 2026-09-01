# Skillscape instructions for GitHub Copilot

Read and follow [AGENTS.md](../AGENTS.md) for shared project guidance and
[the agent workflow](../docs/agent-workflow.md) for commands and unresolved decisions.
Keep shared guidance in those files rather than duplicating it here.

The repository currently has specifications and an empty solution, with no source
or test projects. Treat the .NET 10 / Blazor / EF Core / SQLite structure as planned.
Do not report a successful empty-solution command as tested application behavior.

Before implementing a task, read its spec and the constitution, check document
conflicts, preserve inward Core dependencies, and follow test-first development.
Do not silently decide the unresolved authentication policy or claim CI passed when
no CI workflow ran. Review changes for lost data, missing validation, archived member
visibility, duplicate skills, and deviations from acceptance criteria.
