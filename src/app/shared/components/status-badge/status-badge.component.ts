import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export type BadgeStatus = 'active' | 'inactive' | 'pending' | 'error' | 'success';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="ci-badge" [ngClass]="statusClass">
      <span class="ci-badge__dot"></span>
      {{ label }}
    </span>
  `,
  styles: [`
    .ci-badge {
      display: inline-flex;
      align-items: center;
      gap: 5px;
      padding: 2px 10px;
      border-radius: 999px;
      font-size: 0.75rem;
      font-weight: 500;
      text-transform: capitalize;
    }
    .ci-badge__dot {
      width: 6px; height: 6px;
      border-radius: 50%;
      background: currentColor;
    }
    .active  { background: #dcfce7; color: #16a34a; }
    .inactive{ background: #f1f5f9; color: #64748b; }
    .pending { background: #fef9c3; color: #ca8a04; }
    .error   { background: #fee2e2; color: #dc2626; }
    .success { background: #dcfce7; color: #16a34a; }
  `],
})
export class StatusBadgeComponent {
  @Input() status: BadgeStatus = 'inactive';
  @Input() label = '';

  get statusClass(): string {
    return this.status;
  }
}
