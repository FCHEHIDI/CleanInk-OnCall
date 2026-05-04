import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';
import { PatientService, PatientDto } from '../../../shared/services/patient.service';
import { InvoiceService, InvoiceDto } from '../../../shared/services/invoice.service';

@Component({
  selector: 'app-patient-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    PageHeaderComponent,
  ],
  template: `
    <div class="fn-scene">
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_patients.png)'">
      </div>
      <div class="fn-scene__veil"></div>

      <div class="fn-scene__content">
        <div *ngIf="loading" class="fn-loading">Chargement du dossier…</div>
        <div *ngIf="error" class="fn-error">{{ error }}</div>

        <ng-container *ngIf="patient && !loading">
          <app-page-header
            [title]="patient.lastName + ' ' + patient.firstName"
            [subtitle]="'Né(e) le ' + patient.dateOfBirth + ' · ' + (patient.gender || 'Genre non renseigné')">
            <button mat-stroked-button routerLink="/patients">
              <mat-icon>arrow_back</mat-icon>
              Retour
            </button>
          </app-page-header>

          <!-- Infos rapides -->
          <div class="fn-patient-chips">
            <span class="chip" [class.chip--ok]="patient.consentGiven" [class.chip--ko]="!patient.consentGiven">
              <mat-icon>{{ patient.consentGiven ? 'check_circle' : 'cancel' }}</mat-icon>
              Consentement {{ patient.consentGiven ? 'donné' : 'non donné' }}
            </span>
            <span class="chip" [class.chip--dim]="patient.isPseudonymized">
              <mat-icon>{{ patient.isPseudonymized ? 'visibility_off' : 'person' }}</mat-icon>
              {{ patient.isPseudonymized ? 'Pseudonymisé' : 'Dossier actif' }}
            </span>
            <span class="chip chip--neutral">
              <mat-icon>badge</mat-icon>
              NIR {{ patient.nirEncrypted ? 'enregistré (chiffré)' : 'non renseigné' }}
            </span>
          </div>

          <!-- Onglets -->
          <mat-tab-group class="fn-tabs mt-6" animationDuration="200ms">

            <mat-tab label="Appels">
              <div class="fn-tab-content fn-card">
                <p class="fn-empty">Aucun appel associé à ce patient. (À implémenter)</p>
              </div>
            </mat-tab>

            <mat-tab label="Factures">
              <div class="fn-tab-content fn-card">
                <div *ngIf="invoicesLoading" class="fn-loading">Chargement…</div>
                <table *ngIf="!invoicesLoading && invoices.length > 0" class="fn-table">
                  <thead>
                    <tr>
                      <th>Référence</th>
                      <th>Montant</th>
                      <th>Statut</th>
                      <th>Date</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr *ngFor="let inv of invoices">
                      <td>{{ inv.reference }}</td>
                      <td>{{ (inv.amountCents / 100).toFixed(2) }} €</td>
                      <td>{{ inv.status }}</td>
                      <td>{{ inv.issuedAt | date:'dd/MM/yyyy' }}</td>
                    </tr>
                  </tbody>
                </table>
                <p *ngIf="!invoicesLoading && invoices.length === 0" class="fn-empty">Aucune facture.</p>
              </div>
            </mat-tab>

          </mat-tab-group>
        </ng-container>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .fn-scene { position: relative; min-height: calc(100vh - 52px); overflow: hidden; margin: calc(-1 * var(--scene-pad-y, 1.75rem)) calc(-1 * var(--scene-pad-x, 2rem)); }
    .fn-scene__bg { position: absolute; inset: 0; background-size: cover; background-position: center; opacity: .42; pointer-events: none; z-index: 0; }
    .fn-scene__veil { position: absolute; inset: 0; background: linear-gradient(170deg, rgba(4,7,15,.60) 0%, rgba(8,14,28,.35) 50%, rgba(6,10,22,.58) 100%); pointer-events: none; z-index: 1; }
    .fn-scene__content { position: relative; z-index: 3; padding: var(--scene-pad-y, 1.75rem) var(--scene-pad-x, 2rem); }
    .fn-patient-chips { display: flex; gap: .75rem; flex-wrap: wrap; margin-top: 1rem; }
    .chip { display: flex; align-items: center; gap: .35rem; padding: .3rem .8rem; border-radius: 99px; font-size: .78rem; font-weight: 500; border: 1px solid rgba(255,255,255,.08); }
    .chip mat-icon { font-size: 14px; width: 14px; height: 14px; }
    .chip--ok { background: rgba(61,232,176,.12); color: var(--fn-bio); border-color: rgba(61,232,176,.2); }
    .chip--ko { background: rgba(248,113,113,.1); color: #f87171; border-color: rgba(248,113,113,.2); }
    .chip--dim { background: rgba(120,120,150,.1); color: var(--fn-text-dim); }
    .chip--neutral { background: rgba(255,255,255,.05); color: var(--fn-text-mid); }
    .fn-card { background: rgba(10,18,40,.7); border: 1px solid rgba(28,47,90,.5); border-radius: var(--fn-r-lg, 12px); padding: 1.25rem; backdrop-filter: blur(8px); }
    .fn-tabs { margin-top: 1.5rem; }
    .fn-tab-content { margin-top: .75rem; }
    .mt-6 { margin-top: 1.5rem; }
    .fn-loading, .fn-empty { color: var(--fn-text-dim); padding: 1.5rem; text-align: center; font-size: .9rem; }
    .fn-error { color: #f87171; padding: 1rem; }
    .fn-table { width: 100%; border-collapse: collapse; font-size: .85rem; }
    .fn-table th, .fn-table td { padding: .6rem 1rem; text-align: left; border-bottom: 1px solid rgba(28,47,90,.4); color: var(--fn-text); }
    .fn-table th { color: var(--fn-text-dim); font-weight: 500; font-size: .75rem; text-transform: uppercase; letter-spacing: .05em; }
  `],
})
export class PatientDetailComponent implements OnInit {
  patient: PatientDto | null = null;
  invoices: InvoiceDto[] = [];
  loading = true;
  error: string | null = null;
  invoicesLoading = true;

  constructor(
    private route: ActivatedRoute,
    private patientService: PatientService,
    private invoiceService: InvoiceService,
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.patientService.getById(id).subscribe({
      next: (p) => {
        this.patient = p;
        this.loading = false;
        this.loadInvoices(id);
      },
      error: () => {
        this.error = 'Patient introuvable.';
        this.loading = false;
      },
    });
  }

  private loadInvoices(patientId: string): void {
    this.invoiceService.getInvoices(patientId).subscribe({
      next: (res) => {
        this.invoices = res.items;
        this.invoicesLoading = false;
      },
      error: () => { this.invoicesLoading = false; },
    });
  }
}
