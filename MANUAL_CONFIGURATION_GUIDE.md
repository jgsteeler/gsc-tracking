# Manual Repository Configuration Guide

Some tasks require repository admin access and must be performed manually:

## 1. Branch Protection Rules
- Go to Settings > Branches > Add rule for `main`.
- Require pull request reviews before merging.
- Require status checks to pass before merging.
- Require signed commits (optional).
- Restrict who can push to matching branches.

## 2. GitHub Discussions Setup
- Go to Settings > Features > Discussions > Enable Discussions.
- Create categories for Q&A, Ideas, and Announcements.

## 3. Repository Settings
- Set repository description and topics.
- Set default branch to `main`.
- Enable Dependabot alerts and updates.
- Configure repository visibility as needed.

## 4. Security Features
- Enable code scanning alerts (Settings > Security & analysis).
- Enable secret scanning.
- Set up vulnerability alerts.

## 5. Community Labels
- Create labels as described in `ISSUES.md` and `SETUP-INSTRUCTIONS.md`.

## 6. Project Board
- Create a project board for issue tracking and workflow.

## Reference
See `SETUP-INSTRUCTIONS.md` and `ISSUES.md` for full details.
