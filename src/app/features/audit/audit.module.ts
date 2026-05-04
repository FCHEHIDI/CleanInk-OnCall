import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { AuditRoutingModule } from './audit-routing.module';
import { AuditComponent } from './audit.component';

@NgModule({
  imports: [SharedModule, AuditRoutingModule, AuditComponent],
})
export class AuditModule {}
