# Razor Pages orchestration

This diagram shows what happens when a browser requests the dashboard route (`/`).

```mermaid
sequenceDiagram
    participant Browser
    participant Program as Program.cs
    participant Razor as Razor Pages
    participant PageModel as Index.cshtml.cs<br/>IndexModel
    participant Service as SchoolDataService
    participant Context as SchoolDbContext
    participant Database
    participant View as Index.cshtml
    participant Layout as Shared/_Layout.cshtml

    Browser->>Program: GET /
    Program->>Razor: MapRazorPages() enables page routes
    Razor->>PageModel: Create IndexModel<br/>(inject SchoolDataService)
    Razor->>PageModel: Run OnGetAsync()
    PageModel->>Service: GetStudentCountAsync()<br/>GetCourseCountAsync()<br/>GetMostRecentStudentsAsync()
    Service->>Context: Query Students and Courses
    Context->>Database: Run SQL queries
    Database-->>Context: Return rows/counts
    Context-->>Service: Return query results
    Service-->>PageModel: Return counts and students
    PageModel-->>Razor: StudentCount, CourseCount, RecentStudents ready
    Razor->>View: Render @Model values as page HTML
    Razor->>Layout: Apply layout chosen by _ViewStart.cshtml
    Layout->>Layout: Insert page HTML at @RenderBody()
    Layout-->>Browser: Send completed HTML response
```

## Responsibilities

```mermaid
flowchart TD
    P["Program.cs<br/>Registers services and enables Razor Pages routes"]
    VS["_ViewStart.cshtml<br/>Selects the shared layout"]
    PM["Index.cshtml.cs / IndexModel<br/>Handles GET or POST requests and prepares page data"]
    S["SchoolDataService<br/>Performs school data operations"]
    DB["SchoolDbContext<br/>Talks to the database through EF Core"]
    V["Index.cshtml<br/>Displays the prepared data as HTML"]
    L["Shared/_Layout.cshtml<br/>Provides the shared page shell"]

    P --> PM
    P --> S
    S --> DB
    VS --> L
    PM --> S
    PM --> V
    V --> L
```

`Index.cshtml.cs` does not normally query the database directly. It asks `SchoolDataService`, then makes the resulting values available to `Index.cshtml` through `@Model`.
