# AI Rules Configuration Summary

## Files Created/Updated

### 1. `.github/copilot-instructions.md` (NEW)
**Purpose:** GitHub Copilot specific instructions
**Scope:** Applies when using GitHub Copilot in this repository
**Key Features:**
- Database schema reference requirements
- Code-First EF Core standards
- Build and error checking requirements
- Repository pattern guidelines
- Documentation standards (no icons)
- Migration workflows
- Error handling patterns
- Performance optimization tips

### 2. `.cursorrules` (UPDATED)
**Purpose:** Cursor AI editor instructions
**Scope:** Applies when using Cursor IDE
**Key Features:**
- Condensed version of main rules
- Quick reference format
- Focus on immediate coding standards
- Task completion checklist

### 3. `.ai/rules.md` (ENHANCED)
**Purpose:** Comprehensive AI agent rules
**Scope:** All AI assistants working on this project
**Key Features:**
- Complete rule documentation
- Detailed examples and patterns
- Common pitfalls to avoid
- Code review checklist
- Security guidelines
- Performance optimization
- Migration workflows

## When Each File is Used

### GitHub Copilot (`.github/copilot-instructions.md`)
Used when:
- Writing code in VS Code/Visual Studio with Copilot enabled
- Getting code suggestions
- Using Copilot chat
- Generating code completions

### Cursor AI (`.cursorrules`)
Used when:
- Working in Cursor editor
- AI-assisted coding in Cursor
- Cursor's AI features (Cmd+K, Cmd+L)

### General AI Agents (`.ai/rules.md`)
Used when:
- Any AI assistant works on the project
- Code reviews
- Feature implementation
- Refactoring tasks

## Core Rules Summary

### 1. Database Schema Reference (MANDATORY)
- ALWAYS check `db.dbml` before data-related work
- Database schema is single source of truth
- Verify relationships and constraints

### 2. Build and Error Checking (MANDATORY)
- Run `dotnet build` after ANY code change
- Fix ALL errors before proceeding
- Never mark task complete with errors

### 3. No Icons in Documentation (STRICTLY PROHIBITED)
- No emojis or icons in any .md files
- No decorative symbols in documentation
- Clean, professional documentation only

### 4. No Post-Implementation Documentation
- Don't auto-create documentation after code
- Only create docs when explicitly requested
- XML comments on public APIs are required

### 5. Code-First Entity Framework
- One model per file
- Configure relationships in DbContext
- Use appropriate DeleteBehavior
- All database operations must be async

## Quick Reference Commands

```bash
# Build project
dotnet build

# Create migration
dotnet ef migrations add MigrationName

# Apply migration
dotnet ef database update

# Rollback migration
dotnet ef database update PreviousMigrationName

# Remove last migration
dotnet ef migrations remove

# Run application
dotnet run --project PRN222_ApartmentManagement
```

## Task Completion Checklist

Before completing ANY task:
- [ ] Checked `db.dbml` for schema (if data-related)
- [ ] Code builds successfully (`dotnet build`)
- [ ] No compilation errors
- [ ] No icons in documentation
- [ ] XML comments added to public APIs
- [ ] Async/await used correctly
- [ ] Error handling implemented
- [ ] Logging added appropriately

## Verification

To verify rules are working:

1. **GitHub Copilot**: Ask in chat "What are the coding rules for this project?"
2. **Cursor**: Rules automatically applied when using AI features
3. **General**: Check if AI follows database reference requirement

## Next Steps

1. Commit these files to repository
2. Push to GitHub
3. Team members clone repo - rules automatically available
4. Start using AI assistants - they will follow these rules

## File Locations

```
PRN222_ApartmentManagement/
├── .github/
│   └── copilot-instructions.md    [GitHub Copilot Rules]
├── .ai/
│   └── rules.md                    [Comprehensive AI Rules]
├── .cursorrules                    [Cursor AI Rules]
└── db.dbml                         [Database Schema - SOURCE OF TRUTH]
```

## Important Notes

1. **All rules are MANDATORY** - No exceptions without approval
2. **Database schema in `db.dbml`** is the single source of truth
3. **Build verification** is required after every code change
4. **Documentation must be clean** - no icons or emojis
5. **XML comments required** on all public APIs

## Updates

To update rules:
1. Edit the appropriate file(s)
2. Update version number
3. Update "Last Updated" date
4. Commit changes
5. Notify team

---

**Version:** 1.0.0  
**Last Updated:** February 1, 2026  
**Status:** Active and Enforced

