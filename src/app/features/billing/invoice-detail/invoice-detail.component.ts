import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { InvoiceService, InvoiceDto } from '../../../shared/services/invoice.service';

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatDividerModule,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  template: `
    <div class="fn-scene">
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_factures.png)'">
      </div>
      <div class="fn-scene__veil"></div>

      <div class="fn-scene__content">
        <div *ngIf="loading" class="fn-loading">Chargement de la facture…</div>
        <div *ngIf="error" class="fn-error">{{ error }}</div>

        <ng-container *ngIf="invoice && !loading">
          <app-page-header
            [title]="'Facture · ' + invoice.reference"
            [subtitle]="'Émise le ' + (invoice.issuedAt | date:'dd/MM/yyyy')">
            <button mat-stroked-button routerLink="/billing">
              <mat-icon>arrow_back</mat-icon>
              Retour
            </button>
          </app-page-header>

          <!-- Métadonnées -->
          <div class="fn-card fn-meta-grid">
            <div class="fn-meta-item">
              <span class="fn-meta-label">Statut</span>
              <app-status-badge
                [status]="invoiceStatusBadge(invoice.status)"
                [label]="statusLabel(invoice.status)">
              </app-status-badge>
            </div>
            <div class="fn-meta-item">
              <span class="fn-meta-label">Patient</span>
              <a [routerLink]="['/patients', invoice.patientId]" class="fn-link">
                {{ invoice.patientId | slice:0:8 }}…
              </a>
            </div>
            <div class="fn-meta-item">
              <span class="fn-meta-label">Consultation liée</span>
              <a *ngIf="invoice.encounterId; else noEnc" [routerLink]="['/encounters', invoice.encounterId]" class="fn-link">
                {{ invoice.encounterId | slice:0:8 }}…
              </a>
              <ng-template #noEnc><span class="fn-text-dim">—</span></ng-template>
            </div>
            <div class="fn-meta-item">
              <span class="fn-meta-label">Échéance</span>
              <span>{{ invoice.dueAt ? (invoice.dueAt | date:'dd/MM/yyyy') : '—' }}</span>
            </div>
            <div class="fn-meta-item">
              <span class="fn-meta-label">Payée le</span>
              <span>{{ invoice.paidAt ? (invoice.paidAt | date:'dd/MM/yyyy') : '—' }}</span>
            </div>
          </div>

          <!-- Montants -->
          <div class="fn-card mt-4">
            <h3 class="fn-card-title">
              <mat-icon>receipt_long</mat-icon>
              Détail financier
            </h3>
            <mat-divider class="mb-4"></mat-divider>

            <div class="fn-amount-grid">
              <div class="fn-amount-row">
                <span>Montant HT</span>
                <span>{{ (invoice.amountCents / 100) | number:'1.2-2' }} €</span>
              </div>
              <div class="fn-amount-row">
                <span>TVA</span>
                <span>{{ (invoice.vatCents / 100) | number:'1.2-2' }} €</span>
              </div>
              <mat-divider></mat-divider>
              <div class="fn-amount-row fn-amount-total">
                <span>Total TTC</span>
                <span>{{ ((invoice.amountCents + invoice.vatCents) / 100) | number:'1.2-2' }} €</span>
              </div>
            </div>
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
    .fn-amount-grid { display: flex; flex-direction: column; gap: 12px; max-width: 360px; }
    .fn-amount-row { display: flex; justify-content: space-between; font-size: 0.95rem; }
    .fn-amount-total { font-weight: 700; font-size: 1.1rem; padding-top: 8px; color: var(--fn-accent); }
    .fn-link { color: var(--fn-accent); text-decoration: none; }
    .fn-link:hover { text-decoration: underline; }
  `],
})
export class InvoiceDetailComponent implements OnInit {
  invoice: InvoiceDto | null = null;
  loading = false;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private invoiceService: InvoiceService,
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.error = 'Identifiant manquant.'; return; }
    this.loading = true;
    this.invoiceService.getById(id).subscribe({
      next: (inv) => { this.invoice = inv; this.loading = false; },
      error: () => { this.error = 'Facture introuvable.'; this.loading = false; },
    });
  }

  invoiceStatusBadge(status: string): 'active' | 'pending' | 'inactive' {
    if (status === 'Paid') return 'active';
    if (status === 'Issued') return 'pending';
    return 'inactive';
  }

  statusLabel(status: string): string {
    const map: Record<string, string> = {
      Draft: 'Brouillon', Issued: 'Émise', Paid: 'Payée', Cancelled: 'Annulée',
    };
    return map[status] ?? status;
  }
}
