# Release Process

This repository uses [Release Please](https://github.com/googleapis/release-please) to automate semantic versioning, changelog generation, and GitHub releases for both frontend and backend components.

## How It Works

### Automatic Release Management

When you merge code to the `main` branch:

1. **Release Please Bot** analyzes commits since the last release
2. **Determines Version Bump** based on [Conventional Commits](https://www.conventionalcommits.org/)
3. **Creates/Updates a Release PR** with:
   - Updated version numbers (package.json for frontend, .csproj for backend)
   - Generated CHANGELOG entries
   - All changes since the last release
4. **When you merge the Release PR**:
   - Creates a GitHub Release with release notes
   - Tags the release (e.g., `frontend-v1.0.0`, `backend-v0.2.0`)
   - Publishes the changelog

### Separate Versioning

- **Frontend** and **Backend** are versioned independently
- Changes in `frontend/` directory trigger frontend releases
- Changes in `backend/` directory trigger backend releases
- Each component maintains its own CHANGELOG.md

### Current Versions

| Component | Version | Location |
|-----------|---------|----------|
| Frontend  | 0.1.0   | `frontend/package.json` |
| Backend   | 0.1.0   | `backend/GscTracking.Api/GscTracking.Api.csproj` |

## Commit Message Format

Use [Conventional Commits](https://www.conventionalcommits.org/) to ensure proper version bumping:

### Version Bumping Rules

| Commit Type | Version Bump | Example |
|-------------|--------------|---------|
| `fix:` | Patch (0.0.X) | `fix: resolve null reference in API` |
| `feat:` | Minor (0.X.0) | `feat: add user authentication` |
| `BREAKING CHANGE:` or `!` | Major (X.0.0) | `feat!: redesign API endpoints` |
| `chore:`, `docs:`, `style:` | No version bump | `docs: update README` |

### Examples

```bash
# Patch release (0.1.0 -> 0.1.1)
git commit -m "fix: resolve database connection timeout"

# Minor release (0.1.0 -> 0.2.0)
git commit -m "feat: add equipment tracking feature"

# Major release (0.1.0 -> 1.0.0)
git commit -m "feat!: redesign authentication system

BREAKING CHANGE: JWT token format has changed"
```

### Scoping to Frontend or Backend

Add a scope to clearly indicate which component is affected:

```bash
git commit -m "feat(frontend): add dark mode toggle"
git commit -m "fix(backend): resolve API validation error"
```

## Release Workflow

### Normal Development Flow

1. Create a feature branch from `main`
2. Make changes with conventional commits
3. Create a PR to `main`
4. Merge PR to `main`
5. Release Please creates/updates a release PR automatically
6. Review the release PR (check version bumps and changelog)
7. Merge the release PR to trigger the actual release

### Manual Release Process

If you need to manually trigger a release:

1. Ensure all changes are committed with conventional commit messages
2. The workflow runs automatically on push to `main`
3. Check the [Pull Requests](../../pulls) tab for the Release Please PR
4. Merge the Release Please PR when ready

## Configuration Files

- **`.github/workflows/release-please.yml`** - GitHub Actions workflow
- **`.github/release-please-config.json`** - Release Please configuration
- **`.github/.release-please-manifest.json`** - Current version tracking
- **`frontend/CHANGELOG.md`** - Frontend changelog
- **`backend/CHANGELOG.md`** - Backend changelog

## Troubleshooting

### Release PR Not Created

- Ensure commits follow conventional commit format
- Check that changes were made in `frontend/` or `backend/` directories
- Verify workflow permissions in repository settings

### Wrong Version Bump

- Review commit messages for correct conventional commit prefixes
- Use `feat!:` or add `BREAKING CHANGE:` footer for major version bumps
- Use `fix:` for patches and `feat:` for minor versions

### Multiple Components Changed

Release Please handles this automatically:
- If both frontend and backend have changes, it creates separate sections in the release PR
- Each component gets its own version bump based on its specific changes

## References

- [Release Please Documentation](https://github.com/googleapis/release-please)
- [Conventional Commits Specification](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
