# CLAUDE.md

<!-- MANUAL ADDITIONS START -->
@AGENTS.md

## Claude Code integration

Use the imported shared guidance as the project instructions. Read
[the agent workflow](docs/agent-workflow.md) for commands and known specification
conflicts before implementation.

Existing Spec Kit skills live in `.claude/skills/speckit-*/SKILL.md`. Read the
relevant skill before using its workflow. The installed integration uses PowerShell
scripts under `.specify/scripts/powershell/`; it requires `pwsh`. The Bash context
wrapper currently points to a shared Bash script that is not present.

Keep this import and these notes when refreshing agent context. Review generated
changes: extracted plan text does not resolve conflicts or prove that planned code
exists. Put shared project guidance in `AGENTS.md`, not here.
<!-- MANUAL ADDITIONS END -->
