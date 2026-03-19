# AI Context

## Purpose
- Fast onboarding note for AI contributors.
- Read this before editing features.
- Follow local patterns first; do not infer a cleaner architecture than the repo actually uses.

## Stack
- ASP.NET Core 8 Razor Pages
- Entity Framework Core + SQL Server
- Cookie + JWT mixed authentication
- Tailwind CSS + Razor shared components
- Single main app project: `PRN222_ApartmentManagement`

## Repo Map
| Path | Role |
| --- | --- |
| `PRN222_ApartmentManagement/Program.cs` | DI, auth, authorization, Razor Pages setup |
| `PRN222_ApartmentManagement/Data` | `ApartmentDbContext`, DB init |
| `PRN222_ApartmentManagement/Models` | entities + enums |
| `PRN222_ApartmentManagement/Repositories` | generic repository + entity-specific repositories |
| `PRN222_ApartmentManagement/Services` | cross-cutting/business services |
| `PRN222_ApartmentManagement/Pages` | UI + `PageModel` handlers by role |
| `PRN222_ApartmentManagement/Utils` | stateless helpers |
| `PRN222_ApartmentManagement/Migrations` | EF Core migrations |
| `PRN222_ApartmentManagement/wwwroot` | static assets |

## Runtime Shape
- No MVC controllers; Razor `PageModel` is the main request handler.
- Common flow:
  `Razor Page -> PageModel -> DbContext or Service/Repository -> EF Core -> SQL Server`.
- Architecture is hybrid, not strictly layered.
- Some pages use services/repositories.
- Many pages use `ApartmentDbContext` directly.
- Do not force every new change through repository/service if the local feature already uses direct EF access.

## Auth And Routing
- `Program.cs` enables auth for most pages.
- Auth scheme is `MixedAuth`.
- If request has `Authorization: Bearer ...`, JWT auth is used; otherwise cookies.
- Folder policies:
- `/Admin` -> `AdminOnly`
- `/BQL_Manager` -> `BQLManagerOnly`
- `/BQL_Staff` -> `BQLStaffOnly`
- `/Resident` -> `ResidentOnly`
- `/BQT_Head` -> `BQTHeadOnly`
- `/BQT_Member` -> `BQTMemberOnly`
- Main roles:
- `Admin`
- `BQL_Manager`
- `BQL_Staff`
- `Resident`
- `BQT_Head`
- `BQT_Member`

## Data Rules
- `ApartmentDbContext` is the schema source of truth.
- `OnModelCreating` defines enum-to-string conversion, unique indexes, relationships, delete behavior, and computed columns.
- Many enums are stored as strings.
- Common audit fields: `CreatedAt`, `UpdatedAt`, sometimes `LastLogin`.
- Soft delete exists in some entities, especially `User.IsDeleted`.
- Check delete behavior before changing FKs.
- `ApartmentDbContext` contains some repeated relationship config; avoid cleanup unless migration impact is verified.
- DB initialization is mixed:
- first run may use `EnsureCreatedAsync()`
- later updates may use `MigrateAsync()`
- be careful when changing schema/bootstrap behavior

## Layer Responsibilities
- `Pages`
- Request handling, binding, validation, redirects, display shaping, page-local queries.
- `Services`
- Reusable/cross-cutting logic: auth, email, settings, dashboard, activity logs.
- `Repositories`
- Reusable data access; most implementations inherit `GenericRepository<T>`.
- `Utils`
- Small helpers only.

## Current Reality Vs Preferred Direction
- Current reality:
- direct `DbContext` access inside `PageModel` is common
- repository usage exists but is partial
- activity logging is only automatic when writes go through `GenericRepository<T>` or code explicitly calls `ActivityLogService`
- Preferred for new code:
- stay close to nearby feature patterns
- keep `PageModel` thin when logic grows
- move reusable business rules into `Services`
- use repositories for reusable query/write logic or when consistent logging matters
- keep page-local/simple CRUD on direct `DbContext` if that folder already follows that style

## Code Conventions
- Use async EF APIs end-to-end.
- Use constructor injection.
- Keep C# naming conventional: `PascalCase` members, `_camelCase` private fields.
- Use `[BindProperty]` and `InputModel` for form input when helpful.
- GET filters commonly use `[BindProperty(SupportsGet = true)]`.
- Typical handler names: `OnGetAsync`, `OnPostAsync`, `OnPost<Action>Async`.
- Typical result flow: `return Page()`, `RedirectToPage(...)`, `LocalRedirect(...)`, `NotFound()`.
- Validate `ModelState` on form posts.
- Filter soft-deleted rows manually where relevant.
- Use `OrderBy` + `Skip` + `Take` for paging.
- `PageModel` often prepares UI-specific objects for tables/cards.
- Use `DateTime.Now` to match current codebase.
- Keep Vietnamese UI text style aligned with nearby pages.

## Feature Flow Template
1. Add/update entity or enum in `Models`.
2. Update `ApartmentDbContext` if schema rules changed.
3. Add migration if DB schema changed.
4. Add repository/service only if logic is reusable or non-trivial.
5. Implement page:
   - bind input
   - load current user/role if needed
   - validate auth + `ModelState`
   - query/update via `DbContext` or service/repository
   - set audit fields / soft-delete flags
   - `SaveChangesAsync()`
   - redirect or re-render
6. Update `.cshtml`.
7. Add logging if feature should be auditable.

## Real Example Flows
- Login:
  `Pages/Account/Login.cshtml.cs` -> `IAuthService.LoginAsync(...)` -> generate JWT + update `LastLogin` -> cookie sign-in -> role-based redirect.
- Admin user list:
  `Pages/Admin/Users/Index.cshtml.cs` -> direct `_context.Users` query -> search/filter/paging -> UI row shaping -> `OnPost...Async` updates -> `SaveChangesAsync()`.
- Resident face auth:
  `Pages/Resident/FaceAuth/RegisterModel.cshtml.cs` -> get current user from claims -> load `User` -> update face fields -> save -> redirect.

## Logging Rules
- `ActivityLogService` is scoped DI.
- `GenericRepository<T>` auto-logs create/update/delete on best effort.
- Direct `DbContext` writes do not auto-log.
- If auditability matters, call the logging service explicitly or route writes through repository/service that logs.

## AI Edit Rules
- Read nearby files in the same feature folder first.
- Match local patterns before introducing new abstractions.
- Do not introduce controllers, MediatR, or a new app-wide architecture by default.
- Do not "normalize" mixed architecture across the repo unless explicitly asked.
- Check role restrictions in `Program.cs` and page attributes.
- Check soft delete before removing data.
- Check enum conversion / indexes / FK behavior in `ApartmentDbContext`.
- If adding schema changes, add EF migration.
- If form re-renders on invalid state, reload dropdown/list data before `return Page()`.
- If bypassing repository for writes, decide whether missing audit logging is acceptable.

## Known Inconsistencies
- Mixed direct `DbContext` vs repository usage.
- Authorization uses both policy strings and role strings.
- PageModel filenames are inconsistent: some are `Index.cshtml.cs`, others `CreateModel.cshtml.cs`, `RegisterModel.cshtml.cs`, `StatusModel.cshtml.cs`.
- Activity log coverage is partial.
- Existing docs may describe ideal behavior more than actual code; trust source first.

## Fast Checklist
- Which role folder owns this feature?
- Is this page-local or reusable?
- Does entity need `CreatedAt`, `UpdatedAt`, or soft delete handling?
- Does query need `Include(...)`?
- If enum changed, did `OnModelCreating` need an update?
- If schema changed, did you add migration?
- If form fails validation, did you reload required page data?
- If write bypasses repository, is missing audit logging acceptable?
