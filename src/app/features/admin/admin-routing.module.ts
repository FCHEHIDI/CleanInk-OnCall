import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { AdminRolesComponent } from './roles/admin-roles.component';

const routes: Routes = [
  { path: '', component: AdminComponent },
  { path: 'roles', component: AdminRolesComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
