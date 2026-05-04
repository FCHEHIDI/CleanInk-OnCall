import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { PageHeaderComponent } from '../../shared/components/page-header/page-header.component';
import { AiService } from '../../shared/services/ai.service';

type AiMode = 'triage' | 'summary' | 'compliance';

@Component({
  selector: 'app-ai',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatTabsModule,
    MatProgressSpinnerModule,
    PageHeaderComponent,
  ],
  template: `
    <div class="fn-scene">
      <div class="fn-scene__bg"
        [style.background-image]="'url(assets/images/backgrounds/background_dashboard.png)'">
      </div>
      <div class="fn-scene__veil"></div>
      <div class="fn-scene__deco fn-scene__deco--rosace-tr">
        <img src="assets/images/motif_rosace.png" alt="" />
      </div>
      <div class="fn-scene__deco fn-scene__deco--fougere-bl">
        <img src="assets/images/motif_fougere.png" alt="" />
      </div>

      <div class="fn-scene__content">
        <app-page-header title="Assistant IA" subtitle="Triage clinique, résumé d'appel et conformité RGPD">
        </app-page-header>

        <mat-tab-group class="fn-tabs mt-6" animationDuration="200ms" (selectedIndexChange)="onTabChange($event)">

          <!-- Triage -->
          <mat-tab label="Triage clinique">
            <div class="fn-tab-content fn-card">
              <p class="fn-hint">Saisir le texte d'un appel ou d'une description clinique pour obtenir un tag de triage automatique.</p>
              <mat-form-field appearance="outline" class="w-full mt-3">
                <mat-label>Texte à analyser</mat-label>
                <textarea matInput [(ngModel)]="inputText" rows="6" placeholder="Ex : Patient de 72 ans, douleur thoracique irradiant dans le bras gauche depuis 30 min…"></textarea>
              </mat-form-field>
              <button mat-raised-button color="primary" (click)="run('triage')" [disabled]="loading || !inputText.trim()">
                <mat-spinner *ngIf="loading && mode === 'triage'" diameter="18" style="display:inline-block;margin-right:6px"></mat-spinner>
                <mat-icon *ngIf="!loading || mode !== 'triage'">local_hospital</mat-icon>
                Analyser le triage
              </button>
              <div *ngIf="result && mode === 'triage'" class="fn-result mt-4">
                <div class="fn-result__label">Tag de triage</div>
                <div class="fn-result__content">{{ result }}</div>
              </div>
            </div>
          </mat-tab>

          <!-- Résumé -->
          <mat-tab label="Résumé d'appel">
            <div class="fn-tab-content fn-card">
              <p class="fn-hint">Générer un résumé structuré à partir de la transcription ou de la description d'un appel.</p>
              <mat-form-field appearance="outline" class="w-full mt-3">
                <mat-label>Transcription / description</mat-label>
                <textarea matInput [(ngModel)]="inputText" rows="6" placeholder="Coller ici la transcription de l'appel…"></textarea>
              </mat-form-field>
              <button mat-raised-button color="primary" (click)="run('summary')" [disabled]="loading || !inputText.trim()">
                <mat-spinner *ngIf="loading && mode === 'summary'" diameter="18" style="display:inline-block;margin-right:6px"></mat-spinner>
                <mat-icon *ngIf="!loading || mode !== 'summary'">summarize</mat-icon>
                Générer le résumé
              </button>
              <div *ngIf="result && mode === 'summary'" class="fn-result mt-4">
                <div class="fn-result__label">Résumé</div>
                <div class="fn-result__content">{{ result }}</div>
              </div>
            </div>
          </mat-tab>

          <!-- Conformité -->
          <mat-tab label="Conformité RGPD">
            <div class="fn-tab-content fn-card">
              <p class="fn-hint">Vérifier qu'un texte ne contient pas de données personnelles non conformes avant archivage ou export.</p>
              <mat-form-field appearance="outline" class="w-full mt-3">
                <mat-label>Texte à vérifier</mat-label>
                <textarea matInput [(ngModel)]="inputText" rows="6" placeholder="Coller ici le texte à auditer…"></textarea>
              </mat-form-field>
              <button mat-raised-button color="primary" (click)="run('compliance')" [disabled]="loading || !inputText.trim()">
                <mat-spinner *ngIf="loading && mode === 'compliance'" diameter="18" style="display:inline-block;margin-right:6px"></mat-spinner>
                <mat-icon *ngIf="!loading || mode !== 'compliance'">policy</mat-icon>
                Auditer la conformité
              </button>
              <div *ngIf="result && mode === 'compliance'" class="fn-result mt-4">
                <div class="fn-result__label">Rapport de conformité</div>
                <div class="fn-result__content">{{ result }}</div>
              </div>
            </div>
          </mat-tab>

        </mat-tab-group>

        <div *ngIf="error" class="fn-error mt-4">{{ error }}</div>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .fn-scene { position: relative; min-height: calc(100vh - 52px); overflow: hidden; margin: calc(-1 * var(--scene-pad-y, 1.75rem)) calc(-1 * var(--scene-pad-x, 2rem)); }
    .fn-scene__bg { position: absolute; inset: 0; background-size: cover; background-position: center; opacity: .4; pointer-events: none; z-index: 0; }
    .fn-scene__veil { position: absolute; inset: 0; background: linear-gradient(170deg, rgba(4,7,15,.62) 0%, rgba(8,14,28,.38) 50%, rgba(6,10,22,.60) 100%); pointer-events: none; z-index: 1; }
    .fn-scene__deco { position: absolute; pointer-events: none; z-index: 2; img { width: 100%; height: 100%; object-fit: contain; display: block; } }
    .fn-scene__deco--rosace-tr { width: 460px; height: 460px; top: -120px; right: -140px; opacity: .07; filter: brightness(2) hue-rotate(35deg) blur(1px); }
    .fn-scene__deco--fougere-bl { width: 380px; height: 380px; bottom: -100px; left: -80px; opacity: .055; transform: rotate(40deg); filter: brightness(1.8) blur(1px); }
    .fn-scene__content { position: relative; z-index: 3; padding: var(--scene-pad-y, 1.75rem) var(--scene-pad-x, 2rem); }
    .fn-tabs { margin-top: 1.5rem; }
    .fn-tab-content { margin-top: .75rem; }
    .fn-card { background: rgba(10,18,40,.7); border: 1px solid rgba(28,47,90,.5); border-radius: var(--fn-r-lg, 12px); padding: 1.5rem; backdrop-filter: blur(8px); }
    .fn-hint { color: var(--fn-text-dim); font-size: .85rem; margin: 0; }
    .w-full { width: 100%; }
    .mt-3 { margin-top: .75rem; }
    .mt-4 { margin-top: 1rem; }
    .fn-result { background: rgba(61,232,176,.06); border: 1px solid rgba(61,232,176,.18); border-radius: var(--fn-r-md, 8px); padding: 1rem 1.25rem; }
    .fn-result__label { font-size: .72rem; font-weight: 600; letter-spacing: .08em; text-transform: uppercase; color: var(--fn-bio); margin-bottom: .5rem; }
    .fn-result__content { color: var(--fn-text); font-size: .9rem; white-space: pre-wrap; line-height: 1.6; }
    .fn-error { color: #f87171; font-size: .875rem; }
  `],
})
export class AiComponent {
  inputText = '';
  result: string | null = null;
  error: string | null = null;
  loading = false;
  mode: AiMode = 'triage';

  constructor(private aiService: AiService) {}

  onTabChange(index: number): void {
    this.result = null;
    this.error = null;
    this.inputText = '';
    const modes: AiMode[] = ['triage', 'summary', 'compliance'];
    this.mode = modes[index] ?? 'triage';
  }

  run(mode: AiMode): void {
    const text = this.inputText.trim();
    if (!text) return;
    this.mode = mode;
    this.loading = true;
    this.result = null;
    this.error = null;

    const call$ = mode === 'triage'
      ? this.aiService.triage(text)
      : mode === 'summary'
        ? this.aiService.summary(text)
        : this.aiService.compliance(text);

    call$.subscribe({
      next: (res) => { this.result = res.result; this.loading = false; },
      error: () => { this.error = 'Erreur lors de l\'analyse IA.'; this.loading = false; },
    });
  }
}
