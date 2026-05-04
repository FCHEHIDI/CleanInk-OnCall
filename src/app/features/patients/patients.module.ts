import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { PatientsRoutingModule } from './patients-routing.module';
import { PatientsComponent } from './patients.component';
import { PatientDetailComponent } from './patient-detail/patient-detail.component';

@NgModule({
  imports: [SharedModule, PatientsRoutingModule, PatientsComponent, PatientDetailComponent],
})
export class PatientsModule {}
