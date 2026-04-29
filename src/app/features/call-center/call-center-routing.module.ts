import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CallCenterComponent } from './call-center.component';

const routes: Routes = [
  { path: '', component: CallCenterComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CallCenterRoutingModule {}
