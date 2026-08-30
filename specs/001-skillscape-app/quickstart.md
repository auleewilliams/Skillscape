# Skillscape — Developer Quickstart

Get the project running locally in a few minutes.

---

## Prerequisites

| Tool | Version | Notes |
|------|---------|-------|
| [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) | 10.x | `dotnet --version` to verify |
| [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) | latest | `dotnet tool install --global dotnet-ef` |
| IDE | — | Visual Studio 2022 (17.8+) or JetBrains Rider recommended; VS Code with the C# Dev Kit extension also works |

---

## 1. Clone the Repository

```bash
git clone https://github.com/your-org/skillscape.git
cd skillscape
```

---

## 2. Solution Structure

```
skillscape/
├── src/
│   ├── Skillscape.Core/     # Domain models and business logic
│   ├── Skillscape.Data/     # EF Core DbContext, migrations, repositories
│   └── Skillscape.Web/      # Blazor Server app (entry point)
├── tests/
│   └── Skillscape.Tests/    # xUnit test project
├── Directory.Packages.props # Central Package Management — all NuGet versions here
├── Directory.Build.props    # Shared MSBuild properties
└── Skillscape.sln
```

---

## 3. Restore Dependencies

This project uses [NuGet Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management). All package versions are declared in `Directory.Packages.props` at the solution root — individual `.csproj` files do not include `Version` attributes on `<PackageReference>` elements.

```bash
dotnet restore
```

---

## 4. Apply Database Migrations

The app uses SQLite. Run migrations from the web project directory so EF Core picks up the correct connection string from `appsettings.json`:

```bash
cd src/Skillscape.Web
dotnet ef database update
```

This creates the SQLite database file if it doesn't already exist.

---

## 5. Run the App

```bash
# from src/Skillscape.Web
dotnet run
```

Then open [https://localhost:5001](https://localhost:5001) (or the URL printed in the terminal).

---

## 6. Run Tests

```bash
# from the solution root
dotnet test
```

---

## SQLite Database

| Detail | Value |
|--------|-------|
| Default file location | `src/Skillscape.Web/skillscape.db` |
| Configured in | `src/Skillscape.Web/appsettings.json` → `ConnectionStrings:DefaultConnection` |

**To reset the database for development:**

```bash
# from src/Skillscape.Web
Remove-Item skillscape.db        # PowerShell
# or
rm skillscape.db                 # bash / zsh

dotnet ef database update        # re-creates the schema
```

To seed the database with sample data, run the app once — a development seed runs automatically when the app starts in the `Development` environment (`ASPNETCORE_ENVIRONMENT=Development`).
