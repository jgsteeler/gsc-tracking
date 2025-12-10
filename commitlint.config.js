/**
 * Commitlint configuration for conventional commits
 * This enforces the Conventional Commits specification for all commits
 * 
 * Format: <type>(<scope>): <subject>
 * 
 * Types: feat, fix, docs, style, refactor, perf, test, build, ci, chore, revert
 * Scope: optional, can be any descriptive scope (customer, api, ci, infra, db, etc.)
 * 
 * Examples:
 * - feat(customer): add customer search functionality
 * - fix(api): resolve null reference in job controller
 * - docs(readme): update setup instructions
 * - chore(ci): upgrade commitlint to 9.0.1
 * - feat(infra): add Docker support
 */

module.exports = {
  extends: ['@commitlint/config-conventional'],
  rules: {
    // Ensure type is one of the allowed types
    'type-enum': [
      2,
      'always',
      [
        'feat',     // New feature
        'fix',      // Bug fix
        'docs',     // Documentation only changes
        'style',    // Code style changes (formatting, no logic change)
        'refactor', // Code restructuring (no behavior change)
        'perf',     // Performance improvements
        'test',     // Adding or updating tests
        'build',    // Build system or external dependency changes
        'ci',       // CI/CD configuration changes
        'chore',    // Other changes (tooling, configs, etc.)
        'revert',   // Reverts a previous commit
      ],
    ],
    // Ensure type is lowercase
    'type-case': [2, 'always', 'lower-case'],
    // Ensure type is not empty
    'type-empty': [2, 'never'],
    // Ensure scope is lowercase when provided
    'scope-case': [2, 'always', 'lower-case'],
    // Ensure subject is not empty
    'subject-empty': [2, 'never'],
    // Ensure subject doesn't end with a period
    'subject-full-stop': [2, 'never', '.'],
    // Ensure subject starts with lowercase (imperative mood)
    'subject-case': [2, 'always', 'lower-case'],
    // Ensure header is not too long
    'header-max-length': [2, 'always', 72],
    // Ensure body has a blank line before it
    'body-leading-blank': [2, 'always'],
    // Ensure footer has a blank line before it
    'footer-leading-blank': [2, 'always'],
  },
};
