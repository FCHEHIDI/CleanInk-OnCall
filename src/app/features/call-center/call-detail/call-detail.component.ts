import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatChipsModule } from '@angular/material/chips';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { CallService, CallDto } from '../../../shared/services/call.service';

@Component({
  selector: 'app-call-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    MatChipsModule,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  template: `
    <div class="fn-scene">
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_appels.png)'">
      </div>
      <div class="fn-scene__veil"></div>

      <div class="fn-scene__content">
        <div *ngIf="loading" class="fn-loading">Chargement de l'appel…</div>
        <div *ngIf="error" class="fn-error">{{ error }}</div>

        <ng-container *ngIf="call && !loading">
          <app-page-header
            [title]="'Appel · ' + call.subject"
            [subtitle]="'Créé le ' + (call.createdAt | date:'dd/MM/yyyy HH:mm')">
            <button mat-stroked-button routerLink="/call-center">
              <mat-icon>arrow_back</mat-icon>
              Retour
            </button>
          </app-page-header>

          <!-- Métadonnées -->
          <div class="fn-card fn-meta-grid">
            <div class="fn-meta-item">
              <span class="fn-meta-label">Statut</span>
              <app-status-badge [status]="callStatusBadge(call.status)" [label]="call.status"></app-status-badge>
            </div>
            <div class="fn-meta-item">
              <span class="fn-meta-label">Priorité</span>
              <mat-chip [class]="priorityClass(call.priority)">{{ call.priority }}</mat-chip>
            </div>
            <div class="fn-meta-item">
              <span class="fn-meta-label">Patient</span>
              <a *ngIf="call.patientId; else noPatient" [routerLink]="['/patients', call.patientId]" class="fn-link">
                {{ call.patientId | slice:0:8 }}…
              </a>
              <ng-template #noPatient><span class="fn-text-dim">Non renseigné</span></ng-template>
            </div>
            <div class="fn-meta-item">
              <span class="fn-meta-label">Opérateur</span>
              <span>{{ call.assignedPractitionerId ? (call.assignedPractitionerId | slice:0:8) + '…' : 'Non assigné' }}</span>
            </div>
          </div>

          <!-- Description -->
          <div class="fn-card mt-4">
            <h3 class="fn-card-title">
              <mat-icon>chat_bubble_outline</mat-icon>
              Description
            </h3>
            <mat-divider class="mb-4"></mat-divider>
            <p class="fn-description">{{ call.description }}</p>
          </div>

          <!-- IA Triage -->
          <div class="fn-card mt-4" *ngIf="call.aiTriageTag || call.aiSummary">
            <h3 class="fn-card-title">
              <mat-icon>psychology</mat-icon>
              Analyse IA
            </h3>
            <mat-divider class="mb-4"></mat-divider>
            <p *ngIf="call.aiTriageTag"><strong>Tag triage :</strong> {{ call.aiTriageTag }}</p>
            <p *ngIf="call.aiSummary" class="mt-2"><strong>Résumé IA :</strong> {{ call.aiSummary }}</p>
          </div>
        </ng-container>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .fn-meta-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 16px; }
    .fn-meta-item { display: flex; flex-direction: column; gap: 4px; }
    .fn-meta-label { font-size: 0.75rem; color: var(--fn-text-dim); text-transform: uppercase; letter-spacing: .04em; }
    .fn-card-title { display: flex; align-items: center; gap: 8px; font-size: 1rem; font-weight: 600;
      color: var(--fn-text-dim); margin-bottom: 12px; }
    .fn-description { white-space: pre-wrap; font-size: 0.9rem; line-height: 1.6; }
    .fn-link { color: var(--fn-accent); text-decoration: none; }
    .fn-link:hover { text-decoration: underline; }
    .prio-high { background: rgba(220,50,50,.15) !important; color: #e05050 !important; }
    .prio-normal { background: rgba(30,90,168,.15) !important; color: var(--fn-accent) !important; }
    .prio-low { background: rgba(200,200,200,.15) !important; color: var(--fn-text-dim) !important; }
  `],
})
export class CallDetailComponent implements OnInit {
  call: CallDto | null = null;
  loading = false;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private callService: CallService,
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.error = 'Identifiant manquant.'; return; }
    this.loading = true;
    this.callService.getById(id).subscribe({
      next: (c) => { this.call = c; this.loading = false; },
      error: () => { this.error = 'Appel introuvable.'; this.loading = false; },
    });
  }

  callStatusBadge(status: string): 'active' | 'pending' | 'inactive' {
    if (status === 'InProgress') return 'active';
    if (status === 'Pending') return 'pending';
    return 'inactive';
  }

  priorityClass(priority: string): string {
    if (priority?.toLowerCase() === 'high') return 'prio-high';
    if (priority?.toLowerCase() === 'low') return 'prio-low';
    return 'prio-normal';
  }
}
