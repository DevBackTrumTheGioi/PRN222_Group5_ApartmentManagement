# AI Agent Rules for Apartment Management System

**Version:** 1.0.0  
**Last Updated:** February 1, 2026  
**Status:** MANDATORY - All AI agents MUST follow these rules

---

## CORE PRINCIPLES

All AI-assisted development tasks in this project MUST strictly adhere to the following rules. No exceptions are permitted without explicit user approval.

---

## RULE 1: Database Structure Reference

**MANDATORY DATABASE LOOKUP**

- **Location**: `d:\PRN222\PRN222_ApartmentManagement\db.dbml`
- **Requirement**: ALWAYS read and reference the database schema from `db.dbml` before implementing any feature
- **When to check**:
  - Creating new models/entities
  - Implementing repositories
  - Creating database queries
  - Adding relationships between entities
  - Validating business logic
  - Writing any code that interacts with data

**Actions Required:**
```
1. Locate db.dbml file at project root
2. Read relevant table definitions
3. Verify relationships and constraints
4. Ensure code matches database structure
5. Follow naming conventions from schema
```

**Examples:**
- Creating Invoice model? Check Invoices table structure in db.dbml
- Adding relationships? Verify foreign keys in db.dbml
- Implementing validation? Check constraints in db.dbml

---

## RULE 2: No Icons in Documentation

**STRICTLY PROHIBITED**

Icons and emojis are NOT allowed in any documentation files including:
- README.md
- CONTRIBUTING.md
- Documentation files (.md, .txt)
- Code comments (except standard code documentation)
- Commit messages
- Pull request descriptions
- Issue descriptions

**Existing Files:**
If icons are found in existing documentation, they MUST be removed immediately.

**Allowed:**
- Standard Markdown formatting (headers, lists, code blocks, tables)
- Text-based symbols only when absolutely necessary (*, -, +, #)

**Examples:**

WRONG:
```markdown
## Features
- User management
- Invoice management
- Request tracking
```

CORRECT:
```markdown
## Features
- User management
- Invoice management  
- Request tracking
```

---

## RULE 3: No Post-Implementation Documentation

**PROHIBITED ACTIONS**

After implementing code, DO NOT automatically create:
- Documentation files (.md)
- README updates
- Example files
- Tutorial files
- Summary documents

**Exceptions:**
- User explicitly requests documentation
- Required for code functionality (e.g., API documentation in code comments)
- XML documentation comments in code (these are REQUIRED)

**What to do instead:**
1. Focus on code implementation
2. Add inline XML comments
3. Update existing docs ONLY if user requests
4. Let user decide on documentation needs

---

## RULE 4: Automatic Build and Error Checking

**MANDATORY POST-CODE ACTIONS**

After ANY code addition or modification, MUST execute:

```powershell
# 1. Build the project
dotnet build

# 2. Check for compilation errors
# Review build output

# 3. If errors exist, fix them immediately
# Do not proceed until build succeeds
```

**Required Steps:**
1. Write/modify code
2. Save file(s)
3. Run `dotnet build`
4. Check for errors
5. If errors: Fix and repeat from step 2
6. If success: Verify with `get_errors` tool
7. Only then consider task complete

**Error Handling:**
- Compilation errors: Fix immediately
- Warnings: Report to user, fix critical ones
- Build success: Proceed to next task

**Do NOT:**
- Skip build step
- Ignore errors
- Assume code works without building
- Mark task complete with build errors

---

## RULE 5: Task Compliance

**ALL TASKS** involving AI agents must follow these rules including:

- Code generation
- Refactoring
- Bug fixes
- Feature implementation
- Database migrations
- API development
- Testing
- Documentation updates
- File operations

**No Exceptions:**
These rules apply to ALL AI agent operations without exception.

---

## IMPLEMENTATION CHECKLIST

Before completing ANY task, verify:

```
[ ] Read db.dbml for database structure (if data-related)
[ ] No icons added to any documentation
[ ] No new documentation files created (unless requested)
[ ] Code built successfully with `dotnet build`
[ ] All compilation errors resolved
[ ] Error check completed with `get_errors` tool
[ ] Code follows project structure
[ ] XML comments added to public APIs
```

---

## ENFORCEMENT

**Violation Consequences:**
- Task must be redone
- Code changes must be reverted
- Re-implementation following rules

**Quality Assurance:**
Every AI agent action will be reviewed against these rules. Non-compliance results in immediate task rejection.

---

## PROJECT-SPECIFIC GUIDELINES

### Database
- Always reference `db.dbml` first
- Follow existing naming conventions
- Respect relationships and constraints
- Match data types exactly
- Use appropriate DeleteBehavior (Cascade/NoAction/SetNull/Restrict)

### Code Style
- XML documentation for public members
- Follow C# naming conventions
- Use async/await for I/O
- Repository pattern for data access
- ALL database operations MUST be async

### File Structure
- Follow existing project structure
- Place files in appropriate folders
- Use meaningful names
- No unnecessary files
- One model per file in `Models/` directory
- Repository interfaces in `Repositories/Interfaces/`
- Repository implementations in `Repositories/Implementations/`

### Build Process
- Build after every code change
- Fix errors immediately
- Check warnings
- Verify with error checker

---

## ENVIRONMENT-SPECIFIC CONFIGURATION

### Development (appsettings.Development.json)
- Local database connection (LocalDB or SQL Server Express)
- Detailed error messages enabled
- Verbose logging (Information level)
- Use for local development and testing

### Production (appsettings.json)
- Production database connection
- Generic error messages
- Warning/Error level logging only
- Secure credential storage (User Secrets/Key Vault)

---

## MIGRATION WORKFLOW

```bash
# Create new migration
dotnet ef migrations add DescriptiveName

# Review generated migration code in Migrations folder
# Check Up() and Down() methods carefully

# Apply migration to database
dotnet ef database update

# Rollback to previous migration
dotnet ef database update PreviousMigrationName

# Remove last migration (if NOT yet applied)
dotnet ef migrations remove

# Generate SQL script
dotnet ef migrations script -o migration.sql
```

### Migration Best Practices
- Use descriptive names (e.g., `AddContractTables` not `Migration1`)
- Review ALL generated code before applying
- Never modify applied migrations
- Test rollback functionality
- Keep migrations small and focused
- Commit migrations with related code changes

---

## ERROR HANDLING PATTERNS

### Controller/Page Model Level
```csharp
try
{
    var result = await _repository.GetByIdAsync(id);
    if (result == null)
    {
        _logger.LogWarning("Entity {Id} not found", id);
        return NotFound();
    }
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
- Let exceptions bubble up to controller
- Log only for debugging purposes
- Use `SaveChangesAsync()` with try-catch

---

## PERFORMANCE OPTIMIZATION

### Query Optimization
```csharp
// Use AsNoTracking for read-only queries
var apartments = await _context.Apartments
    .AsNoTracking()
    .ToListAsync();

// Implement pagination
var pagedResults = await _context.Apartments
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

// Avoid N+1 queries with Include
var apartmentsWithResidents = await _context.Apartments
    .Include(a => a.Residents)
    .ToListAsync();

// Project only needed properties
var apartmentSummaries = await _context.Apartments
    .Select(a => new { a.ApartmentId, a.ApartmentNumber })
    .ToListAsync();
```

---

## VALIDATION BEST PRACTICES

### Model Validation
```csharp
public class Apartment
{
    [Required(ErrorMessage = "Apartment number is required")]
    [StringLength(20, ErrorMessage = "Max length is 20 characters")]
    public string ApartmentNumber { get; set; }

    [Range(1, 100, ErrorMessage = "Floor must be between 1 and 100")]
    public int Floor { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? ContactEmail { get; set; }
}
```

---

## LOGGING STANDARDS

```csharp
// Information - normal operations
_logger.LogInformation("User {UserId} created apartment {ApartmentId}", userId, apartmentId);

// Warning - unexpected but handled
_logger.LogWarning("Failed login attempt for user {Username}", username);

// Error - exceptions and failures
_logger.LogError(ex, "Error processing invoice {InvoiceId}", invoiceId);

// Debug - detailed troubleshooting info
_logger.LogDebug("Processing step {Step} for entity {Id}", stepName, id);
```

**Remember:**
- Use structured logging with parameters
- Never log passwords or sensitive data
- Include context for troubleshooting
- Use appropriate log levels

---

## SECURITY CHECKLIST

- [ ] No hardcoded connection strings with passwords
- [ ] User Secrets configured for development
- [ ] All user input validated
- [ ] Parameterized queries used (EF Core default)
- [ ] Authentication/Authorization implemented
- [ ] HTTPS enforced in production
- [ ] Sensitive data not logged
- [ ] SQL injection prevented (via EF Core)
- [ ] XSS prevention in Razor Pages

---

## COMMON PITFALLS TO AVOID

### Database Issues
- ❌ Creating models without checking `db.dbml`
- ❌ Incorrect relationship configurations causing cascade conflicts
- ❌ Forgetting to add DbSet to DbContext
- ❌ Not configuring DeleteBehavior explicitly
- ❌ Using synchronous database operations

### Code Quality
- ❌ Mixing sync and async code
- ❌ Not using `await` with async methods
- ❌ Catching generic Exception without logging
- ❌ Returning null without null checks in callers
- ❌ Not implementing IDisposable when needed

### Performance
- ❌ Loading entire tables without filtering
- ❌ Not using `AsNoTracking()` for read-only queries
- ❌ Creating N+1 query problems
- ❌ Not implementing pagination
- ❌ Loading navigation properties unnecessarily

### Security
- ❌ Exposing internal error details to users
- ❌ Not validating user input
- ❌ Hardcoding credentials in code
- ❌ Not using HTTPS in production
- ❌ Logging sensitive information

---

## CODE REVIEW CHECKLIST

Before submitting code:

**Functionality**
- [ ] Implements required feature correctly
- [ ] All edge cases handled
- [ ] No hardcoded values

**Quality**
- [ ] Follows naming conventions
- [ ] No code duplication
- [ ] Proper error handling
- [ ] Logging implemented appropriately

**Database**
- [ ] Schema verified in `db.dbml`
- [ ] Relationships configured correctly
- [ ] Cascade behavior appropriate
- [ ] Migrations reviewed and tested

**Build & Test**
- [ ] `dotnet build` succeeds
- [ ] No compilation errors
- [ ] No critical warnings
- [ ] Manual testing completed

**Documentation**
- [ ] XML comments on public APIs
- [ ] No icons in documentation
- [ ] No unnecessary doc files

---

## TASK IMPLEMENTATION PRIORITY

When implementing new features:

1. **READ** `db.dbml` to understand schema
2. **CREATE/UPDATE** models matching database
3. **CONFIGURE** relationships in DbContext
4. **IMPLEMENT** repositories with error handling
5. **BUILD** project and fix all errors
6. **TEST** functionality manually
7. **VERIFY** all checklist items

---

## SUPPORT AND QUESTIONS

If rules are unclear or conflict with requirements:
1. Stop implementation immediately
2. Ask for clarification from team/lead
3. Document the decision made
4. Update rules if needed
5. Proceed only after confirmation

---

**Remember:** These rules ensure code quality, maintainability, team consistency, and project success.

---

**End of Rules - Version 1.0.0 - Last Updated: February 1, 2026**
