import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-kpi-card',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <div class="fn-kpi" [class.fn-kpi--positive]="kpi.deltaPositive" [class.fn-kpi--negative]="!kpi.deltaPositive">
      <!-- Top row -->
      <div class="fn-kpi__top">
        <span class="label-sm fn-kpi__label">{{ kpi.label }}</span>
        <div class="fn-kpi__icon-wrap">
          <mat-icon class="fn-kpi__icon">{{ kpi.icon }}</mat-icon>
          <div class="fn-kpi__icon-halo"></div>
        </div>
      </div>

      <!-- Value -->
      <div class="fn-kpi__value">{{ kpi.value }}</div>

      <!-- Delta -->
      <div class="fn-kpi__footer">
        <div class="fn-kpi__delta">
          <span class="fn-kpi__delta-arrow">{{ kpi.deltaPositive ? '↑' : '↓' }}</span>
          {{ kpi.delta }}
        </div>

        <!-- Sparkline nervure -->
        <svg class="fn-kpi__sparkline" viewBox="0 0 60 20" fill="none">
          <polyline
            [attr.points]="sparkPath"
            stroke-width="1"
            stroke-linecap="round"
            stroke-linejoin="round"
            fill="none"
          />
        </svg>
      </div>

      <!-- Bottom vein -->
      <div class="fn-kpi__vein-bar"></div>
    </div>
  `,
  styles: [`
    .fn-kpi {
      position: relative;
      background: var(--fn-velvet-lift);
      border: 1px solid var(--fn-mist);
      border-radius: var(--fn-r-lg);
      padding: 1.25rem 1.25rem 1rem;
      overflow: hidden;
      cursor: default;
      animation: fn-bloom var(--fn-t-slow) var(--fn-ease) both;
      transition: transform var(--fn-t-mid) var(--fn-ease),
                  box-shadow var(--fn-t-mid) var(--fn-ease),
                  border-color var(--fn-t-mid) var(--fn-ease);

      /* Top highlight */
      &::before {
        content: '';
        position: absolute;
        top: 0; left: 12px; right: 12px;
        height: 1px;
        background: linear-gradient(90deg, transparent, var(--fn-vein-trace), transparent);
      }

      /* Background texture */
      &::after {
        content: '';
        position: absolute;
        top: -20px; right: -20px;
        width: 80px; height: 80px;
        border-radius: 50%;
        background: radial-gradient(circle, rgba(130,165,220,.05) 0%, transparent 70%);
        pointer-events: none;
      }

      &:hover {
        transform: translateY(-2px);
        box-shadow: var(--fn-shadow-lg), var(--fn-shadow-vein);
        border-color: rgba(100,140,220,.3);
      }
    }

    .fn-kpi--positive .fn-kpi__vein-bar { background: var(--fn-bio); box-shadow: 0 0 8px var(--fn-bio); }
    .fn-kpi--negative .fn-kpi__vein-bar { background: var(--fn-danger); box-shadow: 0 0 8px var(--fn-danger); }

    .fn-kpi--positive .fn-kpi__sparkline polyline { stroke: var(--fn-bio-trace); }
    .fn-kpi--negative .fn-kpi__sparkline polyline { stroke: rgba(232,94,94,.35); }

    .fn-kpi--positive .fn-kpi__delta { color: var(--fn-bio); }
    .fn-kpi--negative .fn-kpi__delta { color: var(--fn-danger); }

    .fn-kpi--positive .fn-kpi__icon { color: var(--fn-bio) !important; }
    .fn-kpi--negative .fn-kpi__icon { color: var(--fn-danger) !important; }

    .fn-kpi--positive .fn-kpi__icon-halo { background: var(--fn-bio-dim); }
    .fn-kpi--negative .fn-kpi__icon-halo { background: var(--fn-danger-dim); }

    /* Top row */
    .fn-kpi__top {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: .85rem;
    }

    .fn-kpi__label { display: block; }

    .fn-kpi__icon-wrap {
      position: relative;
      width: 32px; height: 32px;
    }

    .fn-kpi__icon {
      font-size: 18px !important;
      width: 18px !important;
      height: 18px !important;
      position: relative;
      z-index: 1;
      margin: 7px;
    }

    .fn-kpi__icon-halo {
      position: absolute;
      inset: 0;
      border-radius: var(--fn-r-sm);
      animation: fn-pulse-bio 3s ease-in-out infinite;
    }

    /* Value */
    .fn-kpi__value {
      font-family: var(--fn-font-title);
      font-size: 2rem;
      font-weight: 700;
      color: var(--fn-vein);
      line-height: 1;
      margin-bottom: .85rem;
      letter-spacing: -.01em;
    }

    /* Footer */
    .fn-kpi__footer {
      display: flex;
      align-items: center;
      justify-content: space-between;
    }

    .fn-kpi__delta {
      display: flex;
      align-items: center;
      gap: 3px;
      font-size: .75rem;
      font-weight: 600;
      letter-spacing: .03em;
    }

    .fn-kpi__delta-arrow {
      font-size: .8rem;
    }

    .fn-kpi__sparkline {
      width: 60px; height: 20px;
    }

    /* Bottom vein bar */
    .fn-kpi__vein-bar {
      position: absolute;
      bottom: 0; left: 0; right: 0;
      height: 2px;
      border-radius: 0 0 var(--fn-r-lg) var(--fn-r-lg);
    }
  `],
})
export class KpiCardComponent {
  @Input() kpi!: { label: string; value: string; delta: string; deltaPositive: boolean; icon: string; color: string };

  // Simple deterministic sparkline based on label hash
  get sparkPath(): string {
    const points: [number,number][] = [[0,15],[10,12],[20,10],[30,13],[40,8],[50,11],[60,9]];
    if (!this.kpi.deltaPositive) {
      return points.map(([x,y], i) => `${x},${20-y + i}`).join(' ');
    }
    return points.map(([x,y]) => `${x},${y}`).join(' ');
  }
}
