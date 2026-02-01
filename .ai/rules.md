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

### Code Style
- XML documentation for public members
- Follow C# naming conventions
- Use async/await for I/O
- Repository pattern for data access

### File Structure
- Follow existing project structure
- Place files in appropriate folders
- Use meaningful names
- No unnecessary files

### Build Process
- Build after every code change
- Fix errors immediately
- Check warnings
- Verify with error checker

---

## EXAMPLES

### Good Workflow

```
1. User: "Add payment method field to Invoice"
2. Agent: Reads db.dbml for Invoices table
3. Agent: Modifies Invoice model
4. Agent: Updates repository if needed
5. Agent: Runs `dotnet build`
6. Agent: Checks for errors
7. Agent: Reports success (no docs created)
```

### Bad Workflow

```
1. User: "Add payment method field to Invoice"
2. Agent: Creates Invoice model (WITHOUT checking db.dbml)
3. Agent: Creates README.md with changes
4. Agent: SKIPS build step
5. Result: REJECTED - Violates Rules 1, 3, and 4
```

---

## SUMMARY

**Remember:**
1. CHECK DATABASE FIRST (db.dbml)
2. NO ICONS ANYWHERE
3. NO AUTO-DOCS
4. ALWAYS BUILD AND FIX ERRORS
5. EVERY TASK FOLLOWS RULES

**These rules are NON-NEGOTIABLE and apply to ALL AI operations.**

---

**Version Control:**
- Changes to these rules require explicit approval
- All agents must re-read rules after updates
- Violations must be reported and corrected

**End of AI Rules Document**

