import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';
import { InvoiceService, InvoiceDto } from '../../shared/services/invoice.service';

@Component({
  selector: 'app-billing',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  template: `
    <div class="fn-scene">

      <!-- ── Décor de salle ─────────────────────────── -->
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_factures.png)'">
      </div>
      <div class="fn-scene__veil"></div>

      <!-- Rosace — haut-droit, large, déborde -->
      <div class="fn-scene__deco fn-scene__deco--rosace-tr">
        <img src="assets/images/motif_rosace.png" alt="" />
      </div>
      <!-- Fougère — bas-gauche, rotation -->
      <div class="fn-scene__deco fn-scene__deco--fougere-bl">
        <img src="assets/images/motif_fougere.png" alt="" />
      </div>
      <!-- Rosace petite — milieu-gauche -->
      <div class="fn-scene__deco fn-scene__deco--rosace-ml">
        <img src="assets/images/motif_rosace.png" alt="" />
      </div>

      <!-- ── Contenu ────────────────────────────────── -->
      <div class="fn-scene__content">

    <app-page-header title="Salle des Factures" subtitle="Gestion des factures et documents">
      <button mat-raised-button color="primary">
        <mat-icon>add</mat-icon>
        Nouvelle facture
      </button>
    </app-page-header>

    <!-- Summary cards -->
    <div class="billing-summary">
      <div class="ci-card billing-stat" *ngFor="let stat of stats">
        <span class="label-sm">{{ stat.label }}</span>
        <div class="billing-stat__value">{{ stat.value }}</div>
      </div>
    </div>

    <!-- Invoices table -->
    <div class="ci-card mt-6">
      <h2 class="section-title mb-4">Factures récentes</h2>
      <table mat-table [dataSource]="invoices" class="w-full">

        <ng-container matColumnDef="id">
          <th mat-header-cell *matHeaderCellDef>N° Facture</th>
          <td mat-cell *matCellDef="let row">{{ row.id }}</td>
        </ng-container>

        <ng-container matColumnDef="client">
          <th mat-header-cell *matHeaderCellDef>Client</th>
          <td mat-cell *matCellDef="let row">{{ row.client }}</td>
        </ng-container>

        <ng-container matColumnDef="amount">
          <th mat-header-cell *matHeaderCellDef>Montant</th>
          <td mat-cell *matCellDef="let row"><strong>{{ row.amount }}</strong></td>
        </ng-container>

        <ng-container matColumnDef="date">
          <th mat-header-cell *matHeaderCellDef>Date</th>
          <td mat-cell *matCellDef="let row">{{ row.date }}</td>
        </ng-container>

        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Statut</th>
          <td mat-cell *matCellDef="let row">
            <app-status-badge [status]="row.status" [label]="statusLabel(row.status)"></app-status-badge>
          </td>
        </ng-container>

        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef></th>
          <td mat-cell *matCellDef="let row">
            <button mat-icon-button matTooltip="Télécharger PDF"><mat-icon>download</mat-icon></button>
            <button mat-icon-button matTooltip="Voir détails"><mat-icon>open_in_new</mat-icon></button>
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
      min-height: 100%;
      overflow: hidden;
      margin: calc(-1 * var(--scene-pad-y, 1.75rem)) calc(-1 * var(--scene-pad-x, 2rem));
    }
    .fn-scene__bg {
      position: absolute;
      inset: 0;
      background-size: cover;
      background-position: center top;
      opacity: .19;
      pointer-events: none;
      z-index: 0;
    }
    .fn-scene__veil {
      position: absolute;
      inset: 0;
      background: linear-gradient(
        170deg,
        rgba(4,7,15,.76) 0%,
        rgba(8,14,28,.48) 50%,
        rgba(6,10,22,.74) 100%
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

    /* Rosace — haut-droit (déborde) */
    .fn-scene__deco--rosace-tr {
      width: 560px; height: 560px;
      top: -160px; right: -170px;
      opacity: .075;
      transform: rotate(10deg);
      filter: brightness(2.1) hue-rotate(35deg) blur(1.2px);
    }
    /* Fougère — bas-gauche (déborde) */
    .fn-scene__deco--fougere-bl {
      width: 420px; height: 420px;
      bottom: -120px; left: -100px;
      opacity: .06;
      transform: rotate(40deg);
      filter: brightness(1.8) hue-rotate(15deg) blur(1px);
    }
    /* Rosace petite — milieu-gauche */
    .fn-scene__deco--rosace-ml {
      width: 220px; height: 220px;
      top: 50%; left: -70px;
      transform: translateY(-50%) rotate(-5deg);
      opacity: .04;
      filter: brightness(2) blur(2px);
    }

    .fn-scene__content {
      position: relative;
      z-index: 3;
      padding: var(--scene-pad-y, 1.75rem) var(--scene-pad-x, 2rem);
    }

    .billing-summary {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: 1rem;
    }
    .billing-stat { cursor: default; }
    .billing-stat__value {
      font-size: 1.5rem;
      font-weight: 700;
      color: var(--fn-bio);
      margin-top: 0.5rem;
      font-family: var(--fn-font-title);
    }
    .mt-6 { margin-top: 1.5rem; }
    .mb-4 { margin-bottom: 1rem; }
    .w-full { width: 100%; }
  `],
})
export class BillingComponent implements OnInit {
  displayedColumns = ['id', 'client', 'amount', 'date', 'status', 'actions'];
  loading = true;
  error: string | null = null;

  stats = [
    { label: 'Total facturé (mois)', value: '—' },
    { label: 'En attente',           value: '—' },
    { label: 'Impayées',             value: '—' },
    { label: 'Factures émises',      value: '—' },
  ];

  invoices: Array<{ id: string; client: string; amount: string; date: string; status: 'success' | 'pending' | 'error' }> = [];

  constructor(private invoiceService: InvoiceService) {}

  ngOnInit(): void {
    this.invoiceService.getInvoices().subscribe({
      next: (res) => {
        this.invoices = res.items.map((inv) => ({
          id: inv.id.substring(0, 8).toUpperCase(),
          client: inv.customerId,
          amount: `€ ${inv.amount.toFixed(2)}`,
          date: new Date(inv.createdAt).toLocaleDateString('fr-FR'),
          status: this.mapStatus(inv.status),
        }));
        const total = res.items.reduce((sum, i) => sum + i.amount, 0);
        const pending = res.items.filter((i) => i.status === 'Issued').reduce((sum, i) => sum + i.amount, 0);
        const overdue = res.items.filter((i) => i.status === 'Overdue').reduce((sum, i) => sum + i.amount, 0);
        this.stats = [
          { label: 'Total facturé (mois)', value: `€ ${total.toFixed(0)}` },
          { label: 'En attente',           value: `€ ${pending.toFixed(0)}` },
          { label: 'Impayées',             value: `€ ${overdue.toFixed(0)}` },
          { label: 'Factures émises',      value: String(res.totalCount) },
        ];
        this.loading = false;
      },
      error: () => {
        this.error = 'Impossible de charger les factures.';
        this.loading = false;
      },
    });
  }

  private mapStatus(status: string): 'success' | 'pending' | 'error' {
    if (status === 'Paid') return 'success';
    if (status === 'Overdue' || status === 'Cancelled') return 'error';
    return 'pending';
  }

  statusLabel(status: string): string {
    const labels: Record<string, string> = { success: 'Payée', pending: 'En attente', error: 'Impayée' };
    return labels[status] ?? status;
  }
}
