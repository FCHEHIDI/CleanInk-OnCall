import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { KpiCardComponent } from './kpis/kpi-card.component';
import { MatIconModule } from '@angular/material/icon';

interface KPI {
  label: string;
  value: string;
  delta: string;
  deltaPositive: boolean;
  icon: string;
  color: string;
}

interface Activity {
  type: 'call' | 'invoice' | 'patient' | 'ai';
  label: string;
  detail: string;
  time: string;
  icon: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, PageHeaderComponent, KpiCardComponent, MatIconModule],
  template: `
    <app-page-header
      title="Salle de Contrôle"
      subtitle="Vue d'ensemble des activités nocturnes"
    ></app-page-header>

    <!-- KPI Grid -->
    <div class="fn-kpi-grid">
      <app-kpi-card *ngFor="let kpi of kpis" [kpi]="kpi"></app-kpi-card>
    </div>

    <!-- Lower section -->
    <div class="fn-dash-lower">

      <!-- Activity feed -->
      <div class="ci-card fn-activity">
        <div class="fn-activity__header">
          <div class="fn-activity__title-wrap">
            <div class="fn-activity__pulse"></div>
            <h2 class="section-title">Activité en direct</h2>
          </div>
          <span class="fn-activity__count fn-badge fn-badge--active">{{ activities.length }} événements</span>
        </div>

        <div class="fn-activity__list">
          <div *ngFor="let item of activities; let i = index"
            class="fn-activity__item"
            [style.animation-delay]="(i * 80) + 'ms'">

            <div class="fn-activity__icon-wrap" [ngClass]="'fn-activity__icon-wrap--' + item.type">
              <mat-icon class="fn-activity__icon">{{ item.icon }}</mat-icon>
            </div>

            <div class="fn-activity__body">
              <span class="fn-activity__label">{{ item.label }}</span>
              <span class="fn-activity__detail">{{ item.detail }}</span>
            </div>

            <span class="fn-activity__time">{{ item.time }}</span>
          </div>
        </div>
      </div>

      <!-- AI status panel -->
      <div class="ci-card fn-ai-panel">
        <div class="fn-ai-panel__header">
          <h2 class="section-title">Agents AI</h2>
        </div>

        <!-- Fractal organism -->
        <div class="fn-ai-panel__organism">
          <svg viewBox="0 0 200 160" fill="none" xmlns="http://www.w3.org/2000/svg"
            class="fn-ai-panel__svg">
            <!-- Root branches -->
            <path d="M100 130 Q100 100 100 80 Q100 60 100 40"
              stroke="rgba(61,232,176,.25)" stroke-width="1.5" fill="none"/>
            <path d="M100 80 Q80 65 60 60 Q45 55 30 50"
              stroke="rgba(61,232,176,.20)" stroke-width="1" fill="none"/>
            <path d="M100 80 Q120 65 140 60 Q155 55 170 50"
              stroke="rgba(61,232,176,.20)" stroke-width="1" fill="none"/>
            <!-- Sub-branches left -->
            <path d="M60 60 Q50 50 40 45" stroke="rgba(61,232,176,.14)" stroke-width=".8" fill="none"/>
            <path d="M60 60 Q55 55 45 65" stroke="rgba(61,232,176,.14)" stroke-width=".8" fill="none"/>
            <!-- Sub-branches right -->
            <path d="M140 60 Q150 50 160 45" stroke="rgba(61,232,176,.14)" stroke-width=".8" fill="none"/>
            <path d="M140 60 Q145 55 155 65" stroke="rgba(61,232,176,.14)" stroke-width=".8" fill="none"/>
            <!-- Root node -->
            <circle cx="100" cy="130" r="5" fill="rgba(61,232,176,.3)"
              stroke="rgba(61,232,176,.6)" stroke-width="1"/>
            <!-- Branch nodes -->
            <circle cx="100" cy="80" r="3" fill="rgba(61,232,176,.5)"/>
            <circle cx="60"  cy="60" r="2.5" fill="rgba(61,232,176,.4)"/>
            <circle cx="140" cy="60" r="2.5" fill="rgba(61,232,176,.4)"/>
            <circle cx="100" cy="40" r="4" fill="rgba(61,232,176,.6)"
              stroke="rgba(61,232,176,.3)" stroke-width="1">
              <animate attributeName="r" values="4;5;4" dur="2.5s" repeatCount="indefinite"/>
            </circle>
          </svg>
        </div>

        <!-- Agent list -->
        <div class="fn-ai-panel__agents">
          <div *ngFor="let agent of agents" class="fn-ai-panel__agent">
            <div class="fn-ai-panel__agent-dot"
              [class.fn-ai-panel__agent-dot--active]="agent.active"></div>
            <div class="fn-ai-panel__agent-info">
              <span class="fn-ai-panel__agent-name">{{ agent.name }}</span>
              <span class="fn-ai-panel__agent-role">{{ agent.role }}</span>
            </div>
            <span class="fn-badge" [class.fn-badge--active]="agent.active" [class.fn-badge--muted]="!agent.active">
              {{ agent.active ? 'Actif' : 'Veille' }}
            </span>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .fn-kpi-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(210px, 1fr));
      gap: 1rem;
      margin-bottom: 1.5rem;
    }

    .fn-dash-lower {
      display: grid;
      grid-template-columns: 1fr 340px;
      gap: 1rem;

      @media (max-width: 1100px) {
        grid-template-columns: 1fr;
      }
    }

    /* Activity */
    .fn-activity__header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 1.25rem;
    }
    .fn-activity__title-wrap {
      display: flex;
      align-items: center;
      gap: 8px;
    }
    .fn-activity__pulse {
      width: 8px; height: 8px;
      border-radius: 50%;
      background: var(--fn-bio);
      box-shadow: 0 0 8px var(--fn-bio);
      animation: fn-pulse-bio 2s ease-in-out infinite;
    }
    .fn-activity__count { cursor: default; }

    .fn-activity__list {
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .fn-activity__item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: .65rem .5rem;
      border-radius: var(--fn-r-md);
      border-bottom: 1px solid var(--fn-mist);
      animation: fn-branch-in var(--fn-t-slow) var(--fn-ease) both;
      transition: background var(--fn-t-fast) var(--fn-ease);

      &:last-child { border-bottom: none; }
      &:hover { background: var(--fn-velvet-high); }
    }

    .fn-activity__icon-wrap {
      width: 32px; height: 32px;
      border-radius: var(--fn-r-sm);
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }
    .fn-activity__icon-wrap--call    { background: rgba(61,232,176,.12); }
    .fn-activity__icon-wrap--invoice { background: rgba(232,168,61,.12); }
    .fn-activity__icon-wrap--patient { background: rgba(130,165,220,.12); }
    .fn-activity__icon-wrap--ai      { background: rgba(61,232,176,.08); border: 1px solid var(--fn-bio-trace); }

    .fn-activity__icon {
      font-size: 15px !important;
      width: 15px !important; height: 15px !important;
      color: var(--fn-text-mid) !important;
    }
    .fn-activity__icon-wrap--call    .fn-activity__icon { color: var(--fn-bio) !important; }
    .fn-activity__icon-wrap--invoice .fn-activity__icon { color: var(--fn-warn) !important; }
    .fn-activity__icon-wrap--ai      .fn-activity__icon { color: var(--fn-bio) !important; }

    .fn-activity__body {
      flex: 1;
      display: flex;
      flex-direction: column;
      gap: 1px;
      min-width: 0;
    }
    .fn-activity__label {
      font-size: .82rem;
      font-weight: 500;
      color: var(--fn-text);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    .fn-activity__detail {
      font-size: .72rem;
      color: var(--fn-text-dim);
    }
    .fn-activity__time {
      font-size: .7rem;
      color: var(--fn-text-dim);
      white-space: nowrap;
      font-family: var(--fn-font-mono);
    }

    /* AI panel */
    .fn-ai-panel__header { margin-bottom: .75rem; }

    .fn-ai-panel__organism {
      display: flex;
      justify-content: center;
      margin-bottom: .75rem;
    }
    .fn-ai-panel__svg {
      width: 180px; height: 140px;
      opacity: .85;
    }

    .fn-ai-panel__agents {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .fn-ai-panel__agent {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: .55rem .75rem;
      border-radius: var(--fn-r-md);
      border: 1px solid var(--fn-mist);
      background: var(--fn-velvet-mid);
      transition: background var(--fn-t-fast) var(--fn-ease);

      &:hover { background: var(--fn-velvet-high); }
    }
    .fn-ai-panel__agent-dot {
      width: 7px; height: 7px;
      border-radius: 50%;
      background: var(--fn-mist);
      flex-shrink: 0;
    }
    .fn-ai-panel__agent-dot--active {
      background: var(--fn-bio);
      box-shadow: 0 0 6px var(--fn-bio);
      animation: fn-pulse-bio 2.2s ease-in-out infinite;
    }
    .fn-ai-panel__agent-info {
      flex: 1;
      display: flex;
      flex-direction: column;
      gap: 1px;
    }
    .fn-ai-panel__agent-name {
      font-size: .8rem;
      font-weight: 500;
      color: var(--fn-text);
    }
    .fn-ai-panel__agent-role {
      font-size: .68rem;
      color: var(--fn-text-dim);
    }
  `],
})
export class DashboardComponent {
  kpis: KPI[] = [
    { label: 'Appels du jour',      value: '142',    delta: '+12%',  deltaPositive: true,  icon: 'cell_tower',   color: '#3de8b0' },
    { label: 'Temps de réponse',    value: '4m 32s', delta: '-8%',   deltaPositive: true,  icon: 'timer',        color: '#3de8b0' },
    { label: 'Factures en attente', value: '23',     delta: '+3',    deltaPositive: false, icon: 'receipt_long', color: '#e8a83d' },
    { label: 'Agents actifs',       value: '8',      delta: 'Stable',deltaPositive: true,  icon: 'hub',          color: '#3de8b0' },
  ];

  activities: Activity[] = [
    { type: 'call',    label: 'Appel entrant — Dupont M.',  detail: 'Triage AI: Urgence modérée',    time: '21:42',  icon: 'call_received'  },
    { type: 'ai',      label: 'Agent Triage activé',        detail: 'Résumé généré en 1.2s',          time: '21:41',  icon: 'psychology'     },
    { type: 'invoice', label: 'Facture #INV-2847 émise',    detail: '320,00 € — En attente',          time: '21:38',  icon: 'receipt'        },
    { type: 'patient', label: 'Patient enregistré',         detail: 'NIR masqué — Consentement OK',   time: '21:35',  icon: 'person_add'     },
    { type: 'call',    label: 'Appel clôturé — Martin L.',  detail: 'Résolu en 6m 14s',               time: '21:30',  icon: 'call_end'       },
    { type: 'ai',      label: 'Audit de conformité',        detail: 'ComplianceAgent — 0 anomalie',   time: '21:22',  icon: 'verified_user'  },
  ];

  agents = [
    { name: 'TriageAgent',     role: 'Triage & priorité',        active: true  },
    { name: 'SummaryAgent',    role: 'Résumé des appels',        active: true  },
    { name: 'ComplianceAgent', role: 'Audit RGPD / HDS',         active: false },
  ];
}
