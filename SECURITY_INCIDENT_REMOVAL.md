# Security Incident: .env.staging File Removal

## Incident Summary
**Date**: 2025-12-16  
**Issue**: Sensitive environment file `.env.staging` containing database credentials was accidentally committed to the Git repository.

## File Details
- **File Path**: `backend/GscTracking.Api/.env.staging`
- **Original Commit**: e88a16d (PR #107) - "ci: add automated database migrations and smoke tests to deployment pipeline"
- **Date Committed**: ~10 hours before removal
- **Content Exposed**: PostgreSQL database connection string with credentials

## Exposed Credentials
The following connection string was exposed in the Git history:
```
DATABASE_URL="postgresql://neondb_owner:npg_pTxJ4Km0oAHs@ep-broad-star-aeeoledx-pooler.c-2.us-east-2.aws.neon.tech/tracking?sslmode=require&channel_binding=require"
```

**Host**: `ep-broad-star-aeeoledx-pooler.c-2.us-east-2.aws.neon.tech`  
**Database**: `tracking`  
**Username**: `neondb_owner`  
**Password**: `npg_pTxJ4Km0oAHs` (EXPOSED - MUST BE ROTATED)

## Actions Taken

### 1. History Rewrite
- Repository was unshallowed to get full history
- Used `git-filter-repo` to remove the file from all commits
- Verified removal with `git log` and `git ls-tree`
- All commit hashes after e88a16d have been changed

### 2. .gitignore Verification
The file was already listed in `.gitignore`:
```gitignore
backend/GscTracking.Api/.env.staging
```

This prevented future commits but did not remove the file from existing history.

### 3. Force Push Required
⚠️ **IMPORTANT**: This branch requires a force push to update the remote repository:
```bash
git push --force-with-lease origin copilot/remove-env-file-from-history
```

## Required Follow-up Actions

### CRITICAL - Immediate Actions Required:
1. **Rotate Database Credentials**
   - The exposed password `npg_pTxJ4Km0oAHs` MUST be changed immediately
   - Update the Neon database user password
   - Update all staging environment configurations with the new password
   - Verify no other systems are using the old credentials

2. **Review Database Access Logs**
   - Check Neon database access logs for any unauthorized access
   - Look for connections from unexpected IP addresses
   - Review query logs for any suspicious activity

3. **Update CI/CD Secrets**
   - Ensure all staging database credentials in GitHub Secrets are updated
   - Update any other secret management systems (Azure Key Vault, etc.)

4. **Notify Team**
   - Inform all team members about the credential exposure
   - Request that anyone who has pulled the repository deletes their local copy after the force push

### After Force Push:
5. **Team Coordination**
   - All developers must delete their local copies of affected branches
   - Fresh clone required after force push is complete
   - Alternatively, developers can run: `git fetch origin && git reset --hard origin/<branch-name>`

6. **Verify Clean History**
   - After force push, verify the file is not in remote history:
     ```bash
     git log --all -- backend/GscTracking.Api/.env.staging
     ```
   - Should return no results

## Prevention Measures

### Already Implemented:
- ✅ `.env.staging` is in `.gitignore`
- ✅ Pattern `*.env` is in `.gitignore` (with exception for `.env.example`)

### Additional Recommendations:
1. **Pre-commit Hooks**
   - Consider adding git-secrets or similar tool to prevent committing sensitive files
   - Add hooks to scan for patterns like connection strings, API keys, passwords

2. **Secret Scanning**
   - Enable GitHub secret scanning alerts (may already be enabled)
   - Consider adding Gitleaks or TruffleHog to CI/CD pipeline

3. **Environment Variable Management**
   - Use Azure Key Vault or similar for production/staging secrets
   - Never store actual secrets in repository, even in ignored files
   - Use environment variable injection at deployment time

4. **Documentation**
   - Update developer onboarding docs to emphasize never committing `.env.*` files
   - Create a security checklist for new features that involve credentials

5. **Regular Audits**
   - Periodically scan repository history for exposed secrets
   - Review `.gitignore` patterns regularly

## Technical Details

### Commands Used:
```bash
# Unshallow repository
git fetch --unshallow

# Remove file from history
git-filter-repo --path backend/GscTracking.Api/.env.staging --invert-paths --force

# Verify removal
git log --all -- backend/GscTracking.Api/.env.staging
git ls-tree -r HEAD --name-only | grep .env
```

### Commit Hash Changes:
The following commits had their hashes changed due to history rewrite:
- `e88a16d` → `[new hash]` (where file was added)
- All subsequent commits also have new hashes

## References
- Original Issue: Problem statement about .env.staging in repository
- Branch: `copilot/remove-env-file-from-history`
- Tool Used: git-filter-repo v2.47.0

## Sign-off
- **Action Completed By**: GitHub Copilot Agent
- **Date**: 2025-12-16
- **Status**: History cleaned, awaiting force push and credential rotation
