import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDividerModule } from '@angular/material/divider';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { EncounterService, EncounterDto } from '../../../shared/services/encounter.service';

@Component({
  selector: 'app-encounter-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatDividerModule,
    PageHeaderComponent,
  ],
  template: `
    <div class="fn-scene">
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_salle_des_documents.png)'">
      </div>
      <div class="fn-scene__veil"></div>

      <div class="fn-scene__content">
        <div *ngIf="loading" class="fn-loading">Chargement de la consultation…</div>
        <div *ngIf="error" class="fn-error">{{ error }}</div>

        <ng-container *ngIf="encounter && !loading">
          <app-page-header
            [title]="'Consultation · ' + classLabel(encounter.encounterClass)"
            [subtitle]="encounter.reasonText || 'Aucun motif renseigné'">
            <button mat-stroked-button routerLink="/encounters">
              <mat-icon>arrow_back</mat-icon>
              Retour
            </button>
          </app-page-header>

          <!-- Métadonnées -->
          <div class="fn-card fn-meta-grid">
            <div class="fn-meta-item">
              <span class="fn-meta-label">Statut</span>
              <span [class]="statusClass(encounter.status)">{{ statusLabel(encounter.status) }}</span>
            </div>
            <div class="fn-meta-item">
              <span class="fn-meta-label">Début</span>
              <span>{{ encounter.periodStart | date:'dd/MM/yyyy HH:mm' }}</span>
            </div>
            <div class="fn-meta-item">
              <span class="fn-meta-label">Fin</span>
              <span>{{ encounter.periodEnd ? (encounter.periodEnd | date:'dd/MM/yyyy HH:mm') : '—' }}</span>
            </div>
            <div class="fn-meta-item">
              <span class="fn-meta-label">Patient</span>
              <a [routerLink]="['/patients', encounter.patientId]" class="fn-link">
                {{ encounter.patientId | slice:0:8 }}…
              </a>
            </div>
          </div>

          <!-- Note clinique -->
          <div class="fn-card mt-4">
            <h3 class="fn-card-title">
              <mat-icon>description</mat-icon>
              Note clinique
            </h3>
            <mat-divider class="mb-4"></mat-divider>

            <ng-container *ngIf="encounter.status === 'InProgress'; else noteReadonly">
              <mat-form-field appearance="outline" class="w-full">
                <mat-label>Note de clôture (obligatoire)</mat-label>
                <textarea matInput [(ngModel)]="clinicalNote" rows="6"
                  placeholder="Saisissez la note clinique pour clôturer la consultation…"></textarea>
              </mat-form-field>
              <div class="fn-actions">
                <button mat-raised-button color="primary" (click)="finish()" [disabled]="!clinicalNote.trim() || saving">
                  <mat-icon>check_circle</mat-icon>
                  Clôturer la consultation
                </button>
                <button mat-stroked-button color="warn" (click)="promptCancel = true">
                  <mat-icon>cancel</mat-icon>
                  Annuler la consultation
                </button>
              </div>

              <!-- Formulaire annulation -->
              <div *ngIf="promptCancel" class="fn-cancel-form mt-4">
                <mat-form-field appearance="outline" class="w-full">
                  <mat-label>Motif d'annulation</mat-label>
                  <input matInput [(ngModel)]="cancelReason" placeholder="Ex : Patient absent, doublon…" />
                </mat-form-field>
                <button mat-raised-button color="warn" (click)="cancel()" [disabled]="!cancelReason.trim() || saving">
                  Confirmer l'annulation
                </button>
                <button mat-button (click)="promptCancel = false">Revenir</button>
              </div>
            </ng-container>

            <ng-template #noteReadonly>
              <p *ngIf="encounter.clinicalNote; else noNote" class="fn-clinical-note">
                {{ encounter.clinicalNote }}
              </p>
              <ng-template #noNote>
                <p class="fn-text-dim">Aucune note clinique enregistrée.</p>
              </ng-template>
            </ng-template>
          </div>

          <p *ngIf="actionError" class="fn-error mt-2">{{ actionError }}</p>
        </ng-container>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .fn-meta-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 16px; }
    .fn-meta-item { display: flex; flex-direction: column; gap: 4px; }
    .fn-meta-label { font-size: 0.75rem; color: var(--fn-text-dim); text-transform: uppercase; letter-spacing: .04em; }
    .fn-card-title { display: flex; align-items: center; gap: 8px; font-size: 1rem; font-weight: 600;
      color: var(--fn-text-dim); margin-bottom: 12px; }
    .fn-actions { display: flex; gap: 12px; flex-wrap: wrap; margin-top: 12px; }
    .fn-cancel-form { border: 1px solid rgba(220,50,50,.3); border-radius: 8px; padding: 16px; }
    .fn-clinical-note { white-space: pre-wrap; font-size: 0.9rem; line-height: 1.6; }
    .fn-link { color: var(--fn-accent); text-decoration: none; }
    .fn-link:hover { text-decoration: underline; }
    .badge-ok  { padding: 2px 8px; border-radius: 4px; font-size: 0.75rem; background: rgba(0,200,100,.15); color: #00c864; font-weight: 600; }
    .badge-dim { padding: 2px 8px; border-radius: 4px; font-size: 0.75rem; background: rgba(200,200,200,.15); color: var(--fn-text-dim); font-weight: 600; }
    .badge-ko  { padding: 2px 8px; border-radius: 4px; font-size: 0.75rem; background: rgba(220,50,50,.15); color: #e05050; font-weight: 600; }
  `],
})
export class EncounterDetailComponent implements OnInit {
  encounter: EncounterDto | null = null;
  loading = false;
  error = '';
  actionError = '';
  saving = false;
  clinicalNote = '';
  cancelReason = '';
  promptCancel = false;

  constructor(
    private route: ActivatedRoute,
    private encounterService: EncounterService,
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.error = 'Identifiant manquant.'; return; }
    this.loading = true;
    this.encounterService.getById(id).subscribe({
      next: (enc) => { this.encounter = enc; this.loading = false; },
      error: () => { this.error = 'Consultation introuvable.'; this.loading = false; },
    });
  }

  finish(): void {
    if (!this.encounter) return;
    this.saving = true;
    this.encounterService.finish(this.encounter.id, { clinicalNote: this.clinicalNote }).subscribe({
      next: (enc) => { this.encounter = enc; this.saving = false; this.actionError = ''; },
      error: () => { this.actionError = 'Erreur lors de la clôture.'; this.saving = false; },
    });
  }

  cancel(): void {
    if (!this.encounter) return;
    this.saving = true;
    this.encounterService.cancel(this.encounter.id, { reason: this.cancelReason }).subscribe({
      next: (enc) => { this.encounter = enc; this.saving = false; this.promptCancel = false; this.actionError = ''; },
      error: () => { this.actionError = 'Erreur lors de l\'annulation.'; this.saving = false; },
    });
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
