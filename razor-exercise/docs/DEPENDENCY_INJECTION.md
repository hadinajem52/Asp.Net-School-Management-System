# Dependency Injection in this project

## The simple idea

**Dependency injection (DI)** means that a class receives the objects it needs instead of creating them itself.

For example, `IndexModel` needs a `SchoolDataService` to load dashboard data.

Without DI, it might try to create one itself:

```csharp
// Avoid doing this here.
SchoolDataService schoolData = new SchoolDataService(...);
```

But `SchoolDataService` also needs a `SchoolDbContext`, and that context needs database settings. The page should not need to know how to build all of that.

With DI, the page states what it needs:

```csharp
public class IndexModel(SchoolDataService schoolData) : PageModel
```

ASP.NET creates and supplies `schoolData` when it creates `IndexModel`.

```mermaid
flowchart LR
    A["IndexModel needs a SchoolDataService"] --> B["ASP.NET DI container"]
    B --> C["Creates or finds a SchoolDataService"]
    C --> A
```

## Where the dependencies are registered

`Program.cs` tells ASP.NET which objects it is allowed to provide:

```csharp
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<SchoolDataService>();
```

These registrations mean:

```text
SchoolDbContext     → ASP.NET knows how to create one with the database connection settings.
SchoolDataService   → ASP.NET knows how to create one when a class asks for it.
```

## DI lifetimes

A **lifetime** tells ASP.NET how long it should keep an object before creating a new one.

| Lifetime | Meaning | Typical use |
| --- | --- | --- |
| `Transient` | Create a new object every time it is requested. | Small, stateless helper services. |
| `Scoped` | Create one object per web request, then reuse it during that request. | Database contexts and services that use them. |
| `Singleton` | Create one object for the entire application lifetime. | Shared configuration or a safe, stateless shared service. |

```mermaid
flowchart LR
    T["Transient"] --> T1["New object"]
    T --> T2["Another request = another new object"]
    S["Scoped"] --> S1["One object for request A"]
    S --> S2["One different object for request B"]
    G["Singleton"] --> G1["One shared object for the whole running app"]
```

### The lifetime in this app

This project registers its data service as scoped:

```csharp
builder.Services.AddScoped<SchoolDataService>();
```

`AddDbContext<SchoolDbContext>(...)` also registers `SchoolDbContext` as **scoped by default**.

So, for one browser request, such as `GET /Students/Create`:

```text
Start request
→ ASP.NET creates one SchoolDataService
→ ASP.NET creates one SchoolDbContext
→ CreateModel uses the service
→ the service uses that context
→ request ends
→ ASP.NET disposes the scoped objects
```

The next browser request receives new scoped instances.

```mermaid
sequenceDiagram
    participant A as Request A: GET /Students
    participant DI as DI container
    participant Service as SchoolDataService
    participant Context as SchoolDbContext

    A->>DI: Create Students IndexModel
    DI->>Service: Create scoped service
    DI->>Context: Create scoped database context
    Service->>Context: Use this context for queries
    A-->>DI: Request ends
    DI->>Service: Dispose scoped service
    DI->>Context: Dispose scoped context

    Note over DI,Context: A later request gets new scoped instances.
```

`Scoped` is a good fit here because a database context should not be shared by every user and every request for the whole lifetime of the application.

## How it is used in this project

### 1. A Razor Page receives the service

`Pages/Index.cshtml.cs` needs school data for the dashboard:

```csharp
public class IndexModel(SchoolDataService schoolData) : PageModel
{
    public async Task OnGetAsync()
    {
        StudentCount = await schoolData.GetStudentCountAsync();
    }
}
```

`schoolData` is provided by DI. `IndexModel` does not create it with `new`.

### 2. The service receives the database context

`SchoolDataService` needs a way to query and save database data:

```csharp
public class SchoolDataService(SchoolDbContext db)
```

`db` is also provided by DI.

### The complete chain

```mermaid
flowchart TD
    P["Program.cs registers SchoolDataService and SchoolDbContext"]
    R["Browser requests a Razor Page"]
    DI["ASP.NET DI container"]
    Page["IndexModel or CreateModel"]
    Service["SchoolDataService"]
    Context["SchoolDbContext"]
    Database[("PostgreSQL database")]

    P --> DI
    R --> DI
    DI -->|"injects"| Page
    DI -->|"injects"| Service
    Page -->|"calls"| Service
    Service -->|"uses"| Context
    Context -->|"queries or saves"| Database
```

## Why use DI?

DI keeps each class focused on its own job:

```text
Razor Page        → handle the web request and prepare page data
SchoolDataService → perform school data operations
SchoolDbContext   → communicate with the database
Program.cs        → configure how these objects are created
```

It also makes replacement easier. For example, a test could provide a fake data service instead of connecting to a real database.

## The pattern to recognize

When you see a class declaration like this:

```csharp
public class CreateModel(SchoolDataService schoolData) : PageModel
```

read it as:

> “To create `CreateModel`, ASP.NET must give it a `SchoolDataService`.”

For that to work, `SchoolDataService` must have been registered in `Program.cs`.
