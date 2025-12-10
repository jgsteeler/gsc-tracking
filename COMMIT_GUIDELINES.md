# Commit Message Guidelines

This project uses [Conventional Commits](https://www.conventionalcommits.org/) to enable automated releases with Release Please and maintain a clear, searchable Git history.

## ⚠️ IMPORTANT

**PR TITLES MUST follow the Conventional Commits format. This is required for Release Please to work properly.**

Individual commit messages following this format is a **best practice** for clear history, but not strictly enforced. However, it's highly recommended to use conventional commits for important changes that should appear in the changelog.

## Format

Every commit message **MUST** follow this structure:

```
<type>(<scope>): <description>

[optional body]

[optional footer(s)]
```

### Examples

```
feat(customer): add customer search functionality
fix(api): resolve null reference in job controller
docs(readme): update setup instructions
chore(deps): upgrade Entity Framework to 9.0.1
```

## Commit Types

| Type       | Description                                      | Version Bump |
|------------|--------------------------------------------------|--------------|
| `feat`     | New feature                                      | Minor        |
| `fix`      | Bug fix                                          | Patch        |
| `docs`     | Documentation only changes                       | None         |
| `style`    | Code style changes (formatting, no logic change) | None         |
| `refactor` | Code restructuring (no behavior change)          | None         |
| `perf`     | Performance improvements                         | Patch        |
| `test`     | Adding or updating tests                         | None         |
| `build`    | Build system or external dependency changes      | None         |
| `ci`       | CI/CD configuration changes                      | None         |
| `chore`    | Other changes (tooling, configs, etc.)           | None         |
| `revert`   | Reverts a previous commit                        | Depends      |

## Scope (Optional)

Scope specifies the area of the codebase affected. Use any scope that clearly describes the area:

**Common Examples:**
- `customer` - Customer management features
- `job` - Job tracking features
- `equipment` - Equipment management
- `expense` - Expense tracking
- `api` - API layer changes
- `db` - Database changes
- `auth` - Authentication/authorization
- `ui` - UI/UX changes
- `deps` - Dependency updates
- `ci` - CI/CD pipeline changes
- `infra` - Infrastructure changes
- `config` - Configuration changes

**Note:** Scopes are not restricted to this list. Use any descriptive scope that makes sense for your change.

### Examples with scope

```
feat(job): add job status filtering
fix(customer): correct email validation regex
refactor(api): extract common error handling
```

## Description Rules

1. **Use imperative mood**: "add feature" not "added feature" or "adds feature"
2. **Start with lowercase**: unless it's a proper noun
3. **No period at the end**
4. **Keep it under 72 characters**
5. **Be specific and descriptive**

### Good Examples

- `feat(auth): add JWT token refresh mechanism`
- `fix(job): prevent duplicate job creation`
- `docs(api): add OpenAPI documentation`

### Bad Examples

- ❌ `Added some stuff` (vague, wrong tense)
- ❌ `Fix bug` (not specific)
- ❌ `Updated files.` (period at end, not descriptive)
- ❌ `Initial plan` (not following format)

## Breaking Changes

For breaking changes that require major version bump:

1. Add `!` after type/scope: `feat(api)!: remove deprecated endpoints`
2. OR add `BREAKING CHANGE:` in the footer:

```
feat(api): restructure customer endpoint response

BREAKING CHANGE: Customer API now returns nested address object instead of flat fields.
Migration guide available in docs/migrations/v2.md
```

## Commit Body (Optional)

Use the body to explain:
- **Why** the change was made
- **What** problem it solves
- **How** it differs from previous behavior

### Example

```
fix(job): prevent race condition in status updates

Job status updates could be lost when multiple requests
arrived simultaneously. Added optimistic locking using
Entity Framework's RowVersion to ensure consistency.

Fixes #123
```

## Commit Footer

Use footers to:
- Reference issues: `Fixes #123`, `Closes #456`, `Refs #789`
- Note breaking changes: `BREAKING CHANGE: description`
- Add co-authors: `Co-authored-by: Name <email>`

## Automated Validation

This repository uses:

- **commitlint** - Validates commit messages against Conventional Commits
- **husky** - Git hooks to run commitlint on commit
- **Release Please** - Automatically creates releases based on commit messages

When you commit, your message will be automatically validated. If it doesn't follow the format, the commit will be rejected with an error message explaining what's wrong.

## Setting Up Your Environment

### Configure Git Commit Template (Optional but Recommended)

```bash
git config commit.template .gitmessage
```

This will show a helpful template every time you commit.

### Testing Commit Messages

You can test a commit message before committing:

```bash
echo "feat(customer): add search" | npx commitlint
```

## Common Scenarios

### Planning Commits

❌ **Don't do this:**
```
Initial plan
WIP
Update files
```

✅ **Do this instead:**
```
chore: add project structure outline
feat(docs): add implementation plan
chore: scaffold customer module
```

### Bug Fixes

❌ **Don't do this:**
```
Fix the bug
Fixed issue
Bug fix
```

✅ **Do this instead:**
```
fix(api): handle null customer reference
fix(ui): correct alignment in mobile view
fix(auth): resolve token expiration issue
```

### Documentation

❌ **Don't do this:**
```
Update docs
Documentation
README update
```

✅ **Do this instead:**
```
docs(readme): add Docker setup instructions
docs(api): document customer endpoints
docs(contributing): update PR guidelines
```

## Why This Matters

Conventional Commits enable:

1. **Automated Changelog Generation** - Release Please automatically creates changelogs
2. **Semantic Versioning** - Version numbers are automatically bumped based on commit types
3. **Clear Project History** - Anyone can understand what changed and why
4. **Easier Debugging** - Find when and why changes were introduced
5. **Better Collaboration** - Consistent format makes code review easier

## Questions?

- See the full [Conventional Commits Specification](https://www.conventionalcommits.org/)
- Check the [Release Please Documentation](https://github.com/googleapis/release-please)
- Review examples in the existing commit history

## Enforcement

These commit conventions are **strictly enforced** for:
- Automated changelog generation
- Semantic versioning with Release Please
- Clear project history and easier debugging

**All commits that don't follow this format will be rejected by the commit-msg hook.**
