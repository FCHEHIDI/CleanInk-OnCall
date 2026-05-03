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
    <div class="fn-scene">

      <!-- ── Décor de salle ─────────────────────────── -->
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_sall_des_audits.png)'">
      </div>
      <div class="fn-scene__veil"></div>

      <!-- Rosace géante — centre-droit, très subtile -->
      <div class="fn-scene__deco fn-scene__deco--rosace-c">
        <img src="assets/images/motif_rosace.png" alt="" />
      </div>
      <!-- Fougère — haut-gauche, petite -->
      <div class="fn-scene__deco fn-scene__deco--fougere-tl">
        <img src="assets/images/motif_fougere.png" alt="" />
      </div>
      <!-- Fougère — bas-droit, rotée -->
      <div class="fn-scene__deco fn-scene__deco--fougere-br">
        <img src="assets/images/motif_fougere.png" alt="" />
      </div>

      <!-- ── Contenu ────────────────────────────────── -->
      <div class="fn-scene__content">

    <app-page-header title="Accès Restreint" subtitle="Gestion des utilisateurs et des rôles">
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
      background-position: center center;
      opacity: .18;
      pointer-events: none;
      z-index: 0;
    }
    .fn-scene__veil {
      position: absolute;
      inset: 0;
      background: linear-gradient(
        125deg,
        rgba(4,7,15,.82) 0%,
        rgba(10,14,30,.50) 55%,
        rgba(4,7,15,.78) 100%
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

    /* Rosace géante — axe central-droit, très discrète */
    .fn-scene__deco--rosace-c {
      width: 760px; height: 760px;
      top: 50%; right: -280px;
      transform: translateY(-50%) rotate(5deg);
      opacity: .05;
      filter: brightness(2.2) hue-rotate(30deg) blur(2px);
    }
    /* Fougère — haut-gauche (déborde) */
    .fn-scene__deco--fougere-tl {
      width: 350px; height: 350px;
      top: -90px; left: -80px;
      opacity: .06;
      transform: rotate(-18deg);
      filter: brightness(1.7) hue-rotate(20deg) blur(1px);
    }
    /* Fougère — bas-droit */
    .fn-scene__deco--fougere-br {
      width: 300px; height: 300px;
      bottom: -80px; right: 40px;
      opacity: .055;
      transform: rotate(150deg);
      filter: brightness(1.7) blur(.8px);
    }

    .fn-scene__content {
      position: relative;
      z-index: 3;
      padding: var(--scene-pad-y, 1.75rem) var(--scene-pad-x, 2rem);
    }

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
      background: var(--fn-velvet-high);
      border: 1px solid var(--fn-bio-dim);
      color: var(--fn-bio);
      font-size: 0.75rem;
      font-weight: 600;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }
    .role-chip {
      background: rgba(61,232,176,.08) !important;
      color: var(--fn-bio) !important;
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
