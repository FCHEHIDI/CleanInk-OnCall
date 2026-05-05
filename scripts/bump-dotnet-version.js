#!/usr/bin/env node
/**
 * scripts/bump-dotnet-version.js
 *
 * Appelé automatiquement par release-it après le bump de version (hook after:bump).
 * Met à jour backend/Directory.Build.props pour synchroniser la version .NET
 * avec la version du package.json (SemVer).
 *
 * Usage : node scripts/bump-dotnet-version.js <version>
 *   ex.  : node scripts/bump-dotnet-version.js 1.2.3
 */

'use strict';

const fs = require('fs');
const path = require('path');

const version = process.argv[2];

if (!version || !/^\d+\.\d+\.\d+/.test(version)) {
  console.error('❌  Usage: bump-dotnet-version.js <semver>  (ex: 1.2.3)');
  process.exit(1);
}

// Assembly version uses 4 parts — strip any pre-release suffix
const [major, minor, patch] = version.replace(/[-+].*$/, '').split('.').map(Number);
const assemblyVersion = `${major}.${minor}.${patch}.0`;

const propsPath = path.join(__dirname, '..', 'backend', 'Directory.Build.props');

if (!fs.existsSync(propsPath)) {
  console.error(`❌  File not found: ${propsPath}`);
  process.exit(1);
}

let content = fs.readFileSync(propsPath, 'utf8');

content = content
  .replace(/<Version>[^<]*<\/Version>/, `<Version>${version}</Version>`)
  .replace(/<AssemblyVersion>[^<]*<\/AssemblyVersion>/, `<AssemblyVersion>${assemblyVersion}</AssemblyVersion>`)
  .replace(/<FileVersion>[^<]*<\/FileVersion>/, `<FileVersion>${assemblyVersion}</FileVersion>`)
  .replace(/<InformationalVersion>[^<]*<\/InformationalVersion>/, `<InformationalVersion>${version}</InformationalVersion>`);

fs.writeFileSync(propsPath, content, 'utf8');
console.log(`✓  .NET version bumped to ${version} (assembly: ${assemblyVersion})`);
