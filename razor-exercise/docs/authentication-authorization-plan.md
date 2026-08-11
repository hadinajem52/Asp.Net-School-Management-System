# Authentication and Authorization Plan

## Goal

Restrict the Student Course Management System so that people must sign in before using it, and their role controls what they can do.

We will use **ASP.NET Core Identity** with an authentication **cookie**.

- Authentication answers: **Who is the current user?**
- Authorization answers: **Is this user allowed to perform this action?**

## Roles

| Role | Allowed actions |
| --- | --- |
| `Admin` | View, create, edit, and delete students, courses, and enrollments. |
| `Viewer` | View the dashboard, students, courses, and enrollment information, but cannot change data. |

`Admin` and `Viewer` are application choices. ASP.NET Core Identity does not create these role names automatically.

## Plan

### 1. Add ASP.NET Core Identity

- Add the required Identity package.
- Create an `ApplicationUser` class for application accounts.
- Configure Identity to use the existing PostgreSQL database.
- Configure cookie login and logout behavior.

**Result:** The application knows how to create accounts, store password hashes securely, and identify a signed-in user from their cookie.

### 2. Update the database

- Create an Entity Framework Core migration.
- Apply it to PostgreSQL.
- Verify that Identity tables were created, such as users, roles, and user-role links.

**Result:** PostgreSQL stores accounts and roles alongside the existing student-course tables.

### 3. Add Identity middleware

Register middleware in this order:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

Authentication must run first: the application must identify the user before it can check their permissions.

**Result:** Each request can have a current user and can be allowed or denied.

### 4. Add login and logout pages

- Add a login page.
- Add a logout action.
- Update the navigation bar to show login/logout and the signed-in user's name.

For now, we will not add public registration. An administrator account will be created through seed data.

**Result:** A user can sign in and sign out through the application UI.

### 5. Seed roles and an administrator account

- Create the `Admin` and `Viewer` roles if they do not exist.
- Create one local administrator account.
- Assign that account to the `Admin` role.

The initial password must come from user secrets or an environment variable, never from source code.

**Result:** We have a safe first account to use while developing.

### 6. Protect the application pages

- Require a signed-in user for dashboard, students, courses, and enrollment pages.
- Allow both `Admin` and `Viewer` to view information.
- Restrict Create, Edit, Delete, Enroll, and Remove Enrollment handlers to `Admin`.
- Ensure POST handlers are protected too, not only buttons or links in the UI.

**Result:** Authorization is enforced by the server even if someone manually types a protected URL.

### 7. Verify the behavior

Test these cases:

1. A signed-out visitor is redirected to login.
2. An `Admin` can complete all management actions.
3. A `Viewer` can view data but receives an access-denied result for a write URL or POST request.
4. Signing out prevents access to protected pages again.

**Result:** We know the feature works for both allowed and denied requests.

## Future extension: Student accounts

Later, add a `Student` role and link an `ApplicationUser` to one `Student` record. A student could then view only their own details and enrollments. This requires an additional database relationship, so it is deliberately outside this first version.

## Suggested implementation order

We will complete one section at a time and explain it before moving on:

1. Add ASP.NET Core Identity configuration.
2. Create and apply the database migration.
3. Add authentication middleware.
4. Build login and logout.
5. Seed the roles and initial admin account.
6. Apply authorization rules.
7. Test the complete flow.
