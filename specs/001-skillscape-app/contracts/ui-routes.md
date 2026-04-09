# UI Route Contracts

**Application:** Skillscape  
**Stack:** ASP.NET Core 10, Blazor Server, MudBlazor, EF Core + SQLite  
**Auth:** None (internal team tool)

---

## Navigation Structure

```
/ (Team Directory)
├── → /member/new        (Add Team Member)
└── → /member/{id}       (Team Member Profile)
       └── → /member/{id}/edit  (Edit Profile)
```

**Breadcrumbs / Back Links**

| Current Page       | Breadcrumb / Back Link         |
|--------------------|-------------------------------|
| Team Directory     | _(root — no back link)_        |
| Add Team Member    | ← Team Directory               |
| Team Member Profile| ← Team Directory               |
| Edit Profile       | ← Team Member Profile          |

---

## Routes

### `GET /`

**Page:** Team Directory

**Purpose:**  
The landing page. Shows all active team members and their associated skills. Users can search by name and filter by skill to locate team members with specific capabilities.

**Data Required:**
- All active `TeamMember` records
- Associated `Skill` records per member (used for filter chips and inline display)
- Distinct skill list (for populating filter options)

**Key Interactions:**
- Free-text search field — filters the member list by name in real time
- Skill filter — single or multi-select; narrows list to members who have all selected skills
- Click a team member card/row — navigates to `/member/{id}`
- "Add Team Member" button — navigates to `/member/new`

**Navigation:**
- → `/member/{id}` (click any member)
- → `/member/new` (Add Team Member button)

**Empty / Error States:**
| Condition | Display |
|---|---|
| No team members exist yet | Friendly empty state with prompt to add the first member |
| Search / filter returns no results | "No members match your search" message; retain current filter values |
| Data load error | Inline error alert with a retry action |

---

### `GET /member/new`

**Page:** Add Team Member

**Purpose:**  
Register a new team member. Captures name and an initial set of skills. On successful submission the user is redirected to the new member's profile page.

**Data Required:**
- Existing skill names (for autocomplete / tag suggestions when adding skills)

**Key Interactions:**
- Name field (required, text input)
- Skills input — type-ahead autocomplete; accepts existing skills or free-text to create new ones; displayed as removable chips
- "Save" button — validates and submits the form
- "Cancel" / breadcrumb back link — returns to Team Directory without saving

**Navigation:**
- On success → `/member/{id}` (newly created member)
- On cancel → `/`

**Empty / Error States:**
| Condition | Display |
|---|---|
| Name left blank | Inline validation error on the name field |
| Duplicate name | Inline warning: "A member with this name already exists" (non-blocking — allow save) |
| Save fails (server error) | Error alert above the form; form data preserved |

---

### `GET /member/{id}`

**Page:** Team Member Profile

**Purpose:**  
Read-only view of a single team member's full skill profile.

**Route Parameter:**
- `{id}` — integer primary key of the `TeamMember` record

**Data Required:**
- `TeamMember` record matching `id`
- All associated `Skill` records for that member

**Key Interactions:**
- "Edit Profile" button — navigates to `/member/{id}/edit`
- Skill chips / tags are display-only
- Back link — returns to Team Directory

**Navigation:**
- → `/member/{id}/edit` (Edit Profile button)
- → `/` (back link / breadcrumb)

**Empty / Error States:**
| Condition | Display |
|---|---|
| `id` not found | "Member not found" message with a link back to Team Directory |
| Member has no skills | Empty skills section with a prompt linking to the edit page |
| Data load error | Inline error alert with a retry action |

---

### `GET /member/{id}/edit`

**Page:** Edit Profile

**Purpose:**  
Edit an existing team member's profile — add new skills, remove existing skills, or update skill details.

**Route Parameter:**
- `{id}` — integer primary key of the `TeamMember` record

**Data Required:**
- `TeamMember` record matching `id` (pre-populates the form)
- All associated `Skill` records for that member (pre-populates the skill list)
- Existing skill names across all members (for autocomplete suggestions)

**Key Interactions:**
- Name field — editable, pre-filled with current name
- Skills input — type-ahead autocomplete; existing skills shown as removable chips; new skills can be added
- Remove chip (×) — removes a skill from the member's profile
- "Save" button — validates and persists changes
- "Cancel" / breadcrumb back link — discards changes and returns to profile view

**Navigation:**
- On success → `/member/{id}` (same member's profile)
- On cancel → `/member/{id}`

**Empty / Error States:**
| Condition | Display |
|---|---|
| `id` not found | "Member not found" message with a link back to Team Directory |
| Name cleared / blank | Inline validation error on the name field |
| Save fails (server error) | Error alert above the form; unsaved edits preserved |
| Concurrent edit conflict | Error alert advising the user to reload and re-apply changes |
