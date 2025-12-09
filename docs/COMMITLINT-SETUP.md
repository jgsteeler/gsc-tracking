# Commitlint Setup Documentation

This document explains the commitlint setup implemented in this repository to enforce Conventional Commits format.

## Why This Was Needed

**Problem:** Release Please requires all commits to follow the Conventional Commits specification to:
- Automatically determine version bumps
- Generate accurate changelogs
- Create semantic releases

Copilot agent was creating commits like "Initial plan" which didn't follow this format, breaking the automated release process.

**Solution:** Implement automatic validation of commit messages at multiple levels.

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

### 2. Husky (`.husky/`)

**What:** Git hooks manager that runs scripts on git events.

**Setup:**
- Initialized with `npx husky init`
- Creates `.husky/` directory with hooks
- Automatically configured on `npm install` via `prepare` script

**commit-msg Hook (`.husky/commit-msg`):**
```bash
npx --no -- commitlint --edit "$1"
```

This hook runs on every commit attempt and validates the message before allowing the commit.

### 3. Git Commit Template (`.gitmessage`)

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

### 4. GitHub Actions Workflows

#### `.github/workflows/commitlint.yml`

**What:** Validates all commit messages in pull requests.

**When It Runs:**
- On pull request to main
- On push to main

**What It Does:**
1. Checks out code
2. Installs dependencies
3. Runs commitlint on all commits in the PR
4. Fails CI if any commit doesn't follow format

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

### 5. Documentation

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

2. **Install dependencies:**
   ```bash
   npm install
   ```
   This automatically:
   - Installs commitlint and husky
   - Sets up git hooks
   - Configures commit-msg hook

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

2. **Install new dependencies:**
   ```bash
   npm install
   ```

3. **Verify setup:**
   ```bash
   # Check that husky is configured
   ls .husky/commit-msg
   
   # Test commitlint
   echo "feat(test): test message" | npx commitlint
   ```

## How It Works

### Local Commit Flow

```
Developer runs: git commit -m "message"
       ↓
Husky intercepts the commit
       ↓
Runs .husky/commit-msg hook
       ↓
Commitlint validates message
       ↓
   Valid? ────Yes──→ Commit succeeds ✅
     ↓
    No
     ↓
Commit rejected with error ❌
(Shows what's wrong and how to fix)
```

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
- Invalid commits caught before push
- Clear error messages guide developers
- No broken commits in history

## Troubleshooting

### "Command not found: commitlint"

**Solution:** Install dependencies
```bash
npm install
```

### Husky hook not running

**Solution:** Reinstall husky
```bash
rm -rf .husky
npm install
```

### Want to bypass the hook (EMERGENCY ONLY)

**⚠️ Warning:** This breaks the automated release process. Only use in emergencies.

```bash
git commit -m "message" --no-verify
```

Then immediately fix the commit message:
```bash
git commit --amend -m "feat(scope): proper message"
```

### CI failing on commits

1. Check the commit messages in your PR
2. Each must follow: `type(scope): description`
3. Fix any invalid commits:
   ```bash
   git rebase -i HEAD~N  # N = number of commits to fix
   # Change 'pick' to 'reword' for commits to fix
   # Save and follow prompts to fix messages
   git push --force-with-lease
   ```

## Maintenance

### Updating Commitlint

```bash
npm update @commitlint/cli @commitlint/config-conventional
```

### Updating Husky

```bash
npm update husky
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

The validation happens at three levels:
1. **Local:** Git hook blocks bad commits
2. **PR Title:** GitHub Action validates PR title
3. **PR Commits:** GitHub Action validates all commits

This multi-layered approach ensures no non-conventional commits make it into the repository.
