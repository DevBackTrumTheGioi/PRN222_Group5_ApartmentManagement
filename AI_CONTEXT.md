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

## UI Conventions
- Main UI stack:
- Razor Pages `.cshtml`
- Tailwind utility classes from `wwwroot/css/output.css`
- Google `Inter` font
- Material Icons
- Authenticated pages usually render through shared layouts and partials in `Pages/Shared`.
- Visual baseline:
- background is usually `bg-slate-50`
- main surfaces are `bg-white`
- borders are mostly `border-slate-200`
- main text uses `text-slate-900`, secondary text uses `text-slate-700` and `text-slate-500`
- corners are usually `rounded-lg` or `rounded-xl`
- shadows are light, usually `shadow-sm`
- Primary color direction is emerald/green.
- Common primary action styling:
- `bg-emerald-600`
- hover `bg-emerald-700`
- selected tint `bg-emerald-50`
- selected text/icon `text-emerald-600` or `text-emerald-700`
- Semantic color usage:
- emerald -> active, success, primary actions
- amber -> pending, warning, approval waiting
- red -> validation error, destructive action, alert
- slate -> neutral, inactive, secondary UI
- Layout pattern:
- left sidebar is fixed and usually `w-64`
- content shell shifts with `lg:ml-64`
- top header is sticky, white/translucent, with blur and bottom border
- page content lives in `main` with generous padding like `p-4 md:p-6 lg:p-8`
- footer is simple, white, and lightly bordered
- Sidebar pattern:
- white vertical nav with right border
- top branding block has a small emerald tile/icon
- menu groups can be collapsible
- active link uses emerald tint and stronger font weight
- inactive links use slate text and subtle hover background
- labels are short, practical, and admin-style
- Page header pattern:
- title on the left, actions on the right
- title is often `text-2xl font-bold tracking-tight`
- subtitle is optional and muted
- breadcrumb/header metadata may be passed through `ViewData["PageHeader"]`
- Form pattern:
- forms are usually wrapped in a white card with `rounded-xl`, `border`, and `shadow-sm`
- major vertical spacing commonly uses `space-y-6`
- field layout often uses `grid grid-cols-1 md:grid-cols-2 gap-6`
- labels use `text-sm font-medium text-slate-700`
- inputs/selects/textareas use soft borders, rounded corners, and emerald focus ring
- validation summary appears near the top in a red-tinted alert box
- per-field validation text is small red text below the input
- form footer actions usually sit in `bg-slate-50` with a top border
- Table and list pattern:
- list pages often use a white rounded bordered container
- table headers often use `bg-slate-50`
- rows use light separators and subtle hover highlight
- search/filter controls usually appear above the table
- wide tables should be wrapped with `overflow-x-auto`
- Card/stat pattern:
- cards are white, bordered, rounded, and lightly shadowed
- metric cards often combine a bold number with a tinted icon chip
- avoid visually dense cards; keep spacing breathable
- Status and badge pattern:
- badges should be compact and easy to scan
- use color first for semantic meaning, but keep text explicit
- prefer consistency with existing emerald/amber/red/slate combinations
- Content and copy style:
- user-facing text is Vietnamese
- headings, labels, and button text are concise and literal
- common verbs are `Tạo`, `Lưu`, `Cập nhật`, `Lọc`, `Duyệt`, `Hủy`
- avoid long marketing-style copy or decorative slogans
- Responsive pattern:
- write mobile-first Tailwind classes
- stack vertically by default, then expand to columns from `md:` upward
- keep key actions visible without forcing horizontal scroll
- use `max-w-*` containers for forms/detail pages when needed
- Preferred `.cshtml` authoring style in this repo:
- set `ViewData["Title"]`
- set `ViewData["CurrentPage"]` when page should highlight the sidebar
- use shared layout/components before creating new custom wrappers
- compose pages from known blocks: header, card, filters, table, form footer
- match nearby spacing, border treatment, and color usage before inventing a new visual style
- do not introduce a new theme or different design language unless explicitly requested

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
