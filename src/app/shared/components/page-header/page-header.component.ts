import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule],
  template: `
    <div class="fn-page-header">
      <div class="fn-page-header__left">
        <!-- Nervure accent -->
        <div class="fn-page-header__vein-group">
          <div class="fn-page-header__vein-line"></div>
          <div class="fn-page-header__vein-dot"></div>
        </div>
        <div>
          <h1 class="page-title">{{ title }}</h1>
          <p *ngIf="subtitle" class="fn-page-header__subtitle">{{ subtitle }}</p>
        </div>
      </div>
      <div class="fn-page-header__actions">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .fn-page-header {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      margin-bottom: 2rem;
      padding-bottom: 1.25rem;
      border-bottom: 1px solid var(--fn-mist);
      position: relative;

      /* bottom glimmer line */
      &::after {
        content: '';
        position: absolute;
        bottom: -1px; left: 0;
        width: 80px; height: 1px;
        background: linear-gradient(90deg, var(--fn-bio-trace), transparent);
      }
    }

    .fn-page-header__left {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .fn-page-header__vein-group {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 3px;
    }

    .fn-page-header__vein-line {
      width: 2px;
      height: 28px;
      background: linear-gradient(180deg, var(--fn-bio-trace) 0%, transparent 100%);
      border-radius: 1px;
    }

    .fn-page-header__vein-dot {
      width: 6px; height: 6px;
      border-radius: 50%;
      background: var(--fn-bio);
      box-shadow: 0 0 8px var(--fn-bio);
      animation: fn-pulse-bio 2.5s ease-in-out infinite;
    }

    .fn-page-header__subtitle {
      font-size: .8rem;
      color: var(--fn-text-dim);
      margin-top: .3rem;
      letter-spacing: .02em;
    }

    .fn-page-header__actions {
      display: flex;
      gap: .5rem;
      align-items: center;
    }
  `],
})
export class PageHeaderComponent {
  @Input() title = '';
  @Input() subtitle = '';
}
