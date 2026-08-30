# Feature Specification: Skillscape — Team Skills Visibility Application

**Feature Branch**: `001-skillscape-app`  
**Created**: 2026-04-08  
**Status**: Clarified  
**Input**: User description: "Build an application for showing what skills are in your team. Weather it be technical skills or what domain knowledge a team member has of applications in the business"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Browse Team Skills Directory (Priority: P1)

A manager or team member opens the application and immediately sees a searchable directory of all team members alongside their skills. They can browse the full team at a glance, filter by skill category (e.g., technical, domain knowledge, business application), and quickly identify who knows what.

**Why this priority**: This is the core value of the application. Without the ability to see the team's skills in one place, no other feature matters. It delivers immediate value as a read-only directory even before any other features are built.

**Independent Test**: Can be fully tested by navigating to the application and verifying a list of team members with their associated skills is displayed, filterable, and searchable — delivering the skill-discovery value on its own.

**Acceptance Scenarios**:

1. **Given** I open the application, **When** I view the team directory, **Then** I see a list of all team members each showing their name and associated skills grouped by category
2. **Given** I am viewing the team directory, **When** I search for a skill name (e.g., "Excel" or "Azure"), **Then** only team members with that skill are shown
3. **Given** I am viewing the team directory, **When** I filter by category (e.g., "Technical" or "Domain Knowledge"), **Then** only skills and members matching that category are displayed
4. **Given** no team members have been added yet, **When** I open the application, **Then** I see an empty state message guiding me to add team members

---

### User Story 2 - Self-Manage My Skills Profile (Priority: P2)

A team member adds themselves to the application and manages their own skills profile — no login or admin approval required. They can add skills using a free-text field with autocomplete suggestions drawn from skills already in the system, assign a proficiency level and category to each, and remove skills that are no longer relevant. Their updates are immediately visible to the rest of the team in the directory.

**Why this priority**: Self-management keeps the skill data accurate and up to date without placing the burden on anyone else. Open self-registration (no authentication) removes friction and mirrors the low-barrier nature of the current Excel spreadsheet it replaces.

**Independent Test**: Can be fully tested by a new team member adding themselves with a set of skills, then confirming their profile and skills appear immediately in the team directory.

**Acceptance Scenarios**:

1. **Given** I am a new team member, **When** I add myself with a name and at least one skill, **Then** my profile appears in the team directory with my skills displayed
2. **Given** I am on my profile, **When** I type a skill name, **Then** I see autocomplete suggestions from skills already used by other team members
3. **Given** I have an existing skill on my profile, **When** I update its proficiency level, **Then** the updated level is reflected in the directory
4. **Given** I have an existing skill on my profile, **When** I remove it, **Then** it no longer appears on my profile or in the directory
5. **Given** I try to add a skill I have already added, **When** I submit, **Then** the system prevents the duplicate and informs me

---

### User Story 3 - View Individual Team Member Profile (Priority: P3)

A user selects a specific team member and sees their full profile: all skills listed with proficiency levels, the applications or business domains they are knowledgeable in, and any notes about their expertise.

**Why this priority**: Provides deeper context beyond the directory view, helping teams make informed decisions when assigning work or identifying mentors.

**Independent Test**: Can be fully tested by selecting a team member from the directory and verifying their complete skill profile page displays correctly with all attributed skills and details.

**Acceptance Scenarios**:

1. **Given** I am viewing the team directory, **When** I select a team member, **Then** I see their full profile including all skills, proficiency levels, and categories
2. **Given** a team member has both technical skills and domain knowledge skills, **When** I view their profile, **Then** both categories are clearly separated and labelled
3. **Given** a team member has no skills recorded yet, **When** I view their profile, **Then** I see an empty state indicating no skills have been added

---

### User Story 4 - Find Who Knows a Specific Skill (Priority: P4)

A manager needs to find out who on the team has knowledge of a particular application or technical skill. They can search or browse by skill name and get a list of team members who have that skill, along with their proficiency levels.

**Why this priority**: Enables the "find an expert" use case — a key business need when assigning tasks or planning projects.

**Independent Test**: Can be fully tested by searching for a specific skill and confirming the returned list of team members matches those who have that skill assigned.

**Acceptance Scenarios**:

1. **Given** multiple team members share a skill, **When** I search for that skill, **Then** all matching team members are listed with their respective proficiency levels
2. **Given** no team member has a searched skill, **When** I search for it, **Then** I see a clear "no results" message
3. **Given** I search for a skill, **When** results are returned, **Then** they are ordered by proficiency level (highest first)

---

### Edge Cases

- What happens when two team members have the same name? Each must be distinguishable (e.g., by role or department).
- What happens when a skill name is entered with inconsistent casing or spacing (e.g., "JavaScript" vs "javascript")? The system should treat them as the same skill.
- What happens when a team member is removed? Their profile is archived — hidden from the active directory but their data is retained. Archived members do not appear in search or filter results.
- How does the system handle a very large team (e.g., 500+ members)? The directory must remain navigable and performant.
- What happens if a user attempts to add a duplicate skill to their own profile? The system should prevent duplicates and inform the user.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST display a directory of all active team members and their associated skills
- **FR-002**: System MUST allow any user to create a new team member profile with a name and skills — no authentication or admin approval required
- **FR-003**: System MUST allow each team member to add, edit, and remove skills on their own profile
- **FR-004**: System MUST support categorisation of skills as either "Technical" or "Domain Knowledge"
- **FR-005**: System MUST allow a proficiency level (Beginner, Intermediate, Expert) to be assigned to each skill
- **FR-006**: System MUST allow users to search the directory by skill name or team member name
- **FR-007**: System MUST allow users to filter the directory by skill category
- **FR-008**: System MUST display an individual profile page for each team member showing all their skills with proficiency and category
- **FR-009**: System MUST prevent a team member from adding a duplicate skill to their own profile
- **FR-010**: System MUST handle skill name matching case-insensitively to avoid duplicate skill entries across the system
- **FR-011**: System MUST provide autocomplete suggestions when entering a skill name, drawn from skills already used by other team members
- **FR-012**: System MUST support a skill search that returns all team members who hold that skill, ordered by proficiency level
- **FR-013**: System MUST allow skills to be associated with business applications (e.g., "SAP", "Salesforce") as a sub-type of Domain Knowledge
- **FR-014**: System MUST support archiving a team member — hiding them from the active directory while retaining their data
- **FR-015**: Skills are self-declared only; there is no endorsement or peer-verification mechanism

### Key Entities

- **Team Member**: Represents a person on the team. Has a unique identity, a display name, an optional role or department, and a collection of skills.
- **Skill**: Represents a capability or knowledge area. Has a name, a category (Technical or Domain Knowledge), and an optional sub-type (e.g., Business Application name).
- **Team Member Skill**: The relationship between a team member and a skill. Includes the assigned proficiency level (Beginner, Intermediate, Expert) and the date it was last updated.
- **Skill Category**: A top-level grouping: Technical (programming languages, tools, methodologies) or Domain Knowledge (business processes, application knowledge, industry expertise).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A user can find all team members who possess a specific skill in under 30 seconds from opening the application
- **SC-002**: A team member can update their own skill profile in under 2 minutes
- **SC-003**: 90% of users can successfully locate a team member's full skill profile on their first attempt without guidance
- **SC-004**: The directory remains fully usable with a team of up to 500 members without noticeable performance degradation
- **SC-005**: Managers report improved confidence in skill-based task assignment after using the application for 30 days
- **SC-006**: All team skills across the organisation can be viewed in a single place, eliminating the need for manual spreadsheet-based skill tracking

## Assumptions

- The application is intended for use within a single organisation or team — multi-tenancy (supporting multiple separate organisations) is out of scope for v1
- No authentication or login is required — this application replaces an Excel spreadsheet and intentionally has no access barrier for internal use
- There is no admin role; any user can add a new team member profile and manage their own skills
- Team members are identified by their name; the system must handle duplicate names (e.g., by showing role or department to distinguish them)
- Archived team members are hidden from the active directory but their data is retained (not permanently deleted)
- Skills are entirely self-declared; there is no peer endorsement or verification mechanism
- Skill entry uses a hybrid model — free-text input with autocomplete suggestions drawn from skills already in the system
- Mobile support is out of scope for v1; the application targets desktop/laptop browser users
- A predefined list of skill categories (Technical, Domain Knowledge) is sufficient; custom categories are out of scope for v1
- The application will be used internally and does not need to be publicly accessible
- Integration with HR systems or identity directories is out of scope for v1
