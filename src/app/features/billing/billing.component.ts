import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';

interface Invoice {
  id: string;
  client: string;
  amount: string;
  date: string;
  status: 'success' | 'pending' | 'error';
}

@Component({
  selector: 'app-billing',
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
export class BillingComponent {
  displayedColumns = ['id', 'client', 'amount', 'date', 'status', 'actions'];

  stats = [
    { label: 'Total facturé (mois)', value: '€ 42 800' },
    { label: 'En attente',           value: '€ 8 350'  },
    { label: 'Impayées',             value: '€ 1 200'  },
    { label: 'Factures émises',      value: '67'        },
  ];

  invoices: Invoice[] = [
    { id: 'FAC-2024-0042', client: 'Clinique St-Marc',      amount: '€ 3 200', date: '28/04/2026', status: 'success' },
    { id: 'FAC-2024-0041', client: 'Cabinet Dr. Renaud',    amount: '€ 1 800', date: '27/04/2026', status: 'pending' },
    { id: 'FAC-2024-0040', client: 'Pharmacie Centrale',    amount: '€ 950',   date: '26/04/2026', status: 'success' },
    { id: 'FAC-2024-0039', client: 'Labo BioAnalyse',       amount: '€ 2 100', date: '25/04/2026', status: 'error'   },
    { id: 'FAC-2024-0038', client: 'Centre Médical Riviera',amount: '€ 4 600', date: '24/04/2026', status: 'success' },
  ];

  statusLabel(status: string): string {
    const labels: Record<string, string> = { success: 'Payée', pending: 'En attente', error: 'Impayée' };
    return labels[status] ?? status;
  }
}
