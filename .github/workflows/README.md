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

## Deploy to Fly.io Workflow

The `deploy-flyio.yml` workflow deploys the backend API to Fly.io hosting platform.

### How It Works

1. **Triggered on**:
   - Push to `main` branch (production deployment)
   - Pull requests to `main` branch (staging deployment)
   - Manual trigger via workflow_dispatch

2. **Environments**:
   - **Staging**: `gsc-tracking-api-staging.fly.dev` (for PRs)
   - **Production**: `gsc-tracking-api.fly.dev` (for main branch)

3. **What it does**:
   - Deploys backend to Fly.io using `flyctl`
   - Automatically applies database migrations on startup
   - Runs automated smoke tests after deployment
   - Comments on PR with deployment information (staging only)

### Database Migrations

Database migrations are applied automatically when the backend application starts in non-development environments (staging and production).

**Implementation:**
```csharp
// In Program.cs
if (!app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
    }
}
```

**Benefits:**
- No manual migration steps required
- Migrations run before app accepts traffic
- Prevents app from starting if migrations fail
- Automatically applies pending migrations

**Local Development:**
Migrations are **not** run automatically in development. Use:
```bash
cd backend/GscTracking.Api
dotnet ef database update
```

### Smoke Tests

Automated smoke tests verify basic functionality after deployment.

**Backend Smoke Tests** (`backend/smoke-test.sh`):
- Tests API health endpoint (`/api/hello`)
- Verifies Swagger UI (staging only)
- Tests core API endpoints (customers, jobs)
- Validates database connectivity

**Running Manually:**
```bash
# Test staging
API_URL="https://gsc-tracking-api-staging.fly.dev" ./backend/smoke-test.sh

# Test production
API_URL="https://gsc-tracking-api.fly.dev" ./backend/smoke-test.sh
```

See [Smoke Tests Documentation](../../docs/SMOKE-TESTS.md) for detailed information.

### Required Secrets

Configure in GitHub repository settings (Settings → Secrets → Actions):
- `FLY_API_TOKEN` - Fly.io token for production deployments
- `STAGING_FLY_API_TOKEN` - Fly.io token for staging deployments

### Testing the Workflow

**Test staging deployment:**
```bash
# Make a backend change
git checkout -b test-deploy
echo "// Test change" >> backend/GscTracking.Api/Program.cs
git add backend/
git commit -m "feat(backend): test deployment workflow"
git push origin test-deploy

# Create PR to main - this triggers staging deployment
# Check PR comments for deployment status and links
```

**Test production deployment:**
```bash
# Merge PR to main - this triggers production deployment
# Check Actions tab for workflow progress
# Verify deployment at https://gsc-tracking-api.fly.dev/api/hello
```

### Troubleshooting

**Deployment Failures:**
1. Check workflow logs in GitHub Actions
2. Check Fly.io logs: `flyctl logs -a gsc-tracking-api`
3. Verify secrets are configured correctly

**Migration Issues:**
1. Check migration files are properly created
2. Test migration locally: `dotnet ef database update`
3. Review application logs for migration errors

**Smoke Test Failures:**
1. Verify endpoints manually
2. Check application logs for runtime errors
3. Increase stabilization wait time if needed
