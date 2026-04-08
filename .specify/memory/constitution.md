<!--
Sync Impact Report
==================
Version change: (none) → 1.0.0 (initial ratification)
Modified principles: N/A — first version
Added sections:
  - Core Principles (I–V)
  - Security & Data Standards
  - Development Workflow
  - Governance
Removed sections: N/A — first version
Templates reviewed:
  - .specify/templates/plan-template.md   ✅ No updates required (Constitution Check is template-generic)
  - .specify/templates/spec-template.md   ✅ No updates required (generic template)
  - .specify/templates/tasks-template.md  ✅ No updates required (generic template)
  - .specify/templates/checklist-template.md ✅ No updates required (generic template)
  - CLAUDE.md                             ✅ No updates required (no principle references)
  - README.md                             ✅ No updates required (no principle references)
Deferred TODOs: None
-->

# Skillscape Constitution

## Core Principles

### I. Code Quality

All production code MUST be clean, readable, and maintainable.

- Files and functions MUST have a single, clearly defined responsibility (Single Responsibility
  Principle).
- Code MUST be peer-reviewed before merge; self-merges are prohibited.
- Technical debt MUST be tracked; any debt introduced MUST have a resolution plan within the
  current or next sprint.
- Magic numbers and strings MUST be replaced with named constants or configuration values.
- All public APIs and domain types MUST carry XML documentation comments.

**Rationale**: Unmanaged complexity is the primary driver of defects and velocity loss. A shared,
non-negotiable quality bar prevents entropy across the codebase over time.

### II. Test-Driven Development (NON-NEGOTIABLE)

Tests MUST be written before implementation code.

- The Red-Green-Refactor cycle MUST be followed: write a failing test, make it pass, refactor.
- Unit test coverage MUST meet or exceed 80% for all business logic.
- Tests MUST be independent, deterministic, and free of shared mutable state.
- Integration tests MUST cover all cross-boundary interactions (database, external services, APIs).
- No feature is considered complete until all its tests pass in CI.

**Rationale**: Test-first development enforces clear requirement thinking, prevents regression, and
produces living documentation of intended system behaviour.

### III. User Experience Consistency

All user-facing interfaces MUST follow a single, unified design language.

- UI components MUST be drawn from the project's shared component library; bespoke one-off
  components require documented justification.
- Error messages MUST be human-readable, actionable, and consistent in tone and terminology.
- Navigation patterns and domain language MUST be consistent across all screens and workflows.
- Accessibility MUST meet WCAG 2.1 AA compliance for all user-facing surfaces.

**Rationale**: Inconsistency erodes user trust and increases training cost. A unified experience
signals product maturity and reduces support burden.

### IV. Performance Standards

System performance MUST meet defined baselines at all times.

- API responses MUST complete within 200ms at the 95th percentile under normal load.
- UI interactions MUST feel instantaneous (< 100ms perceived response for synchronous actions).
- Database queries MUST be profiled; any query exceeding 50ms MUST be reviewed and justified.
- Performance regressions that breach a defined baseline MUST block merge until resolved.

**Rationale**: Performance is a feature. Degraded performance directly impacts user retention and
operational cost; catching regressions early is far cheaper than fixing them in production.

### V. Clean Architecture

The codebase MUST maintain clear separation between domain logic, application orchestration, and
infrastructure concerns.

- Domain entities and business rules MUST NOT depend on infrastructure (databases, HTTP,
  frameworks).
- Dependencies MUST flow inward: Infrastructure → Application → Domain.
- Cross-feature access MUST go through defined public interfaces, never internal implementation
  details.
- The simplest design that satisfies the requirement MUST be preferred (YAGNI); complexity MUST
  be justified with documented rationale.

**Rationale**: A clean architecture keeps the domain model testable and portable, and prevents the
codebase from becoming a tightly coupled monolith that resists change.

## Security & Data Standards

All features that handle user data MUST comply with the following baseline requirements:

- Authentication and authorisation MUST be enforced at the application boundary; they MUST NOT be
  assumed or delegated to the caller.
- Sensitive data (credentials, personal information) MUST be encrypted at rest and in transit.
- Input validation MUST occur at the system boundary before any data is persisted or processed.
- Dependencies MUST be reviewed for known vulnerabilities before adoption; automated audits MUST
  run in CI.
- No secrets or credentials MAY be committed to source control under any circumstances.

## Development Workflow

The following workflow governs all feature work:

- All work MUST be done on a feature branch; direct commits to `main` are prohibited.
- Every branch MUST be linked to a spec document in `/specs/` before implementation begins.
- Pull requests MUST pass all CI checks (build, tests, linting) before entering review.
- At least one peer review approval is REQUIRED before merge.
- Commits MUST be atomic and carry a meaningful message describing the *why*, not just the *what*.

## Governance

This constitution supersedes all informal conventions and prior agreements. Any practice that
conflicts with a principle stated here MUST be brought into compliance.

**Amendment procedure**: Amendments require a documented proposal (updated constitution draft),
team consensus (majority approval), and a migration plan when existing code is affected.
Amendments MUST be versioned according to the following semantic versioning policy:

- MAJOR: Removal or backward-incompatible redefinition of an existing principle.
- MINOR: New principle added, or materially expanded guidance within an existing principle.
- PATCH: Clarifications, wording fixes, or non-semantic refinements.

**Compliance**: All pull requests MUST include a Constitution Check in the associated plan
document. Reviewers MUST verify compliance as part of the review process.

**Guidance**: For runtime development guidance, refer to `CLAUDE.md` at the repository root.

**Version**: 1.0.0 | **Ratified**: 2026-04-08 | **Last Amended**: 2026-04-08
