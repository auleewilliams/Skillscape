# Data Model

**Project:** Skillscape  
**Stack:** ASP.NET Core 10, Blazor Server, Entity Framework Core 10 + SQLite  
**Last updated:** 2025-07-11

---

## Overview

The data model consists of three entities: `TeamMember`, `Skill`, and `TeamMemberSkill`. `Skill` is a shared catalog built from free-text entry; `TeamMemberSkill` is the enriched join that binds a team member to a skill with a proficiency level.

---

## Enumerations

### SkillCategory

| Value | Description |
|---|---|
| `Technical` | Engineering, programming, tooling, infrastructure, etc. |
| `DomainKnowledge` | Business process or application knowledge (e.g. SAP, Salesforce) |

### Proficiency

Ordered from lowest to highest for search ranking purposes.

| Value | Ordinal | Description |
|---|---|---|
| `Beginner` | 1 | Awareness or introductory experience |
| `Intermediate` | 2 | Practical working knowledge |
| `Expert` | 3 | Deep expertise; go-to person for this skill |

---

## Entities

### TeamMember

Represents a person on the team. Up to ~500 records are expected. Members are never hard-deleted; they are archived to retain historical skill data.

| Field | Type | Constraints / Notes |
|---|---|---|
| `Id` | `Guid` | PK, generated on create |
| `Name` | `string` | Required, max 200 chars |
| `Role` | `string?` | Optional, max 200 chars — disambiguates people with the same name |
| `Department` | `string?` | Optional, max 200 chars — disambiguates people with the same name |
| `IsArchived` | `bool` | Default `false`; soft-delete flag |
| `CreatedAt` | `DateTimeOffset` | Set on insert, never modified |
| `UpdatedAt` | `DateTimeOffset` | Updated on every change |

**Business rules**

- `Name` must be non-empty and non-whitespace after trimming.
- `Role` and `Department` are informational only; uniqueness is not enforced — the application relies on the combination of name, role, and department to distinguish people visually.
- Archived members are excluded from the directory and from skill-search results by default. Their `TeamMemberSkill` records are retained.
- Archiving does not cascade-delete skills; it only sets `IsArchived = true`.

---

### Skill

A named capability or knowledge area. The catalog is global and shared across all team members. Skills are created implicitly when a team member enters a skill name that does not already exist.

| Field | Type | Constraints / Notes |
|---|---|---|
| `Id` | `Guid` | PK, generated on create |
| `Name` | `string` | Required, max 200 chars — normalised (trimmed, lower-cased) for deduplication and matching |
| `DisplayName` | `string` | Required, max 200 chars — original casing as first entered; shown in the UI |
| `Category` | `SkillCategory` | Required enum |
| `BusinessApplication` | `string?` | Optional, max 200 chars — relevant only when `Category = DomainKnowledge` (e.g. `"SAP"`, `"Salesforce"`) |
| `CreatedAt` | `DateTimeOffset` | Set on insert, never modified |

**Business rules**

- `Name` is stored in normalised form (trimmed, lower-case). All skill-name lookups use this field to prevent duplicate entries differing only by case or surrounding whitespace (FR-009).
- `DisplayName` preserves the casing from the first entry and is the value shown throughout the UI.
- `BusinessApplication` has no defined meaning when `Category = Technical`; it should be left null in that case.
- Skills are never hard-deleted once any team member has used them (`TeamMemberSkill` references are restricted on delete).

---

### TeamMemberSkill

The assignment of a skill to a team member, enriched with a proficiency level. This is the primary join entity between `TeamMember` and `Skill`.

| Field | Type | Constraints / Notes |
|---|---|---|
| `TeamMemberId` | `Guid` | PK (composite), FK → `TeamMember.Id`, cascade delete |
| `SkillId` | `Guid` | PK (composite), FK → `Skill.Id`, restrict delete |
| `Proficiency` | `Proficiency` | Required enum |
| `LastUpdatedAt` | `DateTimeOffset` | Updated whenever the row changes |

**Business rules**

- The composite primary key `(TeamMemberId, SkillId)` enforces that a team member cannot be assigned the same skill twice (FR-008).
- `Proficiency` must be one of the three defined enum values; it may be updated by the team member at any time.
- Deleting a `TeamMember` cascades to all their `TeamMemberSkill` rows.
- Deleting a `Skill` is restricted while any `TeamMemberSkill` row references it.

---

## Relationships

```
TeamMember  1 ──< TeamMemberSkill >── 1  Skill
```

| Relationship | Cardinality | Notes |
|---|---|---|
| `TeamMember` → `TeamMemberSkill` | One-to-many | A member can have zero or more skill assignments |
| `Skill` → `TeamMemberSkill` | One-to-many | A skill can be assigned to zero or more members |
| `TeamMember` ↔ `Skill` | Many-to-many | Realised through `TeamMemberSkill` with proficiency enrichment |

---

## Validation Rules

| Rule | Source | Detail |
|---|---|---|
| Duplicate skill per member | FR-008 | The composite PK `(TeamMemberId, SkillId)` prevents inserting the same skill twice for a member. The application should surface a meaningful error rather than expose the constraint violation directly. |
| Case-insensitive skill matching | FR-009 | Before creating a new `Skill`, the application normalises the input (trim, lower-case) and queries `Skill.Name`. If a match exists, the existing skill is reused; no new row is inserted. |
| Search ordering by proficiency | FR-010 | Skill-search results must be ordered by `Proficiency` descending (`Expert` first, then `Intermediate`, then `Beginner`). Use the `Proficiency` ordinal value for the `ORDER BY` clause. |
| `TeamMember.Name` non-empty | — | Must be non-empty and non-whitespace after trimming. Enforced at the application layer before EF Core persists the record. |
| `Skill.Name` / `Skill.DisplayName` non-empty | — | Both fields must be non-empty and non-whitespace after trimming. `DisplayName` is set from the raw user input; `Name` is its normalised form. |
| Archived member visibility | — | Queries for the member directory and skill-search results must filter `IsArchived = false` unless the caller explicitly opts in to see archived members. |

---

## Indexes

| Index | Table | Column(s) | Purpose |
|---|---|---|---|
| `IX_TeamMember_Name` | `TeamMember` | `Name` | Name search and sorting in the directory |
| `IX_TeamMember_IsArchived` | `TeamMember` | `IsArchived` | Efficient filtering of active vs. archived members |
| `IX_Skill_Name` | `Skill` | `Name` (normalised) | Autocomplete lookups and deduplication checks (FR-009) |
| `IX_Skill_Category` | `Skill` | `Category` | Filtering skills by category |
| `IX_TeamMemberSkill_SkillId` | `TeamMemberSkill` | `SkillId` | "Who knows this skill?" queries — traversal from skill to members |
| `IX_TeamMemberSkill_Proficiency` | `TeamMemberSkill` | `Proficiency` | Proficiency-ordered search results (FR-010) |

> The composite PK `(TeamMemberId, SkillId)` on `TeamMemberSkill` implicitly creates a clustered index covering `TeamMemberId`-first lookups (all skills for a given member).

---

## State Transitions

### TeamMember Lifecycle

```
          Archive
 Active ──────────> Archived
(default)           (soft-deleted)
```

| State | `IsArchived` | Visible in directory | Included in skill search |
|---|---|---|---|
| Active | `false` | Yes | Yes |
| Archived | `true` | No | No |

**Transition rules**

- All new `TeamMember` records start in the **Active** state (`IsArchived = false`).
- Archiving sets `IsArchived = true` and updates `UpdatedAt`. No data is deleted.
- There is no restore/unarchive workflow defined in the current spec; the flag is a bool to allow for it if needed.
- Archived members' `TeamMemberSkill` rows are retained to preserve historical data integrity.
