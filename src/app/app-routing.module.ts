import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
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
        loadChildren: () =>
          import('./features/admin/admin.module').then((m) => m.AdminModule),
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
