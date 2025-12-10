# Commitlint Setup Documentation

This document explains the commitlint setup implemented in this repository to enforce Conventional Commits format.

## Why This Was Needed

**Problem:** Release Please works best when PR titles and important commits follow the Conventional Commits specification for:
- Automatically determining version bumps
- Generating accurate changelogs
- Creating semantic releases

**Solution:** Implement PR title validation and provide tools for developers who want to follow best practices with individual commits.

**Note:** Release Please does NOT require every commit to be conventional - only PR titles need to follow the format. However, conventional commits are recommended for clear history.

## Components

### 1. Commitlint (`commitlint.config.js`)

**What:** A tool that validates commit messages against Conventional Commits rules.

**Configuration:**
- Extends `@commitlint/config-conventional`
- Enforces lowercase subject
- Requires one of: feat, fix, docs, style, refactor, perf, test, build, ci, chore, revert
- Optional scopes: customer, job, equipment, expense, api, db, auth, ui, deps
- Max 72 character header
- No period at end of subject

**Example Valid Commits:**
```
feat(customer): add search functionality
fix(api): resolve null pointer exception
docs(readme): update installation steps
chore: update dependencies
```

**Example Invalid Commits:**
```
Initial plan           ❌ Missing type
Add feature           ❌ Missing type
feat: Add Feature     ❌ Subject must start with lowercase
feat: add feature.    ❌ No period at end
```

### 2. Git Commit Template (`.gitmessage`)

**What:** A template that appears when you run `git commit` (without -m).

**How to Enable:**
```bash
git config commit.template .gitmessage
```

**Benefits:**
- Shows commit format example
- Lists allowed types and scopes
- Provides helpful reminders
- Guides developers to write proper commits

### 3. GitHub Actions Workflow

#### `.github/workflows/validate-pr.yml`

**What:** Validates PR titles follow Conventional Commits.

**When It Runs:**
- When PR is opened
- When PR title is edited
- When PR is synchronized or reopened

**What It Does:**
- Uses `amannn/action-semantic-pull-request@v5`
- Validates PR title format
- Provides helpful error messages
- Allows same types and scopes as commitlint

### 4. Documentation

#### `COMMIT_GUIDELINES.md`
Comprehensive guide for developers with:
- Format explanation
- Complete type definitions
- Scope examples
- Good and bad examples
- Breaking change syntax
- Integration with Release Please

#### `docs/TESTING-COMMITLINT.md`
Testing procedures with:
- Manual testing steps
- CLI testing commands
- GitHub Actions testing
- Troubleshooting guide
- Expected behavior table

#### Updated Files
- `.github/copilot-instructions.md` - Added prominent warning at top
- `.github/PULL_REQUEST_TEMPLATE/default.md` - Added format reminder
- `CONTRIBUTING.md` - Added setup instructions
- `README.md` - Added reference to guidelines

## Installation and Setup

### For New Developers

1. **Clone the repository:**
   ```bash
   git clone https://github.com/jgsteeler/gsc-tracking.git
   cd gsc-tracking
   ```

2. **Install dependencies (optional):**
   ```bash
   npm install
   ```
   This installs commitlint for manual validation if needed.

3. **Configure commit template (optional but recommended):**
   ```bash
   git config commit.template .gitmessage
   ```

4. **Read the guidelines:**
   - See [COMMIT_GUIDELINES.md](../COMMIT_GUIDELINES.md) for complete guide

### For Existing Developers

If you already have the repository cloned:

1. **Pull the latest changes:**
   ```bash
   git pull origin main
   ```

2. **Verify PR title validation:**
   - Open a PR and check that the title follows Conventional Commits
   - CI will validate the PR title automatically

## How It Works

### PR Validation Flow

```
Developer opens PR
       ↓
GitHub Actions triggered
       ↓
Two workflows run in parallel:
       ↓
[validate-pr.yml]        [commitlint.yml]
Validates PR title       Validates all commits
       ↓                        ↓
   All valid? ←─────────────────
       ↓
    Yes ──→ CI passes ✅
       ↓
     No ──→ CI fails ❌
           (Shows specific errors)
```

## Benefits

### 1. Automated Release Management
- Release Please can parse all commits
- Automatic version bumping based on commit types
- Accurate changelog generation
- No manual release management needed

### 2. Clear Project History
- Consistent commit format
- Easy to understand what changed
- Simple to find when features were added or bugs were fixed
- Better for debugging

### 3. Better Collaboration
- Everyone follows the same format
- Code review is easier
- New contributors understand conventions quickly

### 4. Prevents Mistakes Early
- Invalid PR titles caught in CI
- Clear error messages guide developers
- Ensures Release Please works correctly

## Troubleshooting

### "Command not found: commitlint"

**Solution:** Install dependencies (if you want to use commitlint locally)
```bash
npm install
```

### PR Title Validation Failing

**Solution:** Update PR title to follow Conventional Commits format
- Example: `feat(customer): add search functionality`
- See COMMIT_GUIDELINES.md for format details

### Manual Commit Validation (Optional)

If you want to validate commits locally before pushing:

```bash
# Test a commit message
echo "feat(customer): add search" | npx commitlint

# Or test your last commit
npx commitlint --from HEAD~1
```

## Maintenance

### Updating Commitlint

```bash
npm update @commitlint/cli @commitlint/config-conventional
```

### Modifying Rules

Edit `commitlint.config.js` to:
- Add/remove types
- Add/remove scopes
- Change validation rules
- Adjust error messages

**Example:** Adding a new type
```javascript
'type-enum': [
  2,
  'always',
  [
    'feat', 'fix', 'docs', // ... existing
    'newtype', // Add new type here
  ],
],
```

## References

- [Conventional Commits Specification](https://www.conventionalcommits.org/)
- [Commitlint Documentation](https://commitlint.js.org/)
- [Husky Documentation](https://typicode.github.io/husky/)
- [Release Please Documentation](https://github.com/googleapis/release-please)

## Summary

This setup ensures all commits in the repository follow Conventional Commits, enabling:
- ✅ Automated releases with Release Please
- ✅ Accurate changelog generation
- ✅ Clear, searchable project history
- ✅ Better collaboration
- ✅ Early error detection

PR title validation ensures Release Please works correctly:
1. **PR Title:** GitHub Action validates PR title format (Required)
2. **Optional:** Developers can use commitlint locally for commit validation
3. **Best Practice:** Following Conventional Commits for important commits improves changelog quality
