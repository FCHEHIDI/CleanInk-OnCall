import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CallCenterComponent } from './call-center.component';
import { CallDetailComponent } from './call-detail/call-detail.component';

const routes: Routes = [
  { path: '', component: CallCenterComponent },
  { path: ':id', component: CallDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CallCenterRoutingModule {}
