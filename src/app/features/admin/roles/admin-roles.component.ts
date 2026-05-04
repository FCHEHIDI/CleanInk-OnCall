import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { PageHeaderComponent } from '../../../shared/components/page-header/page-header.component';

interface RoleEntry {
  name: string;
  code: string;
  description: string;
  permissions: string[];
}

@Component({
  selector: 'app-admin-roles',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule, MatDividerModule, PageHeaderComponent],
  template: `
    <div class="fn-scene">
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_sall_des_audits.png)'">
      </div>
      <div class="fn-scene__veil"></div>
      <div class="fn-scene__deco fn-scene__deco--fougere-tl">
        <img src="assets/images/motif_fougere.png" alt="" />
      </div>

      <div class="fn-scene__content">
        <app-page-header title="Rôles & Permissions" subtitle="Configuration des accès par rôle">
          <button mat-stroked-button routerLink="/admin">
            <mat-icon>arrow_back</mat-icon>
            Retour
          </button>
        </app-page-header>

        <div class="fn-roles-grid">
          <div class="fn-card fn-role-card" *ngFor="let role of roles">
            <div class="fn-role-header">
              <mat-icon class="fn-role-icon">{{ roleIcon(role.code) }}</mat-icon>
              <div>
                <strong class="fn-role-name">{{ role.name }}</strong>
                <p class="fn-role-desc">{{ role.description }}</p>
              </div>
            </div>
            <mat-divider class="my-3"></mat-divider>
            <ul class="fn-perm-list">
              <li *ngFor="let perm of role.permissions">
                <mat-icon class="perm-icon">check</mat-icon>
                {{ perm }}
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .fn-scene { position: relative; overflow: hidden; }
    .fn-scene__bg  { position: absolute; inset: 0; background-size: cover; background-position: center; opacity: .4; pointer-events: none; z-index: 0; }
    .fn-scene__veil { position: absolute; inset: 0; background: linear-gradient(170deg, rgba(4,7,15,.65) 0%, rgba(8,14,28,.40) 50%, rgba(6,10,22,.62) 100%); pointer-events: none; z-index: 1; }
    .fn-scene__deco { position: absolute; pointer-events: none; z-index: 2; img { width: 100%; height: 100%; object-fit: contain; display: block; } }
    .fn-scene__deco--fougere-tl { width: 420px; height: 420px; top: -120px; left: -100px; opacity: .055; transform: rotate(-15deg); filter: brightness(1.8) blur(1px); }
    .fn-scene__content { position: relative; z-index: 3; padding: var(--scene-pad-y, 1.75rem) var(--scene-pad-x, 2rem); }
    .fn-roles-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 20px; }
    .fn-role-card { padding: 20px; }
    .fn-role-header { display: flex; gap: 16px; align-items: flex-start; }
    .fn-role-icon { font-size: 32px; width: 32px; height: 32px; color: var(--fn-accent); opacity: .8; }
    .fn-role-name { font-size: 1rem; font-weight: 600; display: block; }
    .fn-role-desc { font-size: 0.8rem; color: var(--fn-text-dim); margin: 2px 0 0; }
    .fn-perm-list { list-style: none; padding: 0; margin: 0; display: flex; flex-direction: column; gap: 6px; }
    .fn-perm-list li { display: flex; align-items: center; gap: 6px; font-size: 0.85rem; }
    .perm-icon { font-size: 14px; width: 14px; height: 14px; color: #00c864; }
  `],
})
export class AdminRolesComponent {
  roles: RoleEntry[] = [
    {
      name: 'Administrateur',
      code: 'admin',
      description: 'Accès complet à toutes les fonctionnalités de la plateforme.',
      permissions: ['Gestion des utilisateurs', 'Journal d\'audit', 'Configuration', 'Toutes les données'],
    },
    {
      name: 'Médecin',
      code: 'doctor',
      description: 'Accès clinique complet — consultations, prescriptions, dossiers patients.',
      permissions: ['Dossiers patients', 'Consultations', 'Notes cliniques', 'Facturation (lecture)'],
    },
    {
      name: 'Infirmier(ère)',
      code: 'nurse',
      description: 'Accès clinique partiel — patients, consultations en lecture.',
      permissions: ['Dossiers patients', 'Consultations (lecture)', 'Centre d\'appels'],
    },
    {
      name: 'Agent Call Center',
      code: 'agent',
      description: 'Gestion des appels entrants et triage.',
      permissions: ['Centre d\'appels', 'Patients (recherche)', 'Assistant IA'],
    },
    {
      name: 'Facturation',
      code: 'billing',
      description: 'Accès à la gestion des factures et encaissements.',
      permissions: ['Facturation complète', 'Patients (lecture)', 'Rapports financiers'],
    },
  ];

  roleIcon(code: string): string {
    const map: Record<string, string> = {
      admin: 'admin_panel_settings',
      doctor: 'local_hospital',
      nurse: 'medical_services',
      agent: 'support_agent',
      billing: 'receipt_long',
    };
    return map[code] ?? 'person';
  }
}
