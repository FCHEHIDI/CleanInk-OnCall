import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';
import { AdminGuard } from './core/guards/admin.guard';
import { LandingComponent } from './features/landing/landing.component';

export const routes: Routes = [
  {
    path: '',
    component: LandingComponent,
    pathMatch: 'full',
  },
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.module').then((m) => m.AuthModule),
  },
  {
    path: '',
    // Layout wrapper — toutes les routes protégées passent par le layout
    loadComponent: () =>
      import('./layout/layout.component').then((m) => m.LayoutComponent),
    canActivate: [AuthGuard],
    children: [
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/dashboard.module').then(
            (m) => m.DashboardModule
          ),
      },
      {
        path: 'call-center',
        loadChildren: () =>
          import('./features/call-center/call-center.module').then(
            (m) => m.CallCenterModule
          ),
      },
      {
        path: 'billing',
        loadChildren: () =>
          import('./features/billing/billing.module').then(
            (m) => m.BillingModule
          ),
      },
      {
        path: 'admin',
        canActivate: [AdminGuard],
        loadChildren: () =>
          import('./features/admin/admin.module').then((m) => m.AdminModule),
      },
      {
        path: 'patients',
        loadChildren: () =>
          import('./features/patients/patients.module').then((m) => m.PatientsModule),
      },
      {
        path: 'audit',
        canActivate: [AdminGuard],
        loadChildren: () =>
          import('./features/audit/audit.module').then((m) => m.AuditModule),
      },
      {
        path: 'ai',
        loadChildren: () =>
          import('./features/ai/ai.module').then((m) => m.AiModule),
      },
      {
        path: 'encounters',
        loadChildren: () =>
          import('./features/encounters/encounters.module').then((m) => m.EncountersModule),
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'dashboard',
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'top' })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
