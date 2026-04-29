import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService, User } from '../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatBadgeModule,
    MatDividerModule,
  ],
  template: `
    <header class="ci-header">
      <!-- Burger menu -->
      <button mat-icon-button class="ci-header__burger" (click)="toggleSidebar.emit()">
        <mat-icon>menu</mat-icon>
      </button>

      <!-- Spacer -->
      <div class="ci-header__spacer"></div>

      <!-- Actions -->
      <div class="ci-header__actions">

        <!-- Notifications -->
        <button mat-icon-button [matBadge]="3" matBadgeColor="warn" matTooltip="Notifications">
          <mat-icon>notifications_none</mat-icon>
        </button>

        <!-- User menu -->
        <button mat-button [matMenuTriggerFor]="userMenu" class="ci-header__user">
          <div class="ci-header__avatar" *ngIf="user">
            {{ initials }}
          </div>
          <span class="ci-header__username">{{ user?.name }}</span>
          <mat-icon>expand_more</mat-icon>
        </button>

        <mat-menu #userMenu="matMenu" xPosition="before">
          <button mat-menu-item>
            <mat-icon>person_outline</mat-icon>
            <span>Mon profil</span>
          </button>
          <button mat-menu-item>
            <mat-icon>settings</mat-icon>
            <span>Paramètres</span>
          </button>
          <mat-divider></mat-divider>
          <button mat-menu-item (click)="logout()">
            <mat-icon>logout</mat-icon>
            <span>Déconnexion</span>
          </button>
        </mat-menu>
      </div>
    </header>
  `,
  styles: [`
    .ci-header {
      display: flex;
      align-items: center;
      height: 60px;
      padding: 0 1.5rem;
      background: var(--color-surface);
      border-bottom: 1px solid var(--color-border);
      box-shadow: var(--shadow-sm);
      position: sticky;
      top: 0;
      z-index: 100;
    }
    .ci-header__burger { color: var(--color-text); }
    .ci-header__spacer { flex: 1; }
    .ci-header__actions {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }
    .ci-header__user {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0 0.75rem;
    }
    .ci-header__avatar {
      width: 32px; height: 32px;
      border-radius: 50%;
      background: var(--color-primary);
      color: #fff;
      font-size: 0.75rem;
      font-weight: 600;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .ci-header__username {
      font-size: 0.875rem;
      font-weight: 500;
      color: var(--color-text);
    }
  `],
})
export class HeaderComponent {
  @Output() toggleSidebar = new EventEmitter<void>();

  constructor(private auth: AuthService) {}

  get user(): User | null {
    return this.auth.currentUser;
  }

  get initials(): string {
    const name = this.user?.name ?? '';
    return name
      .split(' ')
      .map((n) => n[0])
      .slice(0, 2)
      .join('')
      .toUpperCase();
  }

  logout(): void {
    this.auth.logout();
  }
}
