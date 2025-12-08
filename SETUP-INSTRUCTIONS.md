# Setup Instructions for GitHub Issues

This document provides step-by-step instructions for setting up GitHub issues, labels, and project management for the GSC Small Engine Repair Shop business management application.

## Files Created

This PR includes the following files:

1. **ISSUES.md** - Comprehensive list of 25 GitHub issues with full specifications
2. **.github/ISSUE_TEMPLATE/** - Issue templates (MVP Feature, Roadmap Feature, Infrastructure)
3. **.github/labels.yml** - Label configuration file with all predefined labels
4. **business-management-app-analysis.md** - Detailed project analysis and requirements (merged from main)

## Step 1: Create Labels

Labels help organize and categorize issues. Create them using the GitHub CLI or web interface.

### Using GitHub CLI (Recommended for bulk creation)

```bash
# Navigate to your repository
cd /path/to/gsc-tracking

# Create each label from the labels.yml file
# Priority Labels
gh label create "priority: critical" --description "Critical issues that block progress" --color "d73a4a"
gh label create "priority: high" --description "High priority issues" --color "ff9800"
gh label create "priority: medium" --description "Medium priority issues" --color "ffc107"
gh label create "priority: low" --description "Low priority issues" --color "8bc34a"

# Category Labels  
gh label create "mvp" --description "Minimum Viable Product features" --color "0052cc"
gh label create "roadmap" --description "Post-MVP roadmap features" --color "5319e7"
gh label create "infrastructure" --description "Infrastructure and DevOps tasks" --color "1d76db"
gh label create "enhancement" --description "New features or improvements" --color "a2eeef"
gh label create "bug" --description "Something isn't working" --color "d73a4a"
gh label create "documentation" --description "Documentation improvements" --color "0075ca"
gh label create "security" --description "Security-related issues" --color "ee0701"

# Technical Labels
gh label create "backend" --description "Backend/API work" --color "c5def5"
gh label create "frontend" --description "Frontend/UI work" --color "bfdadc"
gh label create "database" --description "Database related changes" --color "d4c5f9"
gh label create "devops" --description "DevOps and deployment" --color "fbca04"
gh label create "testing" --description "Testing related tasks" --color "7057ff"
gh label create "ui/ux" --description "User interface and experience" --color "e99695"
gh label create "api" --description "API design or changes" --color "0e8a16"
gh label create "integration" --description "Third-party integrations" --color "f9d0c4"
gh label create "mobile" --description "Mobile-specific features" --color "d876e3"
gh label create "analytics" --description "Analytics and reporting" --color "0075ca"
gh label create "ai/ml" --description "AI/Machine learning features" --color "006b75"
gh label create "storage" --description "File storage and cloud storage" --color "c2e0c6"
gh label create "architecture" --description "Architecture and design decisions" --color "5319e7"

# Status Labels
gh label create "status: blocked" --description "Blocked by another issue or external dependency" --color "d73a4a"
gh label create "status: in-progress" --description "Currently being worked on" --color "0e8a16"
gh label create "status: needs-review" --description "Needs code review or feedback" --color "fbca04"
gh label create "status: needs-testing" --description "Implementation complete, needs testing" --color "ff9800"
gh label create "good first issue" --description "Good for newcomers to the project" --color "7057ff"
gh label create "help wanted" --description "Extra attention needed from the community" --color "008672"

# Community Labels
gh label create "community" --description "Community contributions and engagement" --color "0e8a16"
gh label create "question" --description "Further information is requested" --color "d876e3"
gh label create "discussion" --description "Discussion or decision needed" --color "cc317c"
gh label create "duplicate" --description "This issue or pull request already exists" --color "cfd3d7"
gh label create "wontfix" --description "This will not be worked on" --color "ffffff"
gh label create "invalid" --description "This doesn't seem right" --color "e4e669"
```

### Using GitHub Web Interface

1. Navigate to https://github.com/jgsteeler/gsc-tracking/labels
2. Click "New label" for each label in `.github/labels.yml`
3. Enter the name, description, and color (without #)
4. Click "Create label"

## Step 2: Create Milestones

Milestones help group related issues and track progress.

### Using GitHub CLI

```bash
gh milestone create "MVP Release" --description "Core features for minimum viable product" --due-date 2026-03-01
gh milestone create "Phase 2 - Enhanced Features" --description "Post-MVP enhanced features (3-6 months)" --due-date 2026-06-01  
gh milestone create "Phase 3 - Advanced Features" --description "Advanced features (6-12 months)" --due-date 2026-12-01
gh milestone create "Infrastructure Setup" --description "Infrastructure and DevOps setup" --due-date 2026-02-01
```

### Using GitHub Web Interface

1. Navigate to https://github.com/jgsteeler/gsc-tracking/milestones
2. Click "New milestone"
3. Enter title, description, and optional due date
4. Click "Create milestone"

## Step 3: Create Issues

All 25 issues are detailed in `ISSUES.md`. You can create them manually or use a script.

### Manual Creation (Web Interface)

1. Navigate to https://github.com/jgsteeler/gsc-tracking/issues
2. Click "New issue"
3. Select the appropriate template (MVP Feature, Roadmap Feature, or Infrastructure)
4. Fill in the details from the corresponding issue in `ISSUES.md`
5. Add labels (see the Labels field in each issue spec)
6. Assign to milestone (optional)
7. Click "Submit new issue"

### Order of Creation

For best results, create issues in this order:

1. **Infrastructure issues first** (Issues #18-25)
   - These are foundational and may be blocking for other work
   
2. **MVP issues** (Issues #1-6)
   - Core features needed for minimum viable product
   
3. **Roadmap issues** (Issues #7-17)
   - Future enhancements and advanced features

### Using GitHub CLI (Bulk Creation)

Here's an example for creating the first MVP issue:

```bash
gh issue create \
  --title "[MVP] Implement Customer Management with CRUD Operations" \
  --body "$(cat <<'EOF'
## Description

Implement complete customer management functionality including Create, Read, Update, and Delete operations for customer records.

## Acceptance Criteria

- [ ] Database schema/model for customers with fields: name, email, phone, address, notes
- [ ] Backend API endpoints for customer CRUD operations
- [ ] Frontend UI for listing all customers with search/filter
- [ ] Frontend form for creating new customers with validation
- [ ] Frontend form for editing existing customers
- [ ] Delete customer functionality with confirmation dialog
- [ ] Form validation on both frontend (Zod) and backend
- [ ] Error handling and user feedback for all operations
- [ ] Responsive design using shadcn/ui components

## Technical Notes

- Use Entity Framework Core for database models and migrations
- Implement REST API endpoints: GET /api/customers, POST /api/customers, PUT /api/customers/:id, DELETE /api/customers/:id
- Use React Hook Form with Zod validation (frontend)
- Use FluentValidation or Data Annotations (backend .NET validation)
- Use shadcn/ui components: Table, Dialog, Form, Input, Button
- Consider soft delete vs hard delete

## Reference

See [business-management-app-analysis.md](./business-management-app-analysis.md#core-entities) for full context.
EOF
)" \
  --label "mvp,enhancement,backend,frontend,priority: critical" \
  --milestone "MVP Release"
```

**Note:** Creating all 25 issues this way can be tedious. Consider writing a script to automate it.

### Automated Script (Optional)

You can create a Node.js or Python script to parse `ISSUES.md` and create issues automatically using the GitHub API. Here's a basic structure:

```javascript
// create-issues.js
const { Octokit } = require("@octokit/rest");
const fs = require("fs");

const octokit = new Octokit({ auth: process.env.GITHUB_TOKEN });

async function createIssue(title, body, labels, milestone) {
  await octokit.issues.create({
    owner: "jgsteeler",
    repo: "gsc-tracking",
    title,
    body,
    labels,
    milestone
  });
}

// Parse ISSUES.md and create issues
// (Implementation details omitted for brevity)
```

## Step 4: Set Up Project Board

Create a project board for tracking progress.

### Using GitHub Web Interface

1. Navigate to https://github.com/jgsteeler/gsc-tracking/projects
2. Click "New project"
3. Choose "Board" layout
4. Name it "GSC Tracking Development"
5. Add columns:
   - **Backlog** - Issues not yet started
   - **To Do** - Ready to start
   - **In Progress** - Currently being worked on
   - **Review** - Awaiting review
   - **Done** - Completed
6. Add issues to the board as they're created

### Alternative: Use GitHub Projects (Beta)

The new GitHub Projects offers more features like custom fields, views, and automation.

1. Navigate to https://github.com/jgsteeler/gsc-tracking/projects
2. Click "New project" → "Table" or "Board"
3. Customize views and fields as needed
4. Add issues to the project

## Step 5: Configure Issue Templates

The issue templates are already created in `.github/ISSUE_TEMPLATE/`. They will automatically appear when users click "New issue".

Templates included:
- **MVP Feature** - For core MVP features
- **Roadmap Feature** - For post-MVP enhancements
- **Infrastructure & DevOps** - For infrastructure tasks

## Summary of Issues

- **Total Issues:** 25
- **MVP Features:** 6 issues (#1-6)
- **Roadmap Features:** 11 issues (#7-17)
- **Infrastructure & Setup:** 8 issues (#18-25)

## Next Steps

After setting up issues, labels, and milestones:

1. ✅ Review and prioritize issues
2. ✅ Assign issues to team members (if applicable)
3. ✅ Start with infrastructure setup (Database, Auth0, Hosting)
4. ✅ Begin MVP development
5. ✅ Set up regular stand-ups or check-ins
6. ✅ Use project board to track progress
7. ✅ Update issues as work progresses
8. ✅ Close issues when complete

## Resources

- [ISSUES.md](./ISSUES.md) - Full issue specifications
- [business-management-app-analysis.md](./business-management-app-analysis.md) - Project analysis and requirements
- [GitHub Issues Documentation](https://docs.github.com/en/issues)
- [GitHub Projects Documentation](https://docs.github.com/en/issues/planning-and-tracking-with-projects)
- [GitHub CLI Documentation](https://cli.github.com/)

## Questions or Issues?

If you encounter any problems or have questions:

1. Check the issue templates in `.github/ISSUE_TEMPLATE/`
2. Review the full specifications in `ISSUES.md`
3. Refer to the project analysis in `business-management-app-analysis.md`
4. Open a discussion in the repository

---

**Document Version:** 1.0  
**Last Updated:** 2025-12-08
