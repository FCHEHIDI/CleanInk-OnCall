import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  template: `
    <div class="empty-state">
      <mat-icon class="empty-state__icon">{{ icon }}</mat-icon>
      <h3 class="empty-state__title">{{ title }}</h3>
      <p class="empty-state__message">{{ message }}</p>
      <ng-content></ng-content>
    </div>
  `,
  styles: [`
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 4rem 2rem;
      text-align: center;
      color: var(--color-text-light);
    }
    .empty-state__icon {
      font-size: 3rem;
      width: 3rem;
      height: 3rem;
      margin-bottom: 1rem;
      color: var(--color-ledger);
    }
    .empty-state__title {
      font-size: 1.125rem;
      font-weight: 600;
      color: var(--color-text);
      margin-bottom: 0.5rem;
    }
    .empty-state__message {
      font-size: 0.875rem;
    }
  `],
})
export class EmptyStateComponent {
  @Input() icon = 'inbox';
  @Input() title = 'Aucun résultat';
  @Input() message = 'Il n\'y a rien à afficher pour le moment.';
}
