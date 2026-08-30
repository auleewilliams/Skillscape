# Tasks: Skillscape — Team Skills Visibility Application

**Input**: Design documents from `/specs/001-skillscape-app/`
**Prerequisites**: plan.md ✅ spec.md ✅ research.md ✅ data-model.md ✅ contracts/ui-routes.md ✅ quickstart.md ✅

**Tests**: Included — TDD is NON-NEGOTIABLE per the project constitution (Principle II). Write each test group FIRST, confirm tests FAIL (Red), then implement (Green), then refactor.

**Organization**: Tasks grouped by user story for independent implementation and delivery.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (US1–US4)
- Exact file paths included in all descriptions

---

## Phase 1: Setup

**Purpose**: Scaffold solution, configure shared build infrastructure and NuGet packages.

- [ ] T001 Create .NET 10 solution file: `dotnet new sln -n Skillscape` at repo root
- [ ] T002 Create source projects: `dotnet new classlib -n Skillscape.Core -o src/Skillscape.Core`, `dotnet new classlib -n Skillscape.Data -o src/Skillscape.Data`, `dotnet new blazorserver -n Skillscape.Web -o src/Skillscape.Web` and add all three to Skillscape.sln
- [ ] T003 Create test projects: `dotnet new xunit -n Skillscape.Core.Tests -o tests/Skillscape.Core.Tests`, `dotnet new xunit -n Skillscape.Data.Tests -o tests/Skillscape.Data.Tests`, `dotnet new xunit -n Skillscape.Web.Tests -o tests/Skillscape.Web.Tests` and add all three to Skillscape.sln
- [ ] T004 Add project references: Core.Tests → Core; Data → Core; Data.Tests → Data + Core; Web → Core + Data; Web.Tests → Web + Core + Data
- [ ] T005 Create `Directory.Packages.props` at solution root with `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>` and declare versions for: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Tools`, `MudBlazor`, `xunit`, `xunit.runner.visualstudio`, `bunit`, `Moq`, `coverlet.collector`
- [ ] T006 [P] Create `Directory.Build.props` at solution root: enable `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`, `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, `<LangVersion>latest</LangVersion>`
- [ ] T007 [P] Remove boilerplate from all generated projects (delete placeholder classes, sample pages, WeatherForecast, Counter components); confirm `dotnet build` passes clean

**Checkpoint**: `dotnet build` and `dotnet test` both pass with no errors

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Domain entities, enums, database context, migrations and app bootstrap — MUST be complete before any user story work begins.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T008 [P] Create `SkillCategory` enum in `src/Skillscape.Core/Enums/SkillCategory.cs` with values: `Technical = 1`, `DomainKnowledge = 2`; add XML doc comments
- [ ] T009 [P] Create `Proficiency` enum in `src/Skillscape.Core/Enums/Proficiency.cs` with ordered values: `Beginner = 1`, `Intermediate = 2`, `Expert = 3`; add XML doc comments
- [ ] T010 [P] Create `TeamMember` entity in `src/Skillscape.Core/Entities/TeamMember.cs`: Id (Guid), Name (string, required), Role (string?), Department (string?), IsArchived (bool, default false), CreatedAt (DateTimeOffset), UpdatedAt (DateTimeOffset), Skills (ICollection<TeamMemberSkill>); add XML doc comments
- [ ] T011 [P] Create `Skill` entity in `src/Skillscape.Core/Entities/Skill.cs`: Id (Guid), Name (string, normalised), DisplayName (string), Category (SkillCategory), BusinessApplication (string?), CreatedAt (DateTimeOffset), TeamMembers (ICollection<TeamMemberSkill>); add XML doc comments
- [ ] T012 Create `TeamMemberSkill` join entity in `src/Skillscape.Core/Entities/TeamMemberSkill.cs`: TeamMemberId (Guid, FK), SkillId (Guid, FK), Proficiency (Proficiency), LastUpdatedAt (DateTimeOffset), navigation properties; add XML doc comments
- [ ] T013 Create `AppDbContext` in `src/Skillscape.Data/Context/AppDbContext.cs`: configure entity tables, composite PK for TeamMemberSkill, EF Core global query filter `IsArchived == false` on TeamMember, all six indexes from data-model.md, cascade/restrict delete rules
- [ ] T014 Add initial EF Core migration: `dotnet ef migrations add InitialCreate --project src/Skillscape.Data --startup-project src/Skillscape.Web`; verify generated migration matches data-model.md schema
- [ ] T015 Configure `Program.cs` in `src/Skillscape.Web/Program.cs`: register `AppDbContext` with SQLite connection string from `appsettings.json`, call `db.Database.MigrateAsync()` on startup, register `MudBlazor` services, register application services (to be added per phase)
- [ ] T016 Create `appsettings.json` connection string in `src/Skillscape.Web/appsettings.json`: `"ConnectionStrings": { "DefaultConnection": "Data Source=skillscape.db" }`
- [ ] T017 Create `MainLayout.razor` in `src/Skillscape.Web/Components/Layout/MainLayout.razor`: MudBlazor `MudLayout` shell with `MudAppBar` (app title "Skillscape"), `MudNavMenu`, and `MudMainContent`; wire up `MudThemeProvider`, `MudDialogProvider`, `MudSnackbarProvider` in `App.razor`

**Checkpoint**: `dotnet run` starts the app; SQLite database is created; MudBlazor shell renders with no errors

---

## Phase 3: User Story 1 — Browse Team Skills Directory (Priority: P1) 🎯 MVP

**Goal**: Any visitor can open the app and see a searchable, filterable directory of all active team members and their skills.

**Independent Test**: Navigate to `/`; verify the directory renders active members with skills; verify search by name filters the list; verify category filter works; verify empty state shows when no members exist.

### Tests for User Story 1 ⚠️ Write FIRST — confirm RED before implementing

- [ ] T018 [P] [US1] Write unit tests for `TeamMemberService.GetActiveMembersAsync` (returns only non-archived members, ordered by name) in `tests/Skillscape.Core.Tests/Services/TeamMemberServiceTests.cs`
- [ ] T019 [P] [US1] Write unit tests for `TeamMemberService.SearchByNameAsync` (case-insensitive partial match, excludes archived) in `tests/Skillscape.Core.Tests/Services/TeamMemberServiceTests.cs`
- [ ] T020 [P] [US1] Write unit tests for `TeamMemberService.FilterByCategoryAsync` (returns members with at least one skill in given category) in `tests/Skillscape.Core.Tests/Services/TeamMemberServiceTests.cs`
- [ ] T021 [P] [US1] Write integration test confirming `AppDbContext` global query filter excludes archived members in `tests/Skillscape.Data.Tests/Context/AppDbContextTests.cs`
- [ ] T022 [P] [US1] Write bUnit component test for `TeamDirectory.razor`: renders member cards, search input filters list, empty state shown when no results in `tests/Skillscape.Web.Tests/Pages/TeamDirectoryTests.cs`

### Implementation for User Story 1

- [ ] T023 [US1] Implement `TeamMemberService` in `src/Skillscape.Core/Services/TeamMemberService.cs`: `GetActiveMembersAsync`, `SearchByNameAsync`, `FilterByCategoryAsync` with direct `AppDbContext` injection; add XML doc comments
- [ ] T024 [US1] Register `TeamMemberService` as scoped in `src/Skillscape.Web/Program.cs`
- [ ] T025 [P] [US1] Create `SkillChip` component in `src/Skillscape.Web/Components/Skills/SkillChip.razor`: renders a `MudChip` showing skill DisplayName, proficiency badge, and category colour
- [ ] T026 [P] [US1] Create `CategoryFilter` component in `src/Skillscape.Web/Components/Directory/CategoryFilter.razor`: `MudSelect` or `MudChipSet` for Technical / Domain Knowledge filter; emits `EventCallback<SkillCategory?>`
- [ ] T027 [P] [US1] Create `MemberCard` component in `src/Skillscape.Web/Components/Directory/MemberCard.razor`: displays member name, role, department, and a row of `SkillChip` components; clicking navigates to `/member/{id}`
- [ ] T028 [US1] Create `TeamDirectory.razor` page in `src/Skillscape.Web/Pages/TeamDirectory.razor` (`@page "/"`): `MudTextField` for name search, `CategoryFilter`, `MudGrid` of `MemberCard` items, empty state with "Add the first team member" prompt, error state with retry, "Add Team Member" `MudButton` navigating to `/member/new`
- [ ] T029 [US1] Wire real-time search and category filter in `TeamDirectory.razor`: debounce name input (300ms), call `TeamMemberService` on change, update rendered member list reactively

**Checkpoint**: User Story 1 fully functional — directory renders, search and filter work, empty state shows correctly

---

## Phase 4: User Story 2 — Self-Manage My Skills Profile (Priority: P2)

**Goal**: Any user can register a new team member and manage that member's skills, including free-text skill entry with autocomplete suggestions.

**Independent Test**: Navigate to `/member/new`; add a member with skills; confirm they appear in the directory. Navigate to `/member/{id}/edit`; add, update proficiency of, and remove a skill; confirm changes appear on profile and in directory.

### Tests for User Story 2 ⚠️ Write FIRST — confirm RED before implementing

- [ ] T030 [P] [US2] Write unit tests for `TeamMemberService.AddMemberAsync` (trims name, rejects blank name, creates member) in `tests/Skillscape.Core.Tests/Services/TeamMemberServiceTests.cs`
- [ ] T031 [P] [US2] Write unit tests for `SkillService.FindOrCreateAsync` (normalises input, returns existing skill on case-insensitive match, creates new skill for novel name) in `tests/Skillscape.Core.Tests/Services/SkillServiceTests.cs`
- [ ] T032 [P] [US2] Write unit tests for `TeamMemberService.AddSkillAsync` (assigns skill with proficiency, prevents duplicate) and `RemoveSkillAsync` in `tests/Skillscape.Core.Tests/Services/TeamMemberServiceTests.cs`
- [ ] T033 [P] [US2] Write unit tests for `SkillService.GetAutocompleteSuggestionsAsync` (returns matching skills ordered by DisplayName prefix, excludes skills already on the member) in `tests/Skillscape.Core.Tests/Services/SkillServiceTests.cs`
- [ ] T034 [P] [US2] Write integration test confirming skill normalisation deduplication persists correctly in `tests/Skillscape.Data.Tests/Services/SkillServiceIntegrationTests.cs`
- [ ] T035 [P] [US2] Write bUnit component test for `AddMember.razor`: name validation, skill chip added on autocomplete selection, form submits in `tests/Skillscape.Web.Tests/Pages/AddMemberTests.cs`
- [ ] T036 [P] [US2] Write bUnit component test for `EditProfile.razor`: pre-populated skills shown, chip removal works, save navigates to profile in `tests/Skillscape.Web.Tests/Pages/EditProfileTests.cs`

### Implementation for User Story 2

- [ ] T037 [US2] Implement `SkillService` in `src/Skillscape.Core/Services/SkillService.cs`: `FindOrCreateAsync` (normalise → lookup → create), `GetAutocompleteSuggestionsAsync` (prefix search on `Skill.Name`); add XML doc comments
- [ ] T038 [US2] Add `AddMemberAsync`, `AddSkillAsync`, `RemoveSkillAsync`, `UpdateSkillProficiencyAsync` to `src/Skillscape.Core/Services/TeamMemberService.cs`; add XML doc comments
- [ ] T039 [US2] Register `SkillService` as scoped in `src/Skillscape.Web/Program.cs`
- [ ] T040 [P] [US2] Create `SkillAutocomplete` component in `src/Skillscape.Web/Components/Skills/SkillAutocomplete.razor`: wraps `MudAutocomplete<string>` with async `SearchFunc` calling `SkillService.GetAutocompleteSuggestionsAsync`; emits `EventCallback<string>` on selection; supports free-text entry for novel skill names
- [ ] T041 [P] [US2] Create `ProficiencySelect` component in `src/Skillscape.Web/Components/Skills/ProficiencySelect.razor`: `MudSelect<Proficiency>` bound to a `Proficiency` value; emits `EventCallback<Proficiency>` on change
- [ ] T042 [P] [US2] Create `SkillEntryRow` component in `src/Skillscape.Web/Components/Skills/SkillEntryRow.razor`: combines `SkillAutocomplete` + `ProficiencySelect` + category select + optional BusinessApplication field + remove button; used in both Add and Edit forms
- [ ] T043 [US2] Create `AddMember.razor` page in `src/Skillscape.Web/Pages/AddMember.razor` (`@page "/member/new"`): name field with validation, one or more `SkillEntryRow` components, "Add another skill" button, Save (calls `TeamMemberService.AddMemberAsync` + `AddSkillAsync` for each row) with redirect to `/member/{id}`, Cancel navigates to `/`
- [ ] T044 [US2] Create `EditProfile.razor` page in `src/Skillscape.Web/Pages/EditProfile.razor` (`@page "/member/{Id:guid}/edit"`): pre-loads member + skills, renders existing skills as `SkillEntryRow` (removable), allows adding new rows, Save persists all changes, Cancel returns to `/member/{Id}`, not-found guard redirects to `/`
- [ ] T045 [US2] Implement duplicate skill prevention in `AddMember.razor` and `EditProfile.razor`: surface a `MudSnackbar` warning ("You've already added this skill") when `AddSkillAsync` returns a duplicate conflict

**Checkpoint**: User Stories 1 and 2 both independently functional — member can be created and skills managed end-to-end

---

## Phase 5: User Story 3 — View Individual Team Member Profile (Priority: P3)

**Goal**: Any visitor can click a team member in the directory and see their complete skill profile with skills grouped by category.

**Independent Test**: Navigate to `/member/{id}`; verify all skills are displayed with proficiency and category; verify Technical and Domain Knowledge sections are visually distinct; verify not-found state for unknown id; verify empty skills state shows with prompt to edit.

### Tests for User Story 3 ⚠️ Write FIRST — confirm RED before implementing

- [ ] T046 [P] [US3] Write unit tests for `TeamMemberService.GetMemberProfileAsync` (returns member + grouped skills, returns null for unknown id, excludes archived) in `tests/Skillscape.Core.Tests/Services/TeamMemberServiceTests.cs`
- [ ] T047 [P] [US3] Write bUnit component test for `MemberProfile.razor`: grouped skills render, not-found state renders for null model, empty-skills state shown with edit link in `tests/Skillscape.Web.Tests/Pages/MemberProfileTests.cs`

### Implementation for User Story 3

- [ ] T048 [US3] Add `GetMemberProfileAsync(Guid id)` to `src/Skillscape.Core/Services/TeamMemberService.cs`: returns `TeamMember` with eagerly-loaded `TeamMemberSkill` and `Skill`; returns `null` for unknown or archived id; add XML doc comments
- [ ] T049 [P] [US3] Create `SkillCategoryGroup` component in `src/Skillscape.Web/Components/Profile/SkillCategoryGroup.razor`: renders a headed section (Technical or Domain Knowledge) containing a `MudGrid` of `SkillChip` components
- [ ] T050 [US3] Create `MemberProfile.razor` page in `src/Skillscape.Web/Pages/MemberProfile.razor` (`@page "/member/{Id:guid}"`): loads member profile, renders name/role/department header, two `SkillCategoryGroup` sections (hidden if empty category), empty-skills state with link to `/member/{Id}/edit`, not-found state with link to `/`, "Edit Profile" button, back link to `/`

**Checkpoint**: User Stories 1, 2, and 3 all independently functional

---

## Phase 6: User Story 4 — Find Who Knows a Specific Skill (Priority: P4)

**Goal**: A visitor can search by skill name from the team directory and see all members who have that skill, ordered by proficiency (Expert first).

**Independent Test**: Enter a skill name in the search field on `/`; verify results show only members with that skill; verify results are ordered Expert → Intermediate → Beginner; verify "no results" message shown for unknown skill.

### Tests for User Story 4 ⚠️ Write FIRST — confirm RED before implementing

- [ ] T051 [P] [US4] Write unit tests for `SkillService.FindMembersBySkillAsync` (returns members with matching skill, ordered by Proficiency descending, excludes archived) in `tests/Skillscape.Core.Tests/Services/SkillServiceTests.cs`
- [ ] T052 [P] [US4] Write integration test confirming proficiency ordering in SQLite query in `tests/Skillscape.Data.Tests/Services/SkillServiceIntegrationTests.cs`
- [ ] T053 [P] [US4] Write bUnit component test for skill-search mode in `TeamDirectory.razor`: results ordered correctly, no-results message shown in `tests/Skillscape.Web.Tests/Pages/TeamDirectoryTests.cs`

### Implementation for User Story 4

- [ ] T054 [US4] Add `FindMembersBySkillAsync(string skillName)` to `src/Skillscape.Core/Services/SkillService.cs`: normalise input, query `TeamMemberSkill` joined to `Skill` by normalised name, order by `Proficiency` descending, exclude archived members; add XML doc comments
- [ ] T055 [US4] Add skill-search mode to `TeamDirectory.razor`: when search input matches an exact skill name (detected by selecting from autocomplete suggestions), switch to skill-search results showing member cards with proficiency label, ordered Expert → Intermediate → Beginner; "no results" message when skill has no members

**Checkpoint**: All four user stories independently functional

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Archive functionality, WCAG compliance verification, performance profiling, and final code quality pass.

- [ ] T056 [P] Add `ArchivedAt (DateTimeOffset?)` field to `TeamMember` entity in `src/Skillscape.Core/Entities/TeamMember.cs`; add EF Core migration `AddArchivedAt`
- [ ] T057 [P] Write unit tests for `TeamMemberService.ArchiveMemberAsync` in `tests/Skillscape.Core.Tests/Services/TeamMemberServiceTests.cs`
- [ ] T058 Implement `ArchiveMemberAsync(Guid id)` in `src/Skillscape.Core/Services/TeamMemberService.cs`: sets `IsArchived = true`, `ArchivedAt = DateTimeOffset.UtcNow`, updates `UpdatedAt`
- [ ] T059 [P] Add "Archive Member" action to `EditProfile.razor` with `MudDialog` confirmation prompt; on confirm calls `ArchiveMemberAsync` and redirects to `/`
- [ ] T060 [P] Apply XML documentation comments to all remaining public types and methods across `src/Skillscape.Core/` and `src/Skillscape.Data/` not yet documented
- [ ] T061 [P] Review all Blazor components for WCAG 2.1 AA compliance: verify `aria-label` attributes on icon buttons, keyboard navigation on `MudAutocomplete` and `MudSelect`, colour contrast on `SkillChip` proficiency badges
- [ ] T062 [P] Profile SQLite query performance: seed 500 `TeamMember` records with 20 skills each; run directory load, name search, category filter, and skill search queries; confirm all complete under 50ms
- [ ] T063 [P] Add development data seeder: in `src/Skillscape.Web/Program.cs` under `if (app.Environment.IsDevelopment())`, seed 10 sample members with varied skills so the app starts with visible data locally
- [ ] T064 Follow `specs/001-skillscape-app/quickstart.md` end-to-end in a fresh environment; fix any discrepancies found

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 — **BLOCKS all user stories**
- **US1 (Phase 3)**: Depends on Phase 2 — no dependency on other user stories
- **US2 (Phase 4)**: Depends on Phase 2 — no dependency on US1 (independently testable; integrates with directory naturally)
- **US3 (Phase 5)**: Depends on Phase 2 — benefits from US2 having created profile data but independently testable with seeded data
- **US4 (Phase 6)**: Depends on Phase 2 — uses `SkillService` introduced in US2; start after T037
- **Polish (Phase 7)**: Depends on all desired user stories being complete

### Within Each User Story

1. Write tests FIRST — verify they FAIL (Red)
2. Models / entities before services
3. Services before components
4. Components before pages
5. Story complete and all tests Green before moving to next priority

### Parallel Opportunities

- T008, T009, T010, T011 — all entity/enum definitions: run in parallel
- T018–T022 — all US1 test writing: run in parallel
- T025, T026, T027 — all US1 leaf components (no inter-dependency): run in parallel
- T030–T036 — all US2 test writing: run in parallel
- T040, T041, T042 — all US2 sub-components: run in parallel
- T046, T047 — US3 test writing: run in parallel
- T051, T052, T053 — US4 test writing: run in parallel
- T057, T060, T061, T062, T063 — all Polish tasks with no blocking dependency: run in parallel

---

## Parallel Example: User Story 1

```
# Write all US1 tests together (all in different files):
T018 — TeamMemberServiceTests (GetActiveMembersAsync)
T019 — TeamMemberServiceTests (SearchByNameAsync)
T020 — TeamMemberServiceTests (FilterByCategoryAsync)
T021 — AppDbContextTests (global query filter)
T022 — TeamDirectoryTests (bUnit component)

# After T023 (service impl) — build leaf components in parallel:
T025 — SkillChip.razor
T026 — CategoryFilter.razor
T027 — MemberCard.razor
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1 (directory browse)
4. **STOP and VALIDATE**: demo the searchable directory with seeded data
5. Deploy / share for feedback before building edit flows

### Incremental Delivery

1. Phase 1 + 2 → foundation ready
2. + Phase 3 (US1) → **browsable directory MVP** ✅ demo
3. + Phase 4 (US2) → members can self-register and manage skills ✅ demo
4. + Phase 5 (US3) → full profile view ✅ demo
5. + Phase 6 (US4) → skill-first expert finder ✅ demo
6. + Phase 7 → polished, WCAG compliant, production-ready ✅

### Parallel Team Strategy

With two or more developers — after Phase 2 completes:
- **Developer A**: US1 (directory) → US4 (skill search, extends directory)
- **Developer B**: US2 (self-manage) → US3 (profile view)

Stories integrate cleanly since US3 (profile) is a read-only companion to US2's data.

---

## Notes

- **[P]** tasks operate on different files with no shared in-progress dependencies
- **[Story]** label maps every task to its user story for traceability
- Constitution Principle II (TDD) is non-negotiable — every implementation phase is preceded by its test group
- Each user story phase ends with a named checkpoint — stop and validate before proceeding
- Commit after each logical group (e.g., after all US1 tests pass green, after each component)
- Archive functionality (Phase 7) deliberately deferred — it is not required by any of the four user stories and can be delivered as a follow-on increment

---

## Summary

| Phase | Story | Tasks | Parallel Tasks |
|-------|-------|-------|---------------|
| 1 — Setup | — | T001–T007 | T006, T007 |
| 2 — Foundational | — | T008–T017 | T008–T011 |
| 3 — US1 Directory | P1 | T018–T029 | T018–T022, T025–T027 |
| 4 — US2 Self-Manage | P2 | T030–T045 | T030–T036, T040–T042 |
| 5 — US3 Profile | P3 | T046–T050 | T046–T047, T049 |
| 6 — US4 Skill Search | P4 | T051–T055 | T051–T053 |
| 7 — Polish | — | T056–T064 | T057, T060–T063 |
| **Total** | | **64 tasks** | **28 parallelisable** |
