# GitHub Copilot Instructions

This file contains mandatory rules that GitHub Copilot MUST follow when working on this project.

## Database Schema Reference

ALWAYS check `db.dbml` file at project root before:
- Creating or modifying models
- Implementing repositories
- Writing database queries
- Adding entity relationships
- Implementing business logic with data

The database schema in `db.dbml` is the single source of truth for all data structures.

## Code-First Entity Framework Rules

### Model Creation
- Each model MUST be in a separate file
- File name matches class name (e.g., `Apartment.cs` for `Apartment` class)
- All models in `Models/` directory
- Use Data Annotations for validation and schema definition
- Include navigation properties for relationships

### Entity Relationships
- Configure relationships in `DbContext.OnModelCreating()`
- Use appropriate `DeleteBehavior` to avoid cascade path conflicts:
  - `DeleteBehavior.Cascade` - For required relationships
  - `DeleteBehavior.NoAction` - To prevent cascade cycles
  - `DeleteBehavior.SetNull` - For optional relationships
  - `DeleteBehavior.Restrict` - To prevent deletion with related data

### Naming Conventions
- PascalCase for classes, properties, methods
- camelCase for private fields with underscore prefix (`_fieldName`)
- Plural for DbSet names (`DbSet<User> Users`)
- Singular for table names via `[Table("TableName")]`

## Build and Error Checking

After ANY code change:
1. Save all files
2. Run `dotnet build`
3. Check compilation errors
4. Fix all errors before proceeding
5. Verify with `dotnet build` again
6. Do NOT mark task complete if errors exist

Commands:
```bash
dotnet clean
dotnet build
dotnet build --no-restore
```

## Repository Pattern Implementation

### Interface Structure
```csharp
public interface IEntityRepository
{
    Task<IEnumerable<Entity>> GetAllAsync();
    Task<Entity?> GetByIdAsync(int id);
    Task<Entity> AddAsync(Entity entity);
    Task UpdateAsync(Entity entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
```

### Implementation Guidelines
- Use async/await for all database operations
- Implement proper error handling
- Use `ILogger` for logging
- Return `null` for not found (not exceptions)
- Validate input parameters

## Documentation Standards

### PROHIBITED
- Do NOT use icons/emojis in any documentation
- Do NOT create documentation files after code implementation unless explicitly requested
- Do NOT use decorative symbols in comments

### REQUIRED
- XML documentation comments for all public APIs
- Inline comments for complex logic only
- Update existing README.md only when requested

Example:
```csharp
/// <summary>
/// Gets all active apartments with their residents
/// </summary>
/// <returns>List of apartments with eager-loaded residents</returns>
public async Task<IEnumerable<Apartment>> GetAllWithResidentsAsync()
{
    return await _context.Apartments
        .Include(a => a.Residents)
        .Where(a => a.Status == "Active")
        .ToListAsync();
}
```

## Database Migration Guidelines

### Creating Migrations
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Review generated migration code
# Check Up() and Down() methods

# Apply migration
dotnet ef database update

# Rollback if needed
dotnet ef database update PreviousMigrationName
```

### Migration Best Practices
- Use descriptive migration names (e.g., `AddContractTables`)
- Review migration code before applying
- Test rollback functionality
- Never modify applied migrations
- Include `HasData()` for seed data if needed

## Error Handling Patterns

### Controller/Page Model Level
```csharp
try
{
    var result = await _repository.GetByIdAsync(id);
    if (result == null)
        return NotFound();
    
    return Page();
}
catch (DbUpdateException ex)
{
    _logger.LogError(ex, "Database error when retrieving entity {Id}", id);
    ModelState.AddModelError("", "An error occurred while processing your request");
    return Page();
}
```

### Repository Level
- Let exceptions bubble up
- Log at repository level only for debugging
- Use `SaveChangesAsync()` with try-catch in repository

## Async/Await Rules

- ALL database operations MUST be async
- Use `async Task<T>` for methods returning data
- Use `async Task` for void operations
- Always `await` database calls
- Use `ConfigureAwait(false)` in library code only

## Testing Requirements

When implementing features:
- Write unit tests for business logic
- Use in-memory database for repository tests
- Mock dependencies in unit tests
- Test both success and failure cases

## Security Guidelines

- NEVER hardcode connection strings with passwords
- Use User Secrets for development
- Use Azure Key Vault or similar for production
- Validate all user input
- Use parameterized queries (EF Core handles this)
- Implement authentication/authorization

## Performance Considerations

- Use `AsNoTracking()` for read-only queries
- Implement pagination for large data sets
- Use `Select()` to project only needed properties
- Avoid N+1 queries with `Include()` or `ThenInclude()`
- Index foreign keys and frequently queried columns

## Code Structure Requirements

### File Organization
```
PRN222_ApartmentManagement/
├── Data/
│   ├── ApartmentDbContext.cs
│   └── DbInitializer.cs
├── Models/
│   ├── Apartment.cs
│   ├── Resident.cs
│   └── (one file per model)
├── Repositories/
│   ├── Interfaces/
│   └── Implementations/
├── Utils/
│   └── (utility classes)
├── Pages/
│   └── (Razor pages)
└── wwwroot/
```

### Namespace Rules
- Match folder structure
- Use `PRN222_ApartmentManagement.FolderName`
- Be consistent across all files

## Validation Rules

### Model Validation
- Use Data Annotations for basic validation
- Implement `IValidatableObject` for complex validation
- Validate in both client and server side

### Required Attributes
```csharp
[Required(ErrorMessage = "Field is required")]
[StringLength(100, ErrorMessage = "Max length is 100")]
[Range(1, 1000, ErrorMessage = "Must be between 1 and 1000")]
[EmailAddress(ErrorMessage = "Invalid email format")]
```

## Logging Standards

- Log at appropriate levels (Debug, Information, Warning, Error)
- Include context in log messages
- Use structured logging with parameters
- Do NOT log sensitive information (passwords, tokens)

Example:
```csharp
_logger.LogInformation("User {UserId} created apartment {ApartmentId}", userId, apartmentId);
_logger.LogWarning("Failed login attempt for user {Username}", username);
_logger.LogError(ex, "Error processing invoice {InvoiceId}", invoiceId);
```

## Git Commit Guidelines

- Use present tense ("Add feature" not "Added feature")
- Be descriptive but concise
- Reference issue numbers when applicable
- One logical change per commit

## Environment Configuration

### Development
- Use `appsettings.Development.json`
- Can point to LocalDB or local SQL Server
- Enable detailed errors and logging

### Production
- Use `appsettings.json`
- Store secrets in secure location
- Disable detailed errors
- Configure production logging level

## Final Checklist

Before completing ANY task:
- [ ] Database schema checked in `db.dbml`
- [ ] Code builds without errors (`dotnet build`)
- [ ] No icons in documentation
- [ ] XML comments added to public APIs
- [ ] Async/await used for database operations
- [ ] Repository pattern followed
- [ ] Error handling implemented
- [ ] Logging added where appropriate
- [ ] No hardcoded connection strings or secrets

## Rule Enforcement

These rules are MANDATORY. Any code that violates these rules MUST be refactored before merge.

Non-compliance will result in:
1. Code review rejection
2. Required refactoring
3. Re-implementation following guidelines

## Questions or Clarifications

If any rule is unclear or conflicts with requirements, ask for clarification before proceeding with implementation.

