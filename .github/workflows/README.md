# GitHub Actions Workflows

## Release Please Workflow

The `release-please.yml` workflow automates semantic versioning, changelog generation, and GitHub releases.

### How It Works

1. **Triggered on**: Push to `main` branch
2. **What it does**:
   - Analyzes commit messages since last release
   - Determines version bumps based on Conventional Commits
   - Creates/updates a Release PR with version bumps and changelog
   - When Release PR is merged, creates a GitHub Release

### Testing the Workflow

To test the release workflow:

1. **Make a change with a conventional commit**:
   ```bash
   # For frontend changes
   git checkout -b test-frontend-release
   echo "// Test change" >> frontend/src/App.tsx
   git add frontend/src/App.tsx
   git commit -m "feat(frontend): add test feature"
   git push origin test-frontend-release
   ```

2. **Create and merge PR to main**:
   - Open PR from your test branch to `main`
   - Merge the PR

3. **Check for Release PR**:
   - Go to Pull Requests tab
   - Look for a PR titled "chore(main): release frontend 0.1.0"
   - Review the PR to see version bumps and changelog entries

4. **Merge Release PR**:
   - Merge the Release PR
   - Check the Releases page for a new release

### Commit Message Examples

```bash
# Patch release (0.1.0 -> 0.1.1)
git commit -m "fix(backend): resolve null reference exception"

# Minor release (0.1.0 -> 0.2.0)
git commit -m "feat(frontend): add user profile page"

# Major release (0.1.0 -> 1.0.0)
git commit -m "feat(backend)!: redesign API endpoints

BREAKING CHANGE: All endpoints now require authentication"

# No release (documentation, chores)
git commit -m "docs: update README"
git commit -m "chore: update dependencies"
```

### Monorepo Support

This workflow supports independent versioning for frontend and backend:

- Changes in `frontend/` trigger frontend releases
- Changes in `backend/` trigger backend releases
- Both can be released simultaneously if both have changes
- Each component maintains its own version and CHANGELOG

### Configuration Files

- **`release-please.yml`** - GitHub Actions workflow definition
- **`release-please-config.json`** - Release Please configuration
- **`.release-please-manifest.json`** - Current version tracking

### Troubleshooting

**No Release PR created:**
- Verify commits follow Conventional Commits format
- Check workflow run logs in Actions tab
- Ensure changes are in `frontend/` or `backend/` directories

**Wrong version bump:**
- Review commit message prefixes (`feat:`, `fix:`, etc.)
- For breaking changes, use `!` or `BREAKING CHANGE:` footer

**Workflow permissions error:**
- Go to Settings > Actions > General
- Under "Workflow permissions", ensure "Read and write permissions" is selected
- Enable "Allow GitHub Actions to create and approve pull requests"

## Docker Build and Push Workflow

The `docker-build.yml` workflow builds and pushes Docker images for frontend and backend services.

### How It Works

1. **Triggered on**: 
   - Push to `main` or `develop` branches
   - Pull requests to `main` branch

2. **Path-based Triggering**:
   - **Backend image**: Only builds when files in `backend/` directory or workflow file change
   - **Frontend image**: Only builds when files in `frontend/` directory or workflow file change
   - This optimization prevents unnecessary builds and saves CI/CD time

3. **What it does**:
   - Detects which paths have changed using `dorny/paths-filter` action
   - Conditionally runs backend build job if backend code changed
   - Conditionally runs frontend build job if frontend code changed
   - Builds Docker images using Docker Buildx
   - Pushes images to GitHub Container Registry (ghcr.io) on push events
   - Tags images with branch name, PR number, semver, and commit SHA

### Image Naming

- Backend: `ghcr.io/jgsteeler/gsc-tracking-backend`
- Frontend: `ghcr.io/jgsteeler/gsc-tracking-frontend`

### Testing the Workflow

**Test backend-only build:**
```bash
# Make a backend change
git checkout -b test-backend-docker
echo "// Test change" >> backend/GscTracking.Api/Program.cs
git add backend/
git commit -m "test(backend): trigger backend Docker build"
git push origin test-backend-docker
```

**Test frontend-only build:**
```bash
# Make a frontend change
git checkout -b test-frontend-docker
echo "// Test change" >> frontend/src/App.tsx
git add frontend/
git commit -m "test(frontend): trigger frontend Docker build"
git push origin test-frontend-docker
```

**Expected behavior:**
- Backend changes trigger only `build-backend` job
- Frontend changes trigger only `build-frontend` job
- Changes to workflow file trigger both jobs
- Changes to docs/other files trigger neither job

### Configuration

The workflow uses these key features:
- **Path filtering**: `dorny/paths-filter@v3` detects changed paths
- **Conditional execution**: Jobs run only if relevant paths changed
- **Docker Buildx**: For multi-platform builds and caching
- **GitHub Actions cache**: Speeds up builds with layer caching

## Future Workflows

Additional workflows can be added here for:
- Testing
- Linting
- Deployment
