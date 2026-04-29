import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-kpi-card',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <div class="kpi-card ci-card">
      <div class="kpi-card__header">
        <span class="label-sm">{{ kpi.label }}</span>
        <div class="kpi-card__icon" [style.background]="kpi.color + '1A'">
          <mat-icon [style.color]="kpi.color">{{ kpi.icon }}</mat-icon>
        </div>
      </div>
      <div class="kpi-card__value">{{ kpi.value }}</div>
      <div class="kpi-card__delta" [class.positive]="kpi.deltaPositive" [class.negative]="!kpi.deltaPositive">
        {{ kpi.delta }}
      </div>
    </div>
  `,
  styles: [`
    .kpi-card { cursor: default; }
    .kpi-card__header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.75rem;
    }
    .kpi-card__icon {
      width: 38px; height: 38px;
      border-radius: var(--radius-md);
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .kpi-card__value {
      font-size: 1.75rem;
      font-weight: 700;
      color: var(--color-primary-dk);
      line-height: 1;
      margin-bottom: 0.5rem;
    }
    .kpi-card__delta {
      font-size: 0.8rem;
      font-weight: 500;
    }
    .positive { color: #16a34a; }
    .negative { color: #dc2626; }
  `],
})
export class KpiCardComponent {
  @Input() kpi!: { label: string; value: string; delta: string; deltaPositive: boolean; icon: string; color: string };
}
