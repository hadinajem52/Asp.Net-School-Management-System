# Request trace: POST Students/Edit

This trace follows a valid submission of the **Edit student** form. The
alternative paths show what happens when validation fails or the student no
longer exists.

```mermaid
sequenceDiagram
    participant Browser
    participant Razor as Razor Pages
    participant DI as DI container
    participant Page as Students EditModel
    participant Service as SchoolDataService
    participant Context as SchoolDbContext
    participant Db as Database

    Browser->>Razor: POST /Students/Edit/{id} with form values
    Razor->>DI: Create EditModel for this request
    DI->>Context: Create scoped context
    DI->>Service: Create scoped service with context
    DI->>Page: Provide service in constructor
    Razor->>Page: Bind form values to Student
    Razor->>Page: Run OnPostAsync()

    alt ModelState is invalid
        Page-->>Razor: Return Page()
        Razor-->>Browser: Re-render Edit page with validation errors
    else ModelState is valid
        Page->>Service: UpdateStudentAsync(Student)
        Service->>Context: FindAsync(Student.Id)
        alt Student does not exist
            Service-->>Page: Return false
            Page-->>Razor: Return NotFound()
            Razor-->>Browser: Send 404 response
        else Student exists
            Service->>Context: Copy submitted values to existingStudent
            Service->>Context: SaveChangesAsync()
            Context->>Db: Run UPDATE SQL
            Db-->>Context: Confirm row was updated
            Context-->>Service: Save completed
            Service-->>Page: Return true
            Page->>Page: Store SuccessMessage in TempData
            Page-->>Razor: RedirectToPage("./Index")
            Razor-->>Browser: Send redirect response to /Students/Index
        end
    end

    Razor->>DI: Request ends
    DI->>Context: Dispose scoped context
```

When validation fails, `return Page()` keeps the learner on the Edit page so
Razor Pages can display validation messages. If the student no longer exists,
the handler returns a 404 response. When validation succeeds, the service finds
the existing row using the submitted `Student.Id`, copies the editable values,
saves the update, stores a one-time success message in `TempData`, and redirects
to the Index page. The browser follows that redirect with a new GET request,
which creates and later disposes its own scoped services.
