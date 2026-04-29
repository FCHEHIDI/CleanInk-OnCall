# CleanInk OnCall — Frontend Angular

Squelette Angular 17 modulaire et scalable pour la plateforme de gestion **CleanInk OnCall**.

---

## Stack

| Technologie         | Rôle                                  |
|---------------------|---------------------------------------|
| Angular 17          | Framework SPA                         |
| Angular Material 17 | Composants UI                         |
| TailwindCSS 3       | Utilitaires CSS                       |
| RxJS 7              | Réactivité / streams                  |
| TypeScript 5.4      | Typage strict                         |

---

## Démarrage rapide

```bash
npm install
npm start          # http://localhost:4200
npm run build:prod # Build de production
```

---

## Architecture

```
src/app/
 ├── core/              # Singleton services, interceptors, guards
 │    ├── services/     # api, auth, config
 │    ├── interceptors/ # auth JWT Bearer
 │    └── guards/       # AuthGuard
 ├── shared/            # Composants réutilisables, modules Material
 │    └── components/   # StatusBadge, PageHeader, LoadingSpinner, EmptyState
 ├── layout/            # Shell applicatif (header + sidebar + router-outlet)
 └── features/          # Modules fonctionnels en lazy loading
      ├── auth/         # Login
      ├── dashboard/    # KPIs + activité récente
      ├── call-center/  # File d'attente & appels
      ├── billing/      # Factures & documents
      └── admin/        # Utilisateurs & rôles
```

---

## Palette de couleurs

| Token               | Valeur    | Usage                  |
|---------------------|-----------|------------------------|
| `--color-primary`   | `#1E5AA8` | CTA, liens actifs      |
| `--color-primary-dk`| `#0A1F44` | Sidebar, titres        |
| `--color-ledger`    | `#D9D9D9` | Bordures, séparateurs  |
| `--color-accent`    | `#3B82F6` | Indicateurs, badges    |

---

## Logo & Favicon

Le logo `CleanInkLogo.png` est référencé depuis `src/assets/images/CleanInkLogo.png`.  
Il est utilisé comme :
- **favicon** dans `src/index.html` (`<link rel="icon" type="image/png">`)
- **logo sidebar** (blanc inversé sur fond sombre via `filter: brightness(0) invert(1)`)
- **logo page de connexion** (couleur naturelle)

---

## Conventions

- Modules en **lazy loading** obligatoire.
- Composants unitaires en **standalone** (imports directs).
- Services injectés `providedIn: 'root'` sauf exceptions.
- Styles CSS-in-component pour les overrides locaux, global `styles.scss` pour les tokens.
- Typage strict TypeScript — pas de `any`.
