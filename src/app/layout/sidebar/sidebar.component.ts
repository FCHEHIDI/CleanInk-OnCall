import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterLinkActive } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';

interface NavItem {
  label: string;
  icon: string;
  route: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    RouterLinkActive,
    MatIconModule,
    MatTooltipModule,
    MatDividerModule,
  ],
  template: `
    <aside class="ci-sidebar" [class.ci-sidebar--collapsed]="collapsed">

      <!-- Brand / Logo -->
      <div class="ci-sidebar__brand">
        <div class="ci-sidebar__logo-wrap" [class.ci-sidebar__logo-wrap--sm]="collapsed">
          <img
            src="assets/images/CleanInkLogo.png"
            alt="CleanInk OnCall"
            class="ci-sidebar__logo"
            [class.ci-sidebar__logo--sm]="collapsed"
          />
        </div>
      </div>

      <mat-divider></mat-divider>

      <!-- Navigation -->
      <nav class="ci-sidebar__nav">
        <a
          *ngFor="let item of navItems"
          class="ci-sidebar__item"
          [routerLink]="item.route"
          routerLinkActive="ci-sidebar__item--active"
          [matTooltip]="collapsed ? item.label : ''"
          matTooltipPosition="right"
        >
          <mat-icon class="ci-sidebar__icon">{{ item.icon }}</mat-icon>
          <span class="ci-sidebar__label" *ngIf="!collapsed">{{ item.label }}</span>
        </a>
      </nav>

      <!-- Collapse toggle -->
      <div class="ci-sidebar__footer">
        <button class="ci-sidebar__collapse-btn" (click)="toggleCollapse.emit()">
          <mat-icon>{{ collapsed ? 'chevron_right' : 'chevron_left' }}</mat-icon>
        </button>
      </div>
    </aside>
  `,
  styles: [`
    .ci-sidebar {
      width: 240px;
      min-width: 240px;
      height: 100vh;
      background: var(--color-primary-dk);
      display: flex;
      flex-direction: column;
      transition: width 0.25s ease, min-width 0.25s ease;
      overflow: hidden;
    }

    .ci-sidebar--collapsed {
      width: 64px;
      min-width: 64px;
    }

    /* ── Brand ── */
    .ci-sidebar__brand {
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 1.25rem 1rem;
      min-height: 88px;
    }

    .ci-sidebar__logo-wrap {
      width: 130px;
      height: 130px;
      border-radius: 50%;
      overflow: hidden;
      flex-shrink: 0;
      transition: width 0.2s, height 0.2s;
    }

    .ci-sidebar__logo-wrap--sm {
      width: 46px;
      height: 46px;
    }

    .ci-sidebar__logo {
      width: 100%;
      height: 100%;
      object-fit: cover;
      display: block;
    }

    .ci-sidebar__brand-name {
      font-size: 0.95rem;
      font-weight: 700;
      color: #ffffff;
      white-space: nowrap;
      letter-spacing: 0.01em;
    }

    mat-divider { border-color: rgba(255,255,255,.12) !important; }

    /* ── Nav ── */
    .ci-sidebar__nav {
      flex: 1;
      padding: 0.75rem 0;
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .ci-sidebar__item {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.65rem 1.25rem;
      color: rgba(255,255,255,.65);
      text-decoration: none;
      border-radius: 0;
      transition: background 0.15s, color 0.15s;
      position: relative;
      white-space: nowrap;
    }

    .ci-sidebar__item:hover {
      background: rgba(255,255,255,.08);
      color: #fff;
    }

    .ci-sidebar__item--active {
      background: rgba(255,255,255,.12);
      color: #fff;
    }

    .ci-sidebar__item--active::before {
      content: '';
      position: absolute;
      left: 0; top: 0; bottom: 0;
      width: 3px;
      background: #3B82F6;
      border-radius: 0 2px 2px 0;
    }

    .ci-sidebar__icon { font-size: 20px; width: 20px; height: 20px; }
    .ci-sidebar__label { font-size: 0.875rem; font-weight: 500; }

    /* ── Footer ── */
    .ci-sidebar__footer {
      padding: 0.75rem;
      border-top: 1px solid rgba(255,255,255,.12);
    }

    .ci-sidebar__collapse-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 100%;
      padding: 0.5rem;
      background: none;
      border: none;
      cursor: pointer;
      color: rgba(255,255,255,.5);
      border-radius: var(--radius-sm);
      transition: color 0.15s, background 0.15s;
    }
    .ci-sidebar__collapse-btn:hover {
      background: rgba(255,255,255,.08);
      color: #fff;
    }
  `],
})
export class SidebarComponent {
  @Input() collapsed = false;
  @Output() toggleCollapse = new EventEmitter<void>();

  navItems: NavItem[] = [
    { label: 'Tableau de bord', icon: 'dashboard',       route: '/dashboard'   },
    { label: 'Centre d\'appels', icon: 'headset_mic',    route: '/call-center' },
    { label: 'Facturation',      icon: 'receipt_long',   route: '/billing'     },
    { label: 'Administration',   icon: 'admin_panel_settings', route: '/admin' },
  ];
}
