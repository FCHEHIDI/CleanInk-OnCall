import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { AuditService, AuditEventDto } from '../../shared/services/audit.service';

@Component({
  selector: 'app-audit',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatTooltipModule,
    PageHeaderComponent,
  ],
  template: `
    <div class="fn-scene">
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_admin.png)'">
      </div>
      <div class="fn-scene__veil"></div>
      <div class="fn-scene__deco fn-scene__deco--rosace-tr">
        <img src="assets/images/motif_rosace.png" alt="" />
      </div>

      <div class="fn-scene__content">
        <app-page-header title="Journal d'Audit" subtitle="Traçabilité RGPD / HDS — Accès restreint">
        </app-page-header>

        <!-- Filtres -->
        <div class="fn-filters mt-4">
          <mat-form-field appearance="outline">
            <mat-label>Type de ressource</mat-label>
            <mat-select [(ngModel)]="resourceType">
              <mat-option value="Call">Appel</mat-option>
              <mat-option value="Patient">Patient</mat-option>
              <mat-option value="User">Utilisateur</mat-option>
              <mat-option value="Invoice">Facture</mat-option>
            </mat-select>
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>ID de la ressource</mat-label>
            <input matInput [(ngModel)]="resourceId" placeholder="UUID…" />
          </mat-form-field>

          <button mat-raised-button color="primary" (click)="search()" [disabled]="!resourceId">
            <mat-icon>search</mat-icon>
            Rechercher
          </button>
        </div>

        <!-- Résultats -->
        <div class="fn-card mt-4">
          <div *ngIf="loading" class="fn-loading">Chargement…</div>
          <div *ngIf="error" class="fn-error">{{ error }}</div>

          <table mat-table [dataSource]="events" *ngIf="!loading && events.length > 0" class="w-full">

            <ng-container matColumnDef="recordedAt">
              <th mat-header-cell *matHeaderCellDef>Date</th>
              <td mat-cell *matCellDef="let row">{{ row.recordedAt | date:'dd/MM/yyyy HH:mm:ss' }}</td>
            </ng-container>

            <ng-container matColumnDef="actorEmail">
              <th mat-header-cell *matHeaderCellDef>Acteur</th>
              <td mat-cell *matCellDef="let row">{{ row.actorEmail }}</td>
            </ng-container>

            <ng-container matColumnDef="action">
              <th mat-header-cell *matHeaderCellDef>Action</th>
              <td mat-cell *matCellDef="let row">
                <span class="action-badge">{{ row.action }}</span>
              </td>
            </ng-container>

            <ng-container matColumnDef="resourceType">
              <th mat-header-cell *matHeaderCellDef>Ressource</th>
              <td mat-cell *matCellDef="let row">{{ row.resourceType }} / {{ row.resourceId | slice:0:8 }}…</td>
            </ng-container>

            <ng-container matColumnDef="ipAddress">
              <th mat-header-cell *matHeaderCellDef>IP</th>
              <td mat-cell *matCellDef="let row">{{ row.ipAddress || '—' }}</td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="columns"></tr>
            <tr mat-row *matRowDef="let row; columns: columns"></tr>
          </table>

          <p *ngIf="!loading && events.length === 0 && searched" class="fn-empty">
            Aucun événement d'audit trouvé.
          </p>
          <p *ngIf="!searched && !loading" class="fn-empty">
            Saisir un type et un ID pour rechercher dans le journal.
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .fn-scene { position: relative; min-height: calc(100vh - 52px); overflow: hidden; margin: calc(-1 * var(--scene-pad-y, 1.75rem)) calc(-1 * var(--scene-pad-x, 2rem)); }
    .fn-scene__bg { position: absolute; inset: 0; background-size: cover; background-position: center; opacity: .38; pointer-events: none; z-index: 0; }
    .fn-scene__veil { position: absolute; inset: 0; background: linear-gradient(170deg, rgba(4,7,15,.65) 0%, rgba(8,14,28,.40) 50%, rgba(6,10,22,.62) 100%); pointer-events: none; z-index: 1; }
    .fn-scene__deco { position: absolute; pointer-events: none; z-index: 2; img { width: 100%; height: 100%; object-fit: contain; display: block; } }
    .fn-scene__deco--rosace-tr { width: 460px; height: 460px; top: -120px; right: -140px; opacity: .065; filter: brightness(2) hue-rotate(35deg) blur(1px); }
    .fn-scene__content { position: relative; z-index: 3; padding: var(--scene-pad-y, 1.75rem) var(--scene-pad-x, 2rem); }
    .fn-filters { display: flex; gap: 1rem; align-items: center; flex-wrap: wrap; }
    .fn-card { background: rgba(10,18,40,.7); border: 1px solid rgba(28,47,90,.5); border-radius: var(--fn-r-lg, 12px); padding: 1rem; backdrop-filter: blur(8px); overflow-x: auto; }
    .mt-4 { margin-top: 1rem; }
    .w-full { width: 100%; }
    .fn-loading, .fn-empty { color: var(--fn-text-dim); padding: 1.5rem; text-align: center; font-size: .9rem; }
    .fn-error { color: #f87171; padding: 1rem; }
    .action-badge { background: rgba(61,232,176,.1); color: var(--fn-bio); padding: .15rem .5rem; border-radius: 4px; font-size: .75rem; font-family: monospace; }
  `],
})
export class AuditComponent {
  columns = ['recordedAt', 'actorEmail', 'action', 'resourceType', 'ipAddress'];
  events: AuditEventDto[] = [];
  loading = false;
  error: string | null = null;
  searched = false;
  resourceType = 'Call';
  resourceId = '';

  constructor(private auditService: AuditService) {}

  search(): void {
    if (!this.resourceId.trim()) return;
    this.loading = true;
    this.error = null;
    this.searched = true;
    this.auditService.getByResource(this.resourceType, this.resourceId.trim()).subscribe({
      next: (events) => {
        this.events = events;
        this.loading = false;
      },
      error: () => {
        this.error = 'Erreur lors de la recherche.';
        this.loading = false;
      },
    });
  }
}
