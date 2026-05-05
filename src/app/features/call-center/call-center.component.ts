import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent, BadgeStatus } from '../../shared/components/status-badge/status-badge.component';
import { CallService, CallDto } from '../../shared/services/call.service';

@Component({
  selector: 'app-call-center',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  template: `
    <div class="fn-scene">

      <!-- ── Décor de salle ─────────────────────────── -->
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_appels.png)'">
      </div>
      <div class="fn-scene__veil"></div>

      <!-- Fougère — droite, grande, déborde verticalement -->
      <div class="fn-scene__deco fn-scene__deco--fougere-r">
        <img src="assets/images/motif_fougere.png" alt="" />
      </div>
      <!-- Rosace — bas-gauche -->
      <div class="fn-scene__deco fn-scene__deco--rosace-bl">
        <img src="assets/images/motif_rosace.png" alt="" />
      </div>
      <!-- Fougère miroir — bas-centre -->
      <div class="fn-scene__deco fn-scene__deco--fougere-bc">
        <img src="assets/images/motif_fougere.png" alt="" />
      </div>

      <!-- ── Contenu ────────────────────────────────── -->
      <div class="fn-scene__content">

    <app-page-header title="Salle des Appels" subtitle="File d'attente et appels en cours">
      <button mat-raised-button color="primary">
        <mat-icon>add_call</mat-icon>
        Nouvel appel
      </button>
    </app-page-header>

    <div class="fn-card mt-4">
      <div *ngIf="loading" class="fn-loading">Chargement…</div>
      <div *ngIf="error" class="fn-error">{{ error }}</div>

      <table *ngIf="!loading && !error" mat-table [dataSource]="calls" class="w-full">

        <ng-container matColumnDef="subject">
          <th mat-header-cell *matHeaderCellDef>Sujet</th>
          <td mat-cell *matCellDef="let row">
            <strong>{{ row.subject }}</strong>
            <div class="fn-sub">{{ row.description | slice:0:60 }}{{ row.description.length > 60 ? '…' : '' }}</div>
          </td>
        </ng-container>

        <ng-container matColumnDef="priority">
          <th mat-header-cell *matHeaderCellDef>Priorité</th>
          <td mat-cell *matCellDef="let row">
            <span [class]="priorityClass(row.priority ?? 'routine')">{{ row.priority ?? 'routine' }}</span>
          </td>
        </ng-container>

        <ng-container matColumnDef="agent">
          <th mat-header-cell *matHeaderCellDef>Assigné à</th>
          <td mat-cell *matCellDef="let row">
            <span *ngIf="row.assignedPractitionerId; else unassigned" class="fn-agent">
              <mat-icon class="fn-agent__icon">person</mat-icon>
              {{ row.assignedPractitionerId | slice:0:8 }}…
            </span>
            <ng-template #unassigned>
              <span class="fn-unassigned">Non assigné</span>
            </ng-template>
          </td>
        </ng-container>

        <ng-container matColumnDef="createdAt">
          <th mat-header-cell *matHeaderCellDef>Reçu le</th>
          <td mat-cell *matCellDef="let row">{{ row.createdAt | date:'dd/MM HH:mm' }}</td>
        </ng-container>

        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Statut</th>
          <td mat-cell *matCellDef="let row">
            <app-status-badge
              [status]="statusToBadge(row.status)"
              [label]="statusLabel(row.status)">
            </app-status-badge>
          </td>
        </ng-container>

        <ng-container matColumnDef="aiTag">
          <th mat-header-cell *matHeaderCellDef>Tag IA</th>
          <td mat-cell *matCellDef="let row">
            <span *ngIf="row.aiTriageTag" class="fn-ai-tag">{{ row.aiTriageTag }}</span>
            <span *ngIf="!row.aiTriageTag" class="fn-dim">—</span>
          </td>
        </ng-container>

        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef></th>
          <td mat-cell *matCellDef="let row">
            <button mat-icon-button [routerLink]="['/call-center', row.id]" matTooltip="Voir l'appel">
              <mat-icon>open_in_new</mat-icon>
            </button>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
      </table>

      <p *ngIf="!loading && !error && calls.length === 0" class="fn-empty">
        Aucun appel en cours.
      </p>
    </div>

      </div><!-- /fn-scene__content -->
    </div><!-- /fn-scene -->
  `,
  styles: [`
    :host { display: block; }

    .fn-scene {
      position: relative;
      min-height: calc(100vh - 52px);
      overflow: hidden;
      margin: calc(-1 * var(--scene-pad-y, 1.75rem)) calc(-1 * var(--scene-pad-x, 2rem));
    }
    .fn-scene__bg {
      position: absolute;
      inset: 0;
      background-size: cover;
      background-position: center center;
      opacity: .46;
      pointer-events: none;
      z-index: 0;
    }
    .fn-scene__veil {
      position: absolute;
      inset: 0;
      background: linear-gradient(
        160deg,
        rgba(4,7,15,.60) 0%,
        rgba(6,12,26,.34) 50%,
        rgba(4,7,15,.58) 100%
      );
      pointer-events: none;
      z-index: 1;
    }
    .fn-scene__deco {
      position: absolute;
      pointer-events: none;
      z-index: 2;
      img { width: 100%; height: 100%; object-fit: contain; display: block; }
    }

    /* Fougère — droite haute (déborde) */
    .fn-scene__deco--fougere-r {
      width: 520px; height: 520px;
      top: -60px; right: -160px;
      opacity: .07;
      transform: rotate(12deg);
      filter: brightness(1.9) hue-rotate(15deg) blur(1px);
    }
    /* Rosace — bas-gauche (déborde) */
    .fn-scene__deco--rosace-bl {
      width: 500px; height: 500px;
      bottom: -180px; left: -150px;
      opacity: .065;
      transform: rotate(-5deg);
      filter: brightness(2) hue-rotate(20deg) blur(1.5px);
    }
    /* Fougère miroir — bas-centre, légère */
    .fn-scene__deco--fougere-bc {
      width: 320px; height: 320px;
      bottom: -80px; left: 50%;
      transform: translateX(-50%) rotate(175deg);
      opacity: .04;
      filter: brightness(1.6) blur(1px);
    }

    .fn-scene__content {
      position: relative;
      z-index: 3;
      padding: var(--scene-pad-y, 1.75rem) var(--scene-pad-x, 2rem);
    }

    .fn-card { background: rgba(10,18,40,.7); border: 1px solid rgba(28,47,90,.5); border-radius: var(--fn-r-lg, 12px); padding: 1rem; backdrop-filter: blur(8px); overflow-x: auto; }
    .mt-4 { margin-top: 1rem; }
    .w-full { width: 100%; }
    .fn-loading, .fn-empty { color: var(--fn-text-dim); padding: 1.5rem; text-align: center; font-size: .9rem; }
    .fn-error { color: #f87171; padding: 1rem; }
    .fn-sub { font-size: .75rem; color: var(--fn-text-dim, #8899bb); margin-top: 2px; }
    .fn-dim { color: var(--fn-text-dim, #8899bb); font-size: .8rem; }
    .fn-agent { display: inline-flex; align-items: center; gap: 4px; font-size: .8rem; color: var(--fn-bio, #3de8b0); }
    .fn-agent__icon { font-size: 14px; width: 14px; height: 14px; }
    .fn-unassigned { font-size: .8rem; color: var(--fn-text-dim, #8899bb); font-style: italic; }
    .fn-ai-tag { background: rgba(61,232,176,.12); color: var(--fn-bio, #3de8b0); padding: .15rem .5rem; border-radius: 99px; font-size: .72rem; font-weight: 500; }
    .prio-routine { color: #94a3b8; font-size: .78rem; }
    .prio-urgent  { color: #fb923c; font-size: .78rem; font-weight: 600; }
    .prio-asap    { color: #f87171; font-size: .78rem; font-weight: 700; }
    .prio-stat    { color: #dc2626; font-size: .78rem; font-weight: 700; text-transform: uppercase; }
  `],
})
export class CallCenterComponent implements OnInit {
  displayedColumns = ['subject', 'priority', 'agent', 'createdAt', 'status', 'aiTag', 'actions'];
  calls: CallDto[] = [];
  loading = true;
  error: string | null = null;

  constructor(private callService: CallService) {}

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.loading = true;
    this.error = null;
    this.callService.getCalls(1, 50).subscribe({
      next: (res) => {
        this.calls = res.items;
        this.loading = false;
      },
      error: () => {
        this.error = 'Impossible de charger les appels.';
        this.loading = false;
      },
    });
  }

  statusToBadge(status: string): BadgeStatus {
    const map: Record<string, BadgeStatus> = {
      Pending:    'pending',
      InProgress: 'active',
      Resolved:   'success',
      Escalated:  'error',
      Cancelled:  'inactive',
    };
    return map[status] ?? 'inactive';
  }

  statusLabel(status: string): string {
    const map: Record<string, string> = {
      Pending:    'En attente',
      InProgress: 'En cours',
      Resolved:   'Résolu',
      Escalated:  'Escaladé',
      Cancelled:  'Annulé',
    };
    return map[status] ?? status;
  }

  priorityClass(priority: string): string {
    return `prio-${priority.toLowerCase()}`;
  }
}
