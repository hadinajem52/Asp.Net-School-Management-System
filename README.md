# Student Course Management System

This repository builds the same school-management application in two ASP.NET Core styles:

- **Razor Pages** in `razor-exercise/`.
- **MVC with progressive AJAX enhancement** in `MVC-exercise/`.

Both projects use ASP.NET Core Identity, Entity Framework Core, and PostgreSQL. They intentionally use separate databases so they can run side by side.

## Features

- Dashboard totals and recently added students.
- Student and Course list, search, details, create, edit, and delete workflows.
- Student-to-Course enrollment relationships.
- Model validation for required fields, email, birth dates, unique Course names, and Course credits.
- Cookie authentication with `Admin` and `Viewer` roles.
- Viewer signup with Admin approval or rejection.
- Role-based read and write permissions.

The MVC version also includes progressive AJAX navigation and form submission, reusable partial views, browser-history support, loading feedback, and confirmation dialogs. Its normal MVC links and forms continue to work when JavaScript is unavailable.

## Repository structure

```text
.
├── MVC-exercise/       MVC controllers, views, AJAX, and partial views
├── razor-exercise/     Razor Pages and Razor Components version
├── .vscode/            VS Code build, run, and stop tasks for MVC
└── README.md
```

Detailed learning notes:

- [MVC building process](MVC-exercise/docs/MVCBuildingProcess.md)
- [Razor Pages building process](razor-exercise/docs/RazorBuildingProcess.md)
- [MVC database diagram](MVC-exercise/Models/Diagram/database-structure.md)
- [Razor database diagram](razor-exercise/Data/database-structure.md)

## Prerequisites

- .NET SDK with .NET 10 support.
- Docker Desktop or another Docker-compatible runtime.
- Entity Framework Core command-line tools:

```powershell
dotnet tool install --global dotnet-ef
```

If `dotnet-ef` is already installed, update it instead:

```powershell
dotnet tool update --global dotnet-ef
```

## Database setup

### Razor Pages database

```powershell
docker run --name student-course-db `
  -e POSTGRES_USER=studentapp `
  -e POSTGRES_PASSWORD=<your-local-password> `
  -e POSTGRES_DB=student_course_management `
  -p 5432:5432 `
  -d postgres:17
```

### MVC database

```powershell
docker run --name student-course-mvc-db `
  -e POSTGRES_USER=studentapp_mvc `
  -e POSTGRES_PASSWORD=<your-local-password> `
  -e POSTGRES_DB=student_course_management_mvc `
  -p 5433:5432 `
  -d postgres:17
```

For containers that have already been created:

```powershell
docker start student-course-db
docker start student-course-mvc-db
```

## Configure local secrets

Passwords and initial Admin credentials are not stored in source control. Configure each project with .NET user secrets.

### Razor Pages

```powershell
dotnet user-secrets set "ConnectionStrings:SchoolDatabase" `
  "Host=localhost;Port=5432;Database=student_course_management;Username=studentapp;Password=<your-local-password>" `
  --project razor-exercise/razor-exercise.csproj

dotnet user-secrets set "IdentitySeed:AdminEmail" "admin@example.com" `
  --project razor-exercise/razor-exercise.csproj

dotnet user-secrets set "IdentitySeed:AdminPassword" "<strong-admin-password>" `
  --project razor-exercise/razor-exercise.csproj
```

### MVC

```powershell
dotnet user-secrets set "ConnectionStrings:SchoolDatabase" `
  "Host=localhost;Port=5433;Database=student_course_management_mvc;Username=studentapp_mvc;Password=<your-local-password>" `
  --project MVC-exercise/MVC-exercise.csproj

dotnet user-secrets set "IdentitySeed:AdminEmail" "admin@example.com" `
  --project MVC-exercise/MVC-exercise.csproj

dotnet user-secrets set "IdentitySeed:AdminPassword" "<strong-admin-password>" `
  --project MVC-exercise/MVC-exercise.csproj
```

Use the same PostgreSQL password in the Docker command and the corresponding connection string.

## Apply migrations

Run these commands after the containers are ready:

```powershell
dotnet ef database update `
  --project razor-exercise/razor-exercise.csproj `
  --startup-project razor-exercise/razor-exercise.csproj

dotnet ef database update `
  --project MVC-exercise/MVC-exercise.csproj `
  --startup-project MVC-exercise/MVC-exercise.csproj
```

The applications seed the Admin and Viewer roles, the configured Admin account, and sample school data during startup. The seeders check existing records before inserting data.

## Run the applications

Run one project at a time from the repository root.

### Razor Pages

```powershell
dotnet run --project razor-exercise/razor-exercise.csproj
```

Default HTTP address: `http://localhost:5141`

### MVC

```powershell
dotnet run --project MVC-exercise/MVC-exercise.csproj
```

Default HTTP address: `http://localhost:5244`

The MVC project can also be built or started through the VS Code tasks:

- `dotnet build`
- `dotnet run`
- `dotnet stop MVC-exercise`

## Authentication and authorization

- Anonymous users can access Login and Signup only.
- A new signup starts with a pending approval status and no Viewer role.
- An Admin can approve or reject pending account requests.
- Approval assigns the `Viewer` role.
- Viewers can read Student and Course information.
- Admins can create, edit, and delete records and manage protected workflows.

Sign in with the Admin email and password configured in user secrets. No default password is committed to this repository.

## Build

```powershell
dotnet build razor-exercise/razor-exercise.csproj
dotnet build MVC-exercise/MVC-exercise.csproj
```

The projects are independent and do not require a solution file.
