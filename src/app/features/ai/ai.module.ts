import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { AiRoutingModule } from './ai-routing.module';
import { AiComponent } from './ai.component';

@NgModule({
  imports: [SharedModule, AiRoutingModule, AiComponent],
})
export class AiModule {}
