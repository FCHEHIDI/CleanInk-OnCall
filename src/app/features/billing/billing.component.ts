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
    <app-page-header title="Facturation" subtitle="Gestion des factures et documents">
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
  `,
  styles: [`
    .billing-summary {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: 1rem;
    }
    .billing-stat { cursor: default; }
    .billing-stat__value {
      font-size: 1.5rem;
      font-weight: 700;
      color: var(--color-primary-dk);
      margin-top: 0.5rem;
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
