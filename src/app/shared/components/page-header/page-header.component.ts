import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatButtonModule],
  template: `
    <div class="page-header">
      <div class="page-header__left">
        <h1 class="page-title">{{ title }}</h1>
        <p *ngIf="subtitle" class="page-header__subtitle">{{ subtitle }}</p>
      </div>
      <div class="page-header__actions">
        <ng-content></ng-content>
      </div>
    </div>
  `,
  styles: [`
    .page-header {
      display: flex;
      align-items: flex-start;
      justify-content: space-between;
      margin-bottom: 1.5rem;
      padding-bottom: 1rem;
      border-bottom: 1px solid var(--color-border);
    }
    .page-header__subtitle {
      font-size: 0.875rem;
      color: var(--color-text-light);
      margin-top: 0.25rem;
    }
    .page-header__actions {
      display: flex;
      gap: 0.5rem;
      align-items: center;
    }
  `],
})
export class PageHeaderComponent {
  @Input() title = '';
  @Input() subtitle = '';
}
