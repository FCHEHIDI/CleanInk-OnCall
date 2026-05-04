import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { PatientService, PatientDto } from '../../shared/services/patient.service';

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatTooltipModule,
    PageHeaderComponent,
  ],
  template: `
    <div class="fn-scene">
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_patients.png)'">
      </div>
      <div class="fn-scene__veil"></div>
      <div class="fn-scene__deco fn-scene__deco--rosace-tr">
        <img src="assets/images/motif_rosace.png" alt="" />
      </div>
      <div class="fn-scene__deco fn-scene__deco--fougere-bl">
        <img src="assets/images/motif_fougere.png" alt="" />
      </div>

      <div class="fn-scene__content">
        <app-page-header title="Dossiers Patients" subtitle="Recherche et gestion des patients">
          <button mat-raised-button color="primary" (click)="showRegister = true">
            <mat-icon>person_add</mat-icon>
            Nouveau patient
          </button>
        </app-page-header>

        <!-- Barre de recherche -->
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Rechercher un patient…</mat-label>
          <input matInput [(ngModel)]="searchQuery" (input)="onSearch()" placeholder="Nom, prénom…" />
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>

        <!-- Tableau -->
        <div class="fn-card mt-4">
          <div *ngIf="loading" class="fn-loading">Chargement…</div>
          <div *ngIf="error" class="fn-error">{{ error }}</div>

          <table mat-table [dataSource]="patients" *ngIf="!loading && !error" class="w-full">

            <ng-container matColumnDef="name">
              <th mat-header-cell *matHeaderCellDef>Nom</th>
              <td mat-cell *matCellDef="let row">
                <strong>{{ row.lastName }} {{ row.firstName }}</strong>
              </td>
            </ng-container>

            <ng-container matColumnDef="dateOfBirth">
              <th mat-header-cell *matHeaderCellDef>Date de naissance</th>
              <td mat-cell *matCellDef="let row">{{ row.dateOfBirth }}</td>
            </ng-container>

            <ng-container matColumnDef="gender">
              <th mat-header-cell *matHeaderCellDef>Genre</th>
              <td mat-cell *matCellDef="let row">{{ row.gender || '—' }}</td>
            </ng-container>

            <ng-container matColumnDef="consent">
              <th mat-header-cell *matHeaderCellDef>Consentement</th>
              <td mat-cell *matCellDef="let row">
                <mat-icon [style.color]="row.consentGiven ? 'var(--fn-bio)' : 'var(--fn-text-dim)'">
                  {{ row.consentGiven ? 'check_circle' : 'cancel' }}
                </mat-icon>
              </td>
            </ng-container>

            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Statut</th>
              <td mat-cell *matCellDef="let row">
                <span [class]="row.isPseudonymized ? 'badge-dim' : 'badge-ok'">
                  {{ row.isPseudonymized ? 'Pseudonymisé' : 'Actif' }}
                </span>
              </td>
            </ng-container>

            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef></th>
              <td mat-cell *matCellDef="let row">
                <button mat-icon-button [routerLink]="['/patients', row.id]" matTooltip="Voir le dossier">
                  <mat-icon>open_in_new</mat-icon>
                </button>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="columns"></tr>
            <tr mat-row *matRowDef="let row; columns: columns"></tr>
          </table>

          <p *ngIf="!loading && patients.length === 0 && !error" class="fn-empty">
            Aucun patient trouvé.
          </p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .fn-scene { position: relative; min-height: calc(100vh - 52px); overflow: hidden; margin: calc(-1 * var(--scene-pad-y, 1.75rem)) calc(-1 * var(--scene-pad-x, 2rem)); }
    .fn-scene__bg { position: absolute; inset: 0; background-size: cover; background-position: center; opacity: .42; pointer-events: none; z-index: 0; }
    .fn-scene__veil { position: absolute; inset: 0; background: linear-gradient(170deg, rgba(4,7,15,.60) 0%, rgba(8,14,28,.35) 50%, rgba(6,10,22,.58) 100%); pointer-events: none; z-index: 1; }
    .fn-scene__deco { position: absolute; pointer-events: none; z-index: 2; img { width: 100%; height: 100%; object-fit: contain; display: block; } }
    .fn-scene__deco--rosace-tr { width: 460px; height: 460px; top: -120px; right: -140px; opacity: .07; filter: brightness(2) hue-rotate(35deg) blur(1px); }
    .fn-scene__deco--fougere-bl { width: 380px; height: 380px; bottom: -100px; left: -80px; opacity: .055; transform: rotate(40deg); filter: brightness(1.8) blur(1px); }
    .fn-scene__content { position: relative; z-index: 3; padding: var(--scene-pad-y, 1.75rem) var(--scene-pad-x, 2rem); }
    .fn-card { background: rgba(10,18,40,.7); border: 1px solid rgba(28,47,90,.5); border-radius: var(--fn-r-lg, 12px); padding: 1rem; backdrop-filter: blur(8px); overflow-x: auto; }
    .search-field { width: 100%; max-width: 420px; margin-top: 1rem; }
    .mt-4 { margin-top: 1rem; }
    .w-full { width: 100%; }
    .fn-loading, .fn-empty { color: var(--fn-text-dim); padding: 1.5rem; text-align: center; font-size: .9rem; }
    .fn-error { color: #f87171; padding: 1rem; }
    .badge-ok { background: rgba(61,232,176,.15); color: var(--fn-bio); padding: .2rem .6rem; border-radius: 99px; font-size: .75rem; font-weight: 500; }
    .badge-dim { background: rgba(120,120,150,.15); color: var(--fn-text-dim); padding: .2rem .6rem; border-radius: 99px; font-size: .75rem; }
  `],
})
export class PatientsComponent implements OnInit {
  columns = ['name', 'dateOfBirth', 'gender', 'consent', 'status', 'actions'];
  patients: PatientDto[] = [];
  loading = true;
  error: string | null = null;
  searchQuery = '';
  showRegister = false;

  constructor(private patientService: PatientService) {}

  ngOnInit(): void {
    this.load();
  }

  onSearch(): void {
    this.load();
  }

  private load(): void {
    this.loading = true;
    this.error = null;
    this.patientService.searchPatients(this.searchQuery).subscribe({
      next: (res) => {
        this.patients = res.items;
        this.loading = false;
      },
      error: () => {
        this.error = 'Impossible de charger les patients.';
        this.loading = false;
      },
    });
  }
}
