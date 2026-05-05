# Changelog

All notable changes to CleanInk OnCall are documented here.
Format: [Semantic Versioning](https://semver.org/) — [Conventional Commits](https://www.conventionalcommits.org/)

## [0.1.0] — 2026-05-05

### ✨ Features

- **Domain** — Clean Architecture domain layer: DDD aggregates, value objects, domain events
- **Application** — CQRS application layer with MediatR use cases and healthcare RBAC policies
- **Infrastructure** — EF Core 9 + Postgres (schema-per-tenant), JWT HS256, BCrypt, multi-AI client (Claude, Azure OpenAI)
- **API** — ASP.NET Core 9 REST API with controllers, FluentValidation, Serilog, OpenTelemetry
- **Frontend** — Angular 17 standalone app wired to real API (calls, patients, encounters, billing)
- **Design** — Fractal Nocturne design system: dark velvet palette, fractal animations, Cormorant Garamond + Inter
- **UI** — Login redesign, per-room backgrounds, CIOC logo, Consultations sidebar section
- **Billing** — KPI cards with correct EUR formatting (cents → euros, fr-FR locale)
- **Docker** — 3-tier network segmentation (fn-edge / fn-app / fn-data), Traefik v3.1, dev overrides

### 🔧 Build System

- Conventional Commits enforced via commitlint + husky
- Automated changelog + SemVer tagging via release-it
- `.NET` version synchronized from `Directory.Build.props`

[0.1.0]: https://github.com/FCHEHIDI/CleanInk-OnCall/releases/tag/v0.1.0
