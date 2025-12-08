# Testing the Release Workflow

This guide provides step-by-step instructions to test the Release Please workflow.

## Prerequisites

- Repository must have "Read and write permissions" enabled for workflows
- "Allow GitHub Actions to create and approve pull requests" must be enabled
- Path: Settings > Actions > General > Workflow permissions

## Test Scenario 1: Frontend Patch Release

This tests a minor bug fix in the frontend (0.0.0 → 0.0.1).

```bash
# 1. Create a test branch
git checkout main
git pull
git checkout -b test-frontend-patch

# 2. Make a small change to frontend
echo "// Bug fix test" >> frontend/src/App.tsx

# 3. Commit with conventional commit format
git add frontend/src/App.tsx
git commit -m "fix(frontend): resolve minor UI rendering issue"

# 4. Push and create PR
git push origin test-frontend-patch
# Create PR on GitHub, then merge to main
```

**Expected Result:**
- Release Please creates a PR titled "chore(main): release frontend 0.0.1"
- CHANGELOG.md shows the bug fix
- package.json updated to 0.0.1

## Test Scenario 2: Backend Minor Release

This tests a new feature in the backend (0.1.0 → 0.2.0).

```bash
# 1. Create a test branch
git checkout main
git pull
git checkout -b test-backend-feature

# 2. Make a change to backend
echo "// New feature" >> backend/GscTracking.Api/Program.cs

# 3. Commit with conventional commit format
git add backend/GscTracking.Api/Program.cs
git commit -m "feat(backend): add health check endpoint"

# 4. Push and create PR
git push origin test-backend-feature
# Create PR on GitHub, then merge to main
```

**Expected Result:**
- Release Please creates a PR titled "chore(main): release backend 0.2.0"
- backend/CHANGELOG.md shows the new feature
- GscTracking.Api.csproj updated to 0.2.0

## Test Scenario 3: Breaking Change (Major Release)

This tests a breaking change in the frontend (0.0.1 → 1.0.0).

```bash
# 1. Create a test branch
git checkout main
git pull
git checkout -b test-frontend-breaking

# 2. Make a significant change
echo "// Breaking change" >> frontend/src/main.tsx

# 3. Commit with breaking change indicator
git add frontend/src/main.tsx
git commit -m "feat(frontend)!: redesign component API

BREAKING CHANGE: Component props have been restructured"

# 4. Push and create PR
git push origin test-frontend-breaking
# Create PR on GitHub, then merge to main
```

**Expected Result:**
- Release Please creates a PR titled "chore(main): release frontend 1.0.0"
- CHANGELOG.md prominently shows BREAKING CHANGE
- package.json updated to 1.0.0

## Test Scenario 4: Both Frontend and Backend Changes

This tests simultaneous releases for both components.

```bash
# 1. Create a test branch
git checkout main
git pull
git checkout -b test-both-changes

# 2. Make changes to both
echo "// Frontend fix" >> frontend/src/App.tsx
echo "// Backend feature" >> backend/GscTracking.Api/Program.cs

# 3. Commit both changes
git add frontend/src/App.tsx backend/GscTracking.Api/Program.cs
git commit -m "fix(frontend): improve error handling

feat(backend): add logging middleware"

# 4. Push and create PR
git push origin test-both-changes
# Create PR on GitHub, then merge to main
```

**Expected Result:**
- Release Please creates PRs for both components
- frontend: 1.0.1 (patch)
- backend: 0.3.0 (minor)
- Both CHANGELOGs updated

## Test Scenario 5: Non-Release Commit

This tests that documentation changes don't trigger releases.

```bash
# 1. Create a test branch
git checkout main
git pull
git checkout -b test-docs-only

# 2. Update documentation
echo "More details" >> README.md

# 3. Commit with non-release prefix
git add README.md
git commit -m "docs: update README with examples"

# 4. Push and create PR
git push origin test-docs-only
# Create PR on GitHub, then merge to main
```

**Expected Result:**
- NO Release PR created
- Documentation updated only

## Verifying Releases

After merging a Release PR:

1. **Check Tags**: Go to Repository > Tags
   - Should see new tags like `frontend-v0.0.1` or `backend-v0.2.0`

2. **Check Releases**: Go to Repository > Releases
   - Should see new release with generated notes
   - Includes link to compare changes

3. **Check CHANGELOGs**: 
   - `frontend/CHANGELOG.md` updated with release notes
   - `backend/CHANGELOG.md` updated with release notes

## Common Issues

### Release PR Not Created

**Problem**: No PR appears after merging to main

**Solutions**:
- Check commit message format (must be conventional)
- Verify changes are in `frontend/` or `backend/` directories
- Check workflow run logs in Actions tab
- Ensure workflow permissions are correct

### Wrong Version Number

**Problem**: Version bump is incorrect

**Solutions**:
- Review commit prefix:
  - `fix:` = patch (0.0.X)
  - `feat:` = minor (0.X.0)
  - `feat!:` or `BREAKING CHANGE:` = major (X.0.0)
- Check for typos in commit message

### Multiple Release PRs

**Problem**: Separate PRs for frontend and backend

**Solutions**:
- This is expected behavior in monorepo setup
- Each component is versioned independently
- Both PRs can be merged separately

## Rollback Process

If a release was created incorrectly:

1. **Delete the Git Tag**:
   ```bash
   git tag -d frontend-v0.0.1
   git push origin :refs/tags/frontend-v0.0.1
   ```

2. **Delete the GitHub Release**:
   - Go to Releases
   - Click the release
   - Delete release

3. **Revert version changes**:
   - Create PR to revert version in package.json or .csproj
   - Update .release-please-manifest.json

4. **Force new release**:
   - Merge corrected commits to main
   - Release Please will create new PR

## Continuous Testing

For ongoing validation:

1. Use conventional commits in all PRs
2. Monitor Release PRs before merging
3. Verify CHANGELOG entries are accurate
4. Check that version bumps make sense
5. Test releases in staging environment before production

## Resources

- [Release Please Documentation](https://github.com/googleapis/release-please)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [Repository RELEASE.md](../RELEASE.md)
