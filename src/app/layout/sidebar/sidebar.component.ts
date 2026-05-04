import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterLinkActive } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../core/services/auth.service';

interface NavItem {
  label: string;
  salle: string;
  icon: string;
  route: string;
  adminOnly?: boolean;
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
    <aside class="fn-sidebar" [class.fn-sidebar--collapsed]="collapsed">

      <!-- Fractal brand -->
      <div class="fn-sidebar__brand">
        <div class="fn-sidebar__owl-wrap">
          <img
            src="assets/images/logo_cioc.png"
            alt="CleanInk OnCall"
            class="fn-sidebar__owl"
          />
          <div class="fn-sidebar__owl-halo"></div>
        </div>
        <div class="fn-sidebar__brand-text" *ngIf="!collapsed">
          <span class="fn-sidebar__brand-name">CleanInk</span>
          <span class="fn-sidebar__brand-sub">OnCall</span>
        </div>
      </div>

      <!-- Nervure divider -->
      <div class="fn-sidebar__divider">
        <svg width="100%" height="1" viewBox="0 0 200 1" preserveAspectRatio="none">
          <line x1="0" y1="0" x2="200" y2="0"
            stroke="rgba(200,222,255,.12)" stroke-width="1"
            stroke-dasharray="4 6"/>
        </svg>
      </div>

      <!-- Navigation -->
      <nav class="fn-sidebar__nav">
        <a
          *ngFor="let item of navItems; let i = index"
          class="fn-sidebar__item"
          [routerLink]="item.route"
          routerLinkActive="fn-sidebar__item--active"
          [matTooltip]="collapsed ? item.label : ''"
          matTooltipPosition="right"
          [style.animation-delay]="(i * 60) + 'ms'"
        >
          <!-- Active vein bar -->
          <span class="fn-sidebar__vein"></span>

          <!-- Icon -->
          <div class="fn-sidebar__icon-wrap">
            <mat-icon class="fn-sidebar__icon">{{ item.icon }}</mat-icon>
          </div>

          <!-- Labels -->
          <div class="fn-sidebar__labels" *ngIf="!collapsed">
            <span class="fn-sidebar__label">{{ item.label }}</span>
            <span class="fn-sidebar__salle">{{ item.salle }}</span>
          </div>
        </a>
      </nav>

      <!-- Bottom fractal motif -->
      <div class="fn-sidebar__motif" *ngIf="!collapsed">
        <svg viewBox="0 0 180 80" fill="none" xmlns="http://www.w3.org/2000/svg">
          <path d="M90 70 Q70 50 50 55 Q30 60 20 40 Q10 20 0 15"
            stroke="rgba(61,232,176,.12)" stroke-width="1" fill="none"/>
          <path d="M90 70 Q110 50 130 55 Q150 60 160 40 Q170 20 180 15"
            stroke="rgba(61,232,176,.12)" stroke-width="1" fill="none"/>
          <path d="M90 70 Q90 45 90 30 Q90 15 90 5"
            stroke="rgba(61,232,176,.15)" stroke-width="1" fill="none"/>
          <circle cx="90" cy="70" r="3" fill="rgba(61,232,176,.2)"/>
        </svg>
      </div>

      <!-- Collapse toggle -->
      <div class="fn-sidebar__footer">
        <button class="fn-sidebar__collapse-btn" (click)="toggleCollapse.emit()"
          [matTooltip]="collapsed ? 'Déployer' : ''" matTooltipPosition="right">
          <mat-icon class="fn-sidebar__chevron">
            {{ collapsed ? 'chevron_right' : 'chevron_left' }}
          </mat-icon>
        </button>
      </div>
    </aside>
  `,
  styles: [`
    /* ── Shell ── */
    .fn-sidebar {
      width: 220px;
      min-width: 220px;
      height: 100vh;
      background: linear-gradient(180deg, #0a1228 0%, #080e1f 100%);
      display: flex;
      flex-direction: column;
      transition: width var(--fn-t-mid) var(--fn-ease),
                  min-width var(--fn-t-mid) var(--fn-ease);
      overflow: hidden;
      position: relative;
      border-right: 1px solid rgba(28,47,90,.6);
      z-index: 10;

      /* ambient fractal background glow */
      &::before {
        content: '';
        position: absolute;
        bottom: 0; left: 50%;
        transform: translateX(-50%);
        width: 200px; height: 300px;
        background: radial-gradient(ellipse at 50% 100%,
          rgba(61,232,176,.06) 0%,
          transparent 70%);
        pointer-events: none;
      }
    }

    .fn-sidebar--collapsed {
      width: 60px;
      min-width: 60px;
    }

    /* ── Brand ── */
    .fn-sidebar__brand {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 1.25rem 1rem 1rem;
      min-height: 80px;
    }

    .fn-sidebar__owl-wrap {
      position: relative;
      flex-shrink: 0;
    }

    .fn-sidebar__owl {
      width: 48px;
      height: 48px;
      object-fit: contain;
      display: block;
      position: relative;
      z-index: 1;
      filter: drop-shadow(0 0 6px rgba(61,232,176,.35));
    }

    .fn-sidebar__owl-halo {
      position: absolute;
      inset: -4px;
      border-radius: 50%;
      background: radial-gradient(circle, rgba(61,232,176,.12) 0%, transparent 70%);
      animation: fn-pulse-vein 3.5s ease-in-out infinite;
    }

    .fn-sidebar__brand-text {
      display: flex;
      flex-direction: column;
      line-height: 1.2;
      overflow: hidden;
    }

    .fn-sidebar__brand-name {
      font-family: var(--fn-font-title);
      font-size: 1rem;
      font-weight: 600;
      color: var(--fn-vein);
      white-space: nowrap;
      letter-spacing: .04em;
    }

    .fn-sidebar__brand-sub {
      font-size: .68rem;
      font-weight: 400;
      color: var(--fn-text-dim);
      letter-spacing: .1em;
      text-transform: uppercase;
    }

    /* ── Divider ── */
    .fn-sidebar__divider {
      padding: 0 1rem;
      opacity: .8;
    }

    /* ── Nav ── */
    .fn-sidebar__nav {
      flex: 1;
      padding: .75rem 0;
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .fn-sidebar__item {
      display: flex;
      align-items: center;
      gap: .65rem;
      padding: .6rem .9rem;
      color: var(--fn-text-mid);
      text-decoration: none;
      position: relative;
      transition: background var(--fn-t-fast) var(--fn-ease),
                  color var(--fn-t-fast) var(--fn-ease);
      animation: fn-branch-in var(--fn-t-slow) var(--fn-ease) both;
      border-radius: 0 var(--fn-r-md) var(--fn-r-md) 0;
      margin-right: .5rem;

      &:hover {
        background: rgba(26,38,80,.5);
        color: var(--fn-vein);

        .fn-sidebar__icon { color: var(--fn-bio) !important; }
      }
    }

    .fn-sidebar__item--active {
      background: rgba(26,38,80,.7) !important;
      color: var(--fn-vein) !important;

      .fn-sidebar__vein {
        opacity: 1 !important;
        animation: fn-pulse-bio 2.5s ease-in-out infinite;
      }
      .fn-sidebar__icon { color: var(--fn-bio) !important; }
      .fn-sidebar__label { color: var(--fn-vein) !important; }
    }

    /* Vein bar (left accent) */
    .fn-sidebar__vein {
      position: absolute;
      left: 0; top: 20%; bottom: 20%;
      width: 2px;
      background: var(--fn-bio);
      border-radius: 0 1px 1px 0;
      opacity: 0;
      transition: opacity var(--fn-t-fast) var(--fn-ease);
      box-shadow: 0 0 6px var(--fn-bio);
    }

    /* Icon */
    .fn-sidebar__icon-wrap {
      width: 28px;
      height: 28px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }

    .fn-sidebar__icon {
      font-size: 18px !important;
      width: 18px !important;
      height: 18px !important;
      color: var(--fn-text-dim) !important;
      transition: color var(--fn-t-fast) var(--fn-ease) !important;
    }

    /* Labels */
    .fn-sidebar__labels {
      display: flex;
      flex-direction: column;
      line-height: 1.2;
      min-width: 0;
      overflow: hidden;
    }

    .fn-sidebar__label {
      font-size: .82rem;
      font-weight: 500;
      color: var(--fn-text);
      white-space: nowrap;
      letter-spacing: .01em;
    }

    .fn-sidebar__salle {
      font-size: .63rem;
      font-weight: 400;
      color: var(--fn-text-dim);
      white-space: nowrap;
      letter-spacing: .08em;
      text-transform: uppercase;
    }

    /* ── Motif fractal ── */
    .fn-sidebar__motif {
      padding: 0 1.5rem .5rem;
      opacity: .6;
    }

    /* ── Footer ── */
    .fn-sidebar__footer {
      padding: .65rem;
      border-top: 1px solid rgba(28,47,90,.5);
    }

    .fn-sidebar__collapse-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 100%;
      padding: .45rem;
      background: none;
      border: 1px solid transparent;
      cursor: pointer;
      color: var(--fn-text-dim);
      border-radius: var(--fn-r-sm);
      transition: all var(--fn-t-fast) var(--fn-ease);

      &:hover {
        background: var(--fn-velvet-high);
        border-color: var(--fn-mist);
        color: var(--fn-vein);
      }
    }

    .fn-sidebar__chevron {
      font-size: 16px !important;
      width: 16px !important;
      height: 16px !important;
    }
  `],
})
export class SidebarComponent implements OnInit {
  @Input() collapsed = false;
  @Output() toggleCollapse = new EventEmitter<void>();

  private readonly allNavItems: NavItem[] = [
    { label: 'Tableau de bord',  salle: 'Salle de Contrôle',  icon: 'hub',                  route: '/dashboard'   },
    { label: "Centre d'appels",  salle: 'Salle des Appels',   icon: 'cell_tower',           route: '/call-center' },
    { label: 'Patients',         salle: 'Dossiers Cliniques', icon: 'folder_shared',        route: '/patients'    },
    { label: 'Consultations',    salle: 'Salle des Rencontres', icon: 'medical_services',   route: '/encounters'  },
    { label: 'Facturation',      salle: 'Salle des Factures', icon: 'receipt_long',         route: '/billing'     },
    { label: 'Assistant IA',     salle: 'Triage & Analyse',   icon: 'psychology',           route: '/ai'          },
    { label: 'Journal d\'audit', salle: 'Traçabilité RGPD',  icon: 'security',             route: '/audit',  adminOnly: true },
    { label: 'Administration',   salle: 'Accès restreint',    icon: 'admin_panel_settings', route: '/admin',  adminOnly: true },
  ];

  navItems: NavItem[] = [];

  constructor(private auth: AuthService) {}

  ngOnInit(): void {
    this.auth.currentUser$.subscribe((user) => {
      const isAdmin = user?.role?.toLowerCase() === 'admin';
      this.navItems = this.allNavItems.filter((item) => !item.adminOnly || isAdmin);
    });
  }
}
