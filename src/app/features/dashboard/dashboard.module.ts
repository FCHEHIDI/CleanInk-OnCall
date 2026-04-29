import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
import { KpiCardComponent } from './kpis/kpi-card.component';

@NgModule({
  imports: [
    SharedModule,
    DashboardRoutingModule,
    DashboardComponent,
    KpiCardComponent,
  ],
})
export class DashboardModule {}
