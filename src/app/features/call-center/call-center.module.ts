import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { CallCenterRoutingModule } from './call-center-routing.module';
import { CallCenterComponent } from './call-center.component';

@NgModule({
  imports: [SharedModule, CallCenterRoutingModule, CallCenterComponent],
})
export class CallCenterModule {}
