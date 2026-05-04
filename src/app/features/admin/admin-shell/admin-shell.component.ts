import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-admin-shell',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTabsModule, MatIconModule],
  template: `
    <div class="fn-scene">
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_sall_des_audits.png)'">
      </div>
      <div class="fn-scene__veil"></div>
      <div class="fn-scene__deco fn-scene__deco--rosace-c">
        <img src="assets/images/motif_rosace.png" alt="" />
      </div>
      <div class="fn-scene__deco fn-scene__deco--fougere-tl">
        <img src="assets/images/motif_fougere.png" alt="" />
      </div>

      <div class="fn-scene__content">
        <nav class="fn-admin-tabs">
          <a routerLink="users" routerLinkActive="active" class="fn-admin-tab">
            <mat-icon>manage_accounts</mat-icon> Utilisateurs
          </a>
          <a routerLink="roles" routerLinkActive="active" class="fn-admin-tab">
            <mat-icon>security</mat-icon> Rôles & permissions
          </a>
        </nav>
        <router-outlet></router-outlet>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .fn-scene { position: relative; overflow: hidden; }
    .fn-scene__bg  { position: absolute; inset: 0; background-size: cover; background-position: center; opacity: .4; pointer-events: none; z-index: 0; }
    .fn-scene__veil { position: absolute; inset: 0; background: linear-gradient(170deg, rgba(4,7,15,.65) 0%, rgba(8,14,28,.40) 50%, rgba(6,10,22,.62) 100%); pointer-events: none; z-index: 1; }
    .fn-scene__deco { position: absolute; pointer-events: none; z-index: 2; img { width: 100%; height: 100%; object-fit: contain; display: block; } }
    .fn-scene__deco--rosace-c { width: 480px; height: 480px; top: -140px; right: -140px; opacity: .06; filter: brightness(2) blur(1px); }
    .fn-scene__deco--fougere-tl { width: 380px; height: 380px; top: -100px; left: -80px; opacity: .055; transform: rotate(-15deg); filter: brightness(1.8) blur(1px); }
    .fn-scene__content { position: relative; z-index: 3; padding: var(--scene-pad-y, 1.75rem) var(--scene-pad-x, 2rem); }
    .fn-admin-tabs { display: flex; gap: 4px; margin-bottom: 24px; }
    .fn-admin-tab { display: flex; align-items: center; gap: 8px; padding: 10px 20px;
      border-radius: 8px 8px 0 0; color: var(--fn-text-dim); text-decoration: none;
      font-size: 0.9rem; font-weight: 500; transition: background .2s, color .2s;
      border: 1px solid transparent; }
    .fn-admin-tab:hover { background: rgba(30,90,168,.1); color: var(--fn-accent); }
    .fn-admin-tab.active { background: rgba(30,90,168,.18); color: var(--fn-accent);
      border-color: rgba(30,90,168,.25); }
    .fn-admin-tab mat-icon { font-size: 18px; width: 18px; height: 18px; }
  `],
})
export class AdminShellComponent {}
