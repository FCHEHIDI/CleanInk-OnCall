import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { KpiCardComponent } from './kpis/kpi-card.component';

interface KPI {
  label: string;
  value: string;
  delta: string;
  deltaPositive: boolean;
  icon: string;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, PageHeaderComponent, KpiCardComponent],
  template: `
    <app-page-header
      title="Tableau de bord"
      subtitle="Vue d'ensemble des activités CleanInk OnCall"
    ></app-page-header>

    <!-- KPIs -->
    <div class="kpi-grid">
      <app-kpi-card *ngFor="let kpi of kpis" [kpi]="kpi"></app-kpi-card>
    </div>

    <!-- Recent Activity placeholder -->
    <div class="ci-card mt-6">
      <h2 class="section-title mb-4">Activité récente</h2>
      <p class="text-sm text-gray-400 text-center py-8">
        Les graphiques seront intégrés ici (Chart.js / ngx-charts).
      </p>
    </div>
  `,
  styles: [`
    .kpi-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
      gap: 1rem;
      margin-bottom: 1.5rem;
    }
    .mt-6 { margin-top: 1.5rem; }
    .mb-4 { margin-bottom: 1rem; }
  `],
})
export class DashboardComponent {
  kpis: KPI[] = [
    { label: 'Appels du jour',     value: '142',    delta: '+12%',  deltaPositive: true,  icon: 'call',         color: '#1E5AA8' },
    { label: 'Temps moyen',        value: '4m 32s', delta: '-8%',   deltaPositive: true,  icon: 'timer',        color: '#0A1F44' },
    { label: 'Factures en attente',value: '23',     delta: '+3',    deltaPositive: false, icon: 'receipt_long', color: '#EF4444' },
    { label: 'Agents actifs',      value: '8',      delta: 'Stable',deltaPositive: true,  icon: 'headset_mic',  color: '#22C55E' },
  ];
}
