# Testing Commitlint Setup

This document explains how to test that the commitlint configuration is working correctly.

## Prerequisites

- Node.js and npm installed
- Repository cloned and dependencies installed (`npm install` at repository root)
- Husky initialized (automatically done on `npm install`)

## Manual Testing

### Test 1: Valid Commit Message

Try to commit with a valid conventional commit message:

```bash
echo "# Test" >> README.md
git add README.md
git commit -m "docs(readme): add test content"
```

**Expected Result:** ✅ Commit succeeds

### Test 2: Invalid Commit Message (No Type)

Try to commit with an invalid message:

```bash
echo "# Test" >> README.md
git add README.md
git commit -m "Initial plan"
```

**Expected Result:** ❌ Commit fails with error:
```
⧗   input: Initial plan
✖   subject may not be empty [subject-empty]
✖   type may not be empty [type-empty]

✖   found 2 problems, 0 warnings
husky - commit-msg script failed (code 1)
```

### Test 3: Invalid Type

Try to commit with an invalid type:

```bash
git commit -m "invalid(test): some change"
```

**Expected Result:** ❌ Commit fails with error about invalid type

### Test 4: Subject with Period

Try to commit with a period at the end of the subject:

```bash
git commit -m "feat(test): add feature."
```

**Expected Result:** ❌ Commit fails with error about period in subject

### Test 5: Uppercase Subject

Try to commit with uppercase subject:

```bash
git commit -m "feat(test): Add feature"
```

**Expected Result:** ❌ Commit fails (subject must be lowercase)

## CLI Testing (Without Committing)

You can test commit messages without actually committing:

```bash
# Test a valid message
echo "feat(customer): add search" | npx commitlint

# Test an invalid message
echo "Initial plan" | npx commitlint
```

## GitHub Actions Testing

### PR Title Validation

When you create a pull request, the PR title is automatically validated:

1. Create a PR with a conventional commit title: `feat(customer): add search functionality`
   - **Expected:** ✅ Validation passes
2. Create a PR with a non-conventional title: `Add customer search`
   - **Expected:** ❌ Validation fails

### Commit Message Validation in PRs

All commits in a pull request are validated by the commitlint workflow:

1. Push a branch with conventional commits
   - **Expected:** ✅ CI passes
2. Push a branch with non-conventional commits
   - **Expected:** ❌ CI fails with detailed error messages

## Verifying Husky Installation

Check that husky is properly installed:

```bash
# Check that the husky directory exists
ls -la .husky/

# Should show:
# - commit-msg (executable)
# - _/ (husky scripts directory)

# Verify the commit-msg hook content
cat .husky/commit-msg

# Should contain:
# npx --no -- commitlint --edit "$1"
```

## Troubleshooting

### Hook Not Running

If commits are not being validated:

1. **Check husky installation:**
   ```bash
   npm install
   ```

2. **Verify git hooks are enabled:**
   ```bash
   git config core.hooksPath
   # Should show: .husky
   ```

3. **Make commit-msg executable:**
   ```bash
   chmod +x .husky/commit-msg
   ```

### Commitlint Not Found

If you see "command not found" errors:

```bash
# Install dependencies
npm install

# Or install commitlint globally (not recommended)
npm install -g @commitlint/cli @commitlint/config-conventional
```

### Want to Bypass (NOT Recommended)

If you absolutely must bypass the hook (emergency only):

```bash
git commit -m "message" --no-verify
```

**⚠️ WARNING:** This should NEVER be used for regular commits. It breaks the automated release process.

## Expected Behavior Summary

| Commit Message | Should Pass? |
|----------------|--------------|
| `feat(customer): add search` | ✅ Yes |
| `fix(api): resolve bug` | ✅ Yes |
| `docs: update readme` | ✅ Yes (scope optional) |
| `Initial plan` | ❌ No (missing type) |
| `Add feature` | ❌ No (missing type) |
| `feat: Add Feature` | ❌ No (subject must be lowercase) |
| `feat: add feature.` | ❌ No (no period at end) |
| `invalid(test): test` | ❌ No (invalid type) |
| `FEAT(test): test` | ❌ No (type must be lowercase) |

## Integration with Release Please

When commits follow the conventional format, Release Please will:

1. **Automatically determine version bump:**
   - `feat:` → Minor version bump (0.1.0 → 0.2.0)
   - `fix:` → Patch version bump (0.1.0 → 0.1.1)
   - `feat!:` or `BREAKING CHANGE:` → Major version bump (0.1.0 → 1.0.0)

2. **Generate changelog entries:**
   - Group commits by type
   - Include commit descriptions
   - Link to commits and issues

3. **Create release PRs:**
   - Automatically update version numbers
   - Update CHANGELOG.md
   - Create GitHub releases when merged

## Success Criteria

✅ The setup is working correctly if:
1. Invalid commit messages are rejected locally
2. The commit-msg hook runs on every commit
3. PR title validation passes/fails appropriately
4. Commit message validation in CI passes/fails appropriately
5. Release Please can parse all commits and create releases
