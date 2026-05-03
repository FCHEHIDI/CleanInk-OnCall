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
    <header class="fn-header">
      <!-- Burger -->
      <button mat-icon-button class="fn-header__burger" (click)="toggleSidebar.emit()">
        <mat-icon class="fn-header__burger-icon">segment</mat-icon>
      </button>

      <!-- Breadcrumb nervure -->
      <div class="fn-header__trail">
        <span class="fn-header__trail-dot"></span>
        <span class="fn-header__trail-label">CleanInk OnCall</span>
      </div>

      <div class="fn-header__spacer"></div>

      <!-- AI pulse indicator -->
      <div class="fn-header__ai-pulse">
        <div class="fn-header__ai-ring"></div>
        <div class="fn-header__ai-core"></div>
        <span class="fn-header__ai-label">AI</span>
      </div>

      <!-- Actions -->
      <div class="fn-header__actions">
        <button mat-icon-button class="fn-header__action-btn"
          [matBadge]="3" matBadgeSize="small">
          <mat-icon class="fn-header__action-icon">notifications_none</mat-icon>
        </button>

        <!-- User menu -->
        <button mat-button [matMenuTriggerFor]="userMenu" class="fn-header__user">
          <div class="fn-header__avatar" *ngIf="user">{{ initials }}</div>
          <span class="fn-header__username">{{ user?.name }}</span>
          <mat-icon class="fn-header__expand">expand_more</mat-icon>
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
    .fn-header {
      display: flex;
      align-items: center;
      height: 52px;
      padding: 0 1.25rem;
      background: rgba(6,10,25,.92);
      border-bottom: 1px solid rgba(28,47,90,.5);
      backdrop-filter: blur(12px);
      -webkit-backdrop-filter: blur(12px);
      position: sticky;
      top: 0;
      z-index: 100;
      gap: .75rem;
    }

    .fn-header__burger {
      color: var(--fn-text-dim) !important;
      transition: color var(--fn-t-fast) !important;
      &:hover { color: var(--fn-vein) !important; }
    }
    .fn-header__burger-icon { font-size: 18px !important; }

    /* Trail */
    .fn-header__trail {
      display: flex;
      align-items: center;
      gap: 7px;
    }
    .fn-header__trail-dot {
      width: 5px; height: 5px;
      border-radius: 50%;
      background: var(--fn-bio);
      box-shadow: 0 0 6px var(--fn-bio);
      animation: fn-pulse-bio 2.5s ease-in-out infinite;
    }
    .fn-header__trail-label {
      font-family: var(--fn-font-title);
      font-size: .82rem;
      letter-spacing: .1em;
      color: var(--fn-text-dim);
      text-transform: uppercase;
    }

    /* Spacer */
    .fn-header__spacer { flex: 1; }

    /* AI pulse */
    .fn-header__ai-pulse {
      display: flex;
      align-items: center;
      gap: 6px;
      padding: 4px 10px;
      border: 1px solid var(--fn-bio-dim);
      border-radius: var(--fn-r-pill);
      background: var(--fn-bio-glow);
    }
    .fn-header__ai-ring {
      width: 12px; height: 12px;
      border-radius: 50%;
      border: 1px solid var(--fn-bio-trace);
      animation: fn-halo-rotate 3s linear infinite;
      flex-shrink: 0;
    }
    .fn-header__ai-core {
      position: absolute;
      width: 4px; height: 4px;
      border-radius: 50%;
      background: var(--fn-bio);
      margin-left: 4px;
      box-shadow: 0 0 4px var(--fn-bio);
    }
    .fn-header__ai-label {
      font-size: .65rem;
      font-weight: 600;
      letter-spacing: .12em;
      color: var(--fn-bio);
      text-transform: uppercase;
    }

    /* Actions */
    .fn-header__actions {
      display: flex;
      align-items: center;
      gap: .25rem;
    }
    .fn-header__action-btn { color: var(--fn-text-dim) !important; }
    .fn-header__action-btn:hover { color: var(--fn-vein) !important; }
    .fn-header__action-icon { font-size: 18px !important; }

    /* User */
    .fn-header__user {
      display: flex;
      align-items: center;
      gap: 7px;
      padding: 0 .65rem !important;
      border: 1px solid transparent;
      border-radius: var(--fn-r-pill) !important;
      transition: all var(--fn-t-fast) var(--fn-ease) !important;
      color: var(--fn-text-mid) !important;
      line-height: 1 !important;

      &:hover {
        border-color: var(--fn-mist) !important;
        background: var(--fn-velvet-high) !important;
        color: var(--fn-vein) !important;
      }
    }

    .fn-header__avatar {
      width: 26px; height: 26px;
      border-radius: 50%;
      background: var(--fn-bio-dim);
      border: 1px solid var(--fn-bio-trace);
      color: var(--fn-bio);
      font-size: .68rem;
      font-weight: 700;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .fn-header__username {
      font-size: .78rem;
      font-weight: 500;
      color: inherit;
      letter-spacing: .02em;
    }
    .fn-header__expand {
      font-size: 14px !important;
      width: 14px !important;
      height: 14px !important;
      color: inherit !important;
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
