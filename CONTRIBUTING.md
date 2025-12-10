# Contributing to GSC Tracking

Thank you for your interest in contributing! Please read these guidelines to help us maintain a welcoming and productive environment.

## ⚠️ Required Reading

**BEFORE creating PRs, read [COMMIT_GUIDELINES.md](./COMMIT_GUIDELINES.md)**

PR titles MUST follow Conventional Commits format for automated releases. Individual commits following this format is recommended but not required.

## How to Contribute

- **Fork the repository** and create your branch from `main`.
- **Write clear, descriptive commit messages** - Conventional Commits recommended, see [COMMIT_GUIDELINES.md](./COMMIT_GUIDELINES.md).
- **Open a pull request** with a conventional commit title and clear description. Reference related issues (e.g., `Fixes #27`).
- **Follow the code style and conventions** described in `.github/copilot-instructions.md` and project READMEs.
- **Add or update tests** for your changes.
- **Run linters and ensure all tests pass** before submitting.

## Setting Up Your Environment

1. Clone the repository
2. Install dependencies: `npm install` (at repository root - optional, for commitlint CLI tool)
3. Configure the commit template (optional but recommended):
   ```bash
   git config commit.template .gitmessage
   ```

## Issue Workflow

- Check existing issues before opening a new one.
- Use the provided issue templates for bugs, features, and infrastructure.
- Label your issues appropriately.
- Reference documentation and acceptance criteria in your issue description.

## Pull Request Process

- Ensure your PR passes CI/CD checks.
- Request reviews as needed.
- Address review comments promptly.
- Do not merge your own PR unless you are a project maintainer.

## Code of Conduct

All contributors must adhere to the [CODE_OF_CONDUCT.md](./CODE_OF_CONDUCT.md).

## License

By contributing, you agree your work will be released under the AGPL-3.0 license.
