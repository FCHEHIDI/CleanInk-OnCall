import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { StatusBadgeComponent } from '../../shared/components/status-badge/status-badge.component';

interface UserEntry {
  id: string;
  name: string;
  email: string;
  role: string;
  status: 'active' | 'inactive';
}

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    PageHeaderComponent,
    StatusBadgeComponent,
  ],
  template: `
    <app-page-header title="Administration" subtitle="Gestion des utilisateurs et des rôles">
      <button mat-raised-button color="primary">
        <mat-icon>person_add</mat-icon>
        Ajouter un utilisateur
      </button>
    </app-page-header>

    <div class="ci-card">
      <table mat-table [dataSource]="users" class="w-full">

        <ng-container matColumnDef="name">
          <th mat-header-cell *matHeaderCellDef>Nom</th>
          <td mat-cell *matCellDef="let row">
            <div class="user-cell">
              <div class="user-avatar">{{ initials(row.name) }}</div>
              <div>
                <strong>{{ row.name }}</strong>
                <div class="text-xs text-gray-400">{{ row.email }}</div>
              </div>
            </div>
          </td>
        </ng-container>

        <ng-container matColumnDef="role">
          <th mat-header-cell *matHeaderCellDef>Rôle</th>
          <td mat-cell *matCellDef="let row">
            <mat-chip class="role-chip">{{ row.role }}</mat-chip>
          </td>
        </ng-container>

        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Statut</th>
          <td mat-cell *matCellDef="let row">
            <app-status-badge [status]="row.status" [label]="row.status === 'active' ? 'Actif' : 'Inactif'"></app-status-badge>
          </td>
        </ng-container>

        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef></th>
          <td mat-cell *matCellDef="let row">
            <button mat-icon-button matTooltip="Modifier"><mat-icon>edit</mat-icon></button>
            <button mat-icon-button matTooltip="Désactiver" color="warn"><mat-icon>block</mat-icon></button>
          </td>
        </ng-container>

        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
      </table>
    </div>
  `,
  styles: [`
    .w-full { width: 100%; }
    .user-cell {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.5rem 0;
    }
    .user-avatar {
      width: 36px; height: 36px;
      border-radius: 50%;
      background: var(--color-primary);
      color: #fff;
      font-size: 0.75rem;
      font-weight: 600;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }
    .role-chip {
      background: rgba(30,90,168,.1) !important;
      color: var(--color-primary) !important;
      font-size: 0.75rem !important;
    }
  `],
})
export class AdminComponent {
  displayedColumns = ['name', 'role', 'status', 'actions'];

  users: UserEntry[] = [
    { id: '1', name: 'Fares Chehidi',     email: 'fares@cleanink.fr',   role: 'Administrateur', status: 'active'   },
    { id: '2', name: 'Léa Martin',        email: 'lea@cleanink.fr',     role: 'Agent',          status: 'active'   },
    { id: '3', name: 'Tom Rousseau',      email: 'tom@cleanink.fr',     role: 'Agent',          status: 'active'   },
    { id: '4', name: 'Ana Sousa',         email: 'ana@cleanink.fr',     role: 'Facturation',    status: 'active'   },
    { id: '5', name: 'Pierre Legrand',    email: 'pierre@cleanink.fr',  role: 'Agent',          status: 'inactive' },
  ];

  initials(name: string): string {
    return name.split(' ').map((n) => n[0]).slice(0, 2).join('').toUpperCase();
  }
}
