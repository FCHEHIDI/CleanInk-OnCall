import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { BillingRoutingModule } from './billing-routing.module';
import { BillingComponent } from './billing.component';

@NgModule({
  imports: [SharedModule, BillingRoutingModule, BillingComponent],
})
export class BillingModule {}
