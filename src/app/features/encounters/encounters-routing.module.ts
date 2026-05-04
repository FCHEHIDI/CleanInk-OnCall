import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EncountersComponent } from './encounters.component';
import { EncounterDetailComponent } from './encounter-detail/encounter-detail.component';

const routes: Routes = [
  { path: '', component: EncountersComponent },
  { path: ':id', component: EncounterDetailComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class EncountersRoutingModule {}
