// commitlint.config.js — règles pour les messages de commit (Conventional Commits)
// Docs : https://commitlint.js.org
//
// Format attendu : <type>(<scope>): <subject>
//   feat(billing): add invoice PDF export
//   fix(auth): handle expired JWT refresh token
//   chore: bump dependencies
//
// Types autorisés :
//   feat     → nouvelle fonctionnalité  (bump MINOR)
//   fix      → correction de bug        (bump PATCH)
//   perf     → amélioration perf        (bump PATCH)
//   refactor → refactoring sans nouvelle feature ni bug
//   docs     → documentation uniquement
//   style    → formatage, espaces (pas de logique)
//   test     → ajout/modification de tests
//   build    → système de build, dépendances
//   ci       → configuration CI/CD
//   chore    → maintenance, tâches diverses
//   revert   → annule un commit précédent
//
// Scopes suggérés :
//   auth, billing, encounters, patients, calls, api, ui, db, docker, deps, infra

/** @type {import('@commitlint/types').UserConfig} */
module.exports = {
  extends: ['@commitlint/config-conventional'],
  rules: {
    'type-enum': [
      2,
      'always',
      ['feat', 'fix', 'perf', 'refactor', 'docs', 'style', 'test', 'build', 'ci', 'chore', 'revert'],
    ],
    'scope-case': [2, 'always', 'lower-case'],
    'subject-case': [2, 'never', ['sentence-case', 'start-case', 'pascal-case', 'upper-case']],
    'subject-max-length': [2, 'always', 100],
    'subject-empty': [2, 'never'],
    'subject-full-stop': [2, 'never', '.'],
    'header-max-length': [2, 'always', 120],
    'body-leading-blank': [1, 'always'],
    'footer-leading-blank': [1, 'always'],
  },
};
