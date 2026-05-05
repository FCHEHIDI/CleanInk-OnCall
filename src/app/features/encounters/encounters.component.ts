import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { EncounterService, EncounterDto } from '../../shared/services/encounter.service';

@Component({
  selector: 'app-encounters',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatSelectModule,
    MatFormFieldModule,
    MatTooltipModule,
    PageHeaderComponent,
  ],
  template: `
    <div class="fn-scene">
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_salle_des_documents.png)'">
      </div>
      <div class="fn-scene__veil"></div>
      <div class="fn-scene__deco fn-scene__deco--rosace-c">
        <img src="assets/images/motif_rosace.png" alt="" />
      </div>
      <div class="fn-scene__deco fn-scene__deco--fougere-br">
        <img src="assets/images/motif_fougere.png" alt="" />
      </div>

      <div class="fn-scene__content">
        <app-page-header title="Consultations" subtitle="Historique des rencontres cliniques">
          <mat-form-field appearance="outline" class="filter-field">
            <mat-label>Statut</mat-label>
            <mat-select [(ngModel)]="statusFilter" (ngModelChange)="onFilterChange()">
              <mat-option value="">Tous</mat-option>
              <mat-option value="InProgress">En cours</mat-option>
              <mat-option value="Finished">Terminé</mat-option>
              <mat-option value="Cancelled">Annulé</mat-option>
            </mat-select>
          </mat-form-field>
        </app-page-header>

        <div class="fn-card mt-4">
          <div *ngIf="loading" class="fn-loading">Chargement…</div>
          <div *ngIf="error" class="fn-error">{{ error }}</div>

          <div *ngIf="!loading && !error && encounters.length === 0" class="fn-empty">
            <mat-icon>event_busy</mat-icon>
            <p>Aucune consultation trouvée.</p>
          </div>

          <table mat-table [dataSource]="encounters" *ngIf="!loading && !error && encounters.length > 0" class="w-full">

            <ng-container matColumnDef="patient">
              <th mat-header-cell *matHeaderCellDef>Patient</th>
              <td mat-cell *matCellDef="let row">
                <span class="mono">{{ row.patientId | slice:0:8 }}…</span>
              </td>
            </ng-container>

            <ng-container matColumnDef="class">
              <th mat-header-cell *matHeaderCellDef>Type</th>
              <td mat-cell *matCellDef="let row">
                <span class="badge-class" [attr.data-class]="row.encounterClass">
                  {{ classLabel(row.encounterClass) }}
                </span>
              </td>
            </ng-container>

            <ng-container matColumnDef="reason">
              <th mat-header-cell *matHeaderCellDef>Motif</th>
              <td mat-cell *matCellDef="let row">{{ row.reasonText || '—' }}</td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Statut</th>
              <td mat-cell *matCellDef="let row">
                <span [class]="statusClass(row.status)">{{ statusLabel(row.status) }}</span>
              </td>
            </ng-container>

            <ng-container matColumnDef="start">
              <th mat-header-cell *matHeaderCellDef>Début</th>
              <td mat-cell *matCellDef="let row">{{ row.periodStart | date:'dd/MM/yyyy HH:mm' }}</td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let row">
                <button mat-icon-button [routerLink]="['/encounters', row.id]" matTooltip="Détail">
                  <mat-icon>open_in_new</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="columns"></tr>
            <tr mat-row *matRowDef="let row; columns: columns"></tr>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }

    /* ── Stacking layers ── */
    .fn-scene { position: relative; overflow: hidden; }
    .fn-scene__bg  { position: absolute; inset: 0; background-size: cover; background-position: center;
                     opacity: .4; pointer-events: none; z-index: 0; }
    .fn-scene__veil { position: absolute; inset: 0;
                      background: linear-gradient(170deg, rgba(4,7,15,.65) 0%, rgba(8,14,28,.40) 50%, rgba(6,10,22,.62) 100%);
                      pointer-events: none; z-index: 1; }
    .fn-scene__deco { position: absolute; pointer-events: none; z-index: 2;
                      img { width: 100%; height: 100%; object-fit: contain; display: block; } }
    .fn-scene__deco--rosace-c { width: 520px; height: 520px; top: -160px; right: -160px;
                                 opacity: .06; filter: brightness(2) blur(1px); }
    .fn-scene__deco--fougere-br { width: 400px; height: 400px; bottom: -100px; left: -80px;
                                   opacity: .055; transform: rotate(20deg); filter: brightness(1.8) blur(1px); }
    .fn-scene__content { position: relative; z-index: 3;
                          padding: var(--scene-pad-y, 1.75rem) var(--scene-pad-x, 2rem); }

    .filter-field { width: 180px; }
    .mono { font-family: monospace; font-size: 0.85rem; color: var(--fn-text-dim); }
    .badge-class { padding: 2px 8px; border-radius: 4px; font-size: 0.75rem; font-weight: 600;
      background: rgba(30,90,168,.15); color: var(--fn-accent); }
    .badge-ok  { padding: 2px 8px; border-radius: 4px; font-size: 0.75rem; background: rgba(0,200,100,.15); color: #00c864; }
    .badge-dim { padding: 2px 8px; border-radius: 4px; font-size: 0.75rem; background: rgba(200,200,200,.15); color: var(--fn-text-dim); }
    .badge-ko  { padding: 2px 8px; border-radius: 4px; font-size: 0.75rem; background: rgba(220,50,50,.15); color: #e05050; }
    .fn-empty  { display: flex; flex-direction: column; align-items: center; gap: 8px; padding: 48px; color: var(--fn-text-dim); }
    .fn-empty mat-icon { font-size: 48px; width: 48px; height: 48px; opacity: .4; }
  `],
})
export class EncountersComponent implements OnInit {
  encounters: EncounterDto[] = [];
  columns = ['patient', 'class', 'reason', 'status', 'start', 'actions'];
  loading = false;
  error = '';
  statusFilter = '';

  constructor(private encounterService: EncounterService) {}

  ngOnInit(): void {
    this.loadEncounters();
  }

  loadEncounters(): void {
    this.loading = true;
    this.error = '';
    this.encounterService.getAll(1, 50, this.statusFilter || undefined).subscribe({
      next: (result) => {
        this.encounters = result.items;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Impossible de charger les consultations.';
        this.loading = false;
      },
    });
  }

  onFilterChange(): void {
    this.loadEncounters();
  }

  classLabel(cls: string): string {
    const map: Record<string, string> = {
      AMB: 'Ambulatoire', EMER: 'Urgence', IMP: 'Hospitalisation', HH: 'À domicile',
    };
    return map[cls] ?? cls;
  }

  statusLabel(status: string): string {
    const map: Record<string, string> = {
      InProgress: 'En cours', Finished: 'Terminé', Cancelled: 'Annulé',
    };
    return map[status] ?? status;
  }

  statusClass(status: string): string {
    return status === 'InProgress' ? 'badge-ok' : status === 'Finished' ? 'badge-dim' : 'badge-ko';
  }
}
