import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';

interface CallEntry {
  id: string;
  caller: string;
  phone: string;
  agent: string;
  duration: string;
  status: 'active' | 'pending' | 'inactive';
  time: string;
}

@Component({
  selector: 'app-call-center',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
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

    <div class="ci-card">
      <table mat-table [dataSource]="calls" class="w-full">

        <ng-container matColumnDef="caller">
          <th mat-header-cell *matHeaderCellDef>Appelant</th>
          <td mat-cell *matCellDef="let row">
            <strong>{{ row.caller }}</strong>
            <div class="text-xs text-gray-400">{{ row.phone }}</div>
          </td>
        </ng-container>

        <ng-container matColumnDef="agent">
          <th mat-header-cell *matHeaderCellDef>Agent</th>
          <td mat-cell *matCellDef="let row">{{ row.agent }}</td>
        </ng-container>

        <ng-container matColumnDef="duration">
          <th mat-header-cell *matHeaderCellDef>Durée</th>
          <td mat-cell *matCellDef="let row">{{ row.duration }}</td>
        </ng-container>

        <ng-container matColumnDef="time">
          <th mat-header-cell *matHeaderCellDef>Heure</th>
          <td mat-cell *matCellDef="let row">{{ row.time }}</td>
        </ng-container>

        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Statut</th>
          <td mat-cell *matCellDef="let row">
            <app-status-badge [status]="row.status" [label]="row.status"></app-status-badge>
          </td>
        </ng-container>

        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef></th>
          <td mat-cell *matCellDef="let row">
            <button mat-icon-button matTooltip="Détails">
              <mat-icon>open_in_new</mat-icon>
            </button>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
      </table>
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

    .w-full { width: 100%; }
  `],
})
export class CallCenterComponent {
  displayedColumns = ['caller', 'agent', 'duration', 'time', 'status', 'actions'];

  calls: CallEntry[] = [
    { id: '1', caller: 'Martin Dupont',   phone: '+33 6 12 34 56 78', agent: 'Léa M.',   duration: '3m 12s', status: 'active',   time: '09:41' },
    { id: '2', caller: 'Sophie Bernard',  phone: '+33 7 98 76 54 32', agent: 'Tom R.',   duration: '1m 05s', status: 'active',   time: '09:39' },
    { id: '3', caller: 'Jean-Paul Morin', phone: '+33 6 55 44 33 22', agent: 'En attente', duration: '—', status: 'pending', time: '09:44' },
    { id: '4', caller: 'Claire Fontaine', phone: '+33 6 11 22 33 44', agent: 'Ana S.',   duration: '7m 48s', status: 'inactive', time: '09:30' },
  ];
}
