# Student Course Management Database Structure

```mermaid
erDiagram
    direction LR

    STUDENT ||--o{ ENROLLMENT : has
    COURSE ||--o{ ENROLLMENT : has

    STUDENT {
        int Id PK
        string FirstName
        string LastName
        string Email
        date DateOfBirth
    }

    COURSE {
        int Id PK
        string Name UK
        string Description
        int Credits
    }

    ENROLLMENT {
        int StudentId PK, FK
        int CourseId PK, FK
    }
```

`Enrollment` connects students and courses. Its `StudentId` and `CourseId` columns form a composite primary key, preventing the same student from being enrolled in the same course more than once.

`Course.Name` is marked as a unique key. These constraints will be implemented later in `SchoolDbContext`.
