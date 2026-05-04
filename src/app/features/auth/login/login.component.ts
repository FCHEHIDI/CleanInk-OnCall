import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatFormFieldModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  template: `
    <!-- ── Fractal Nocturne login scene ── -->
    <div class="fn-login">

      <!-- Veil gradient -->
      <div class="fn-login__veil"></div>

      <!-- Giant fractal decos -->
      <div class="fn-login__deco fn-login__deco--owl">
        <img src="assets/images/logo_chouette_fractale.png" alt="" />
      </div>
      <div class="fn-login__deco fn-login__deco--fougere">
        <img src="assets/images/motif_fougere_fractale.png" alt="" />
      </div>
      <div class="fn-login__deco fn-login__deco--rosace">
        <img src="assets/images/motif_rosace_cellulaire.png" alt="" />
      </div>

      <!-- Card -->
      <div class="fn-login__card">

        <!-- Logo + brand -->
        <div class="fn-login__brand">
          <div class="fn-login__logo-wrap">
            <img src="assets/images/logo_cioc.png" alt="CleanInk OnCall" class="fn-login__logo" />
          </div>
          <h1 class="fn-login__title">CleanInk OnCall</h1>
          <p class="fn-login__subtitle">Connectez-vous à votre espace</p>
        </div>

        <!-- Nervure divider -->
        <div class="fn-login__divider">
          <svg width="100%" height="1" viewBox="0 0 300 1" preserveAspectRatio="none">
            <line x1="0" y1="0" x2="300" y2="0"
              stroke="rgba(61,232,176,.18)" stroke-width="1" stroke-dasharray="4 6"/>
          </svg>
        </div>

        <!-- Form -->
        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="fn-login__form">

          <mat-form-field appearance="outline" class="fn-field">
            <mat-label>Adresse email</mat-label>
            <input matInput type="email" formControlName="email" placeholder="vous@cleanink.fr" />
            <mat-icon matPrefix>mail_outline</mat-icon>
            <mat-error *ngIf="form.get('email')?.hasError('required')">Email requis</mat-error>
            <mat-error *ngIf="form.get('email')?.hasError('email')">Email invalide</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="fn-field">
            <mat-label>Mot de passe</mat-label>
            <input matInput [type]="showPassword ? 'text' : 'password'" formControlName="password" />
            <mat-icon matPrefix>lock_outline</mat-icon>
            <button mat-icon-button matSuffix type="button" (click)="showPassword = !showPassword">
              <mat-icon>{{ showPassword ? 'visibility_off' : 'visibility' }}</mat-icon>
            </button>
            <mat-error *ngIf="form.get('password')?.hasError('required')">Mot de passe requis</mat-error>
          </mat-form-field>

          <button
            mat-raised-button
            type="submit"
            class="fn-login__btn"
            [disabled]="form.invalid || loading"
          >
            <mat-spinner diameter="18" *ngIf="loading"></mat-spinner>
            <span *ngIf="!loading">Se connecter</span>
          </button>

          <p *ngIf="error" class="fn-login__error">{{ error }}</p>
        </form>
      </div>
    </div>
  `,
  styles: [`
    /* ── Page shell ── */
    .fn-login {
      min-height: 100vh;
      background: #060c1a;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 1.5rem;
      position: relative;
      overflow: hidden;
    }

    /* Atmospheric veil */
    .fn-login__veil {
      position: absolute;
      inset: 0;
      background:
        radial-gradient(ellipse 80% 60% at 20% 80%, rgba(30,90,168,.18) 0%, transparent 60%),
        radial-gradient(ellipse 60% 50% at 80% 20%, rgba(61,232,176,.06) 0%, transparent 55%),
        linear-gradient(160deg, rgba(6,12,26,.95) 0%, rgba(10,18,40,.85) 100%);
      pointer-events: none;
      z-index: 1;
    }

    /* ── Giant fractal decos ── */
    .fn-login__deco {
      position: absolute;
      pointer-events: none;
      z-index: 2;
      overflow: visible;
    }
    .fn-login__deco img {
      display: block;
      object-fit: contain;
    }

    /* Chouette — centrée en haut, géante, déborde */
    .fn-login__deco--owl {
      top: -180px;
      left: 50%;
      transform: translateX(-50%);
      opacity: 0.07;
    }
    .fn-login__deco--owl img { width: 700px; height: 700px; }

    /* Fougère — bas gauche, rotée */
    .fn-login__deco--fougere {
      bottom: -140px;
      left: -120px;
      transform: rotate(-15deg);
      opacity: 0.09;
    }
    .fn-login__deco--fougere img { width: 560px; height: 560px; }

    /* Rosace — bas droite */
    .fn-login__deco--rosace {
      bottom: -100px;
      right: -100px;
      opacity: 0.08;
    }
    .fn-login__deco--rosace img { width: 480px; height: 480px; }

    /* ── Card ── */
    .fn-login__card {
      position: relative;
      z-index: 3;
      background: rgba(13,21,48,.82);
      border: 1px solid rgba(61,232,176,.12);
      border-radius: 16px;
      padding: 2.75rem 2.5rem 2.25rem;
      width: 100%;
      max-width: 440px;
      backdrop-filter: blur(18px);
      box-shadow:
        0 0 0 1px rgba(61,232,176,.05),
        0 24px 64px rgba(0,0,0,.55),
        0 0 80px rgba(30,90,168,.1);
    }

    /* ── Brand ── */
    .fn-login__brand {
      text-align: center;
      margin-bottom: 1.75rem;
    }
    .fn-login__logo-wrap {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 1.25rem;
      filter: drop-shadow(0 0 20px rgba(61,232,176,.25));
    }
    .fn-login__logo {
      width: 220px;
      height: 220px;
      object-fit: contain;
    }
    .fn-login__title {
      font-family: 'Cormorant Garamond', Georgia, serif;
      font-size: 1.5rem;
      font-weight: 700;
      color: #c8d8f0;
      letter-spacing: .04em;
      margin-bottom: 0.3rem;
    }
    .fn-login__subtitle {
      font-size: 0.82rem;
      color: rgba(140,165,210,.7);
      letter-spacing: .06em;
      text-transform: uppercase;
    }

    /* Dashed divider */
    .fn-login__divider {
      margin-bottom: 1.5rem;
      opacity: .7;
    }

    /* ── Form ── */
    .fn-login__form {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }
    .fn-field { width: 100%; }

    /* Override Material outline field for dark theme */
    ::ng-deep .fn-login__form .mat-mdc-form-field {
      --mdc-outlined-text-field-outline-color: rgba(61,232,176,.2);
      --mdc-outlined-text-field-hover-outline-color: rgba(61,232,176,.45);
      --mdc-outlined-text-field-focus-outline-color: rgba(61,232,176,.7);
      --mdc-outlined-text-field-label-text-color: rgba(140,165,210,.7);
      --mdc-outlined-text-field-input-text-color: #c8d8f0;
      --mdc-outlined-text-field-caret-color: #3de8b0;
      --mat-icon-color: rgba(140,165,210,.6);
    }

    .fn-login__btn {
      width: 100%;
      height: 46px;
      margin-top: 0.75rem;
      font-weight: 600;
      font-size: 0.9rem;
      letter-spacing: .04em;
      background: linear-gradient(90deg, #1a5aa8 0%, #1e6fd4 100%) !important;
      color: #fff !important;
      border-radius: 8px !important;
      border: 1px solid rgba(61,232,176,.25) !important;
      box-shadow: 0 0 20px rgba(30,90,168,.35) !important;
      transition: box-shadow .2s, opacity .2s;

      &:hover:not([disabled]) {
        box-shadow: 0 0 30px rgba(30,90,168,.55) !important;
        opacity: .92;
      }
      &[disabled] { opacity: .45 !important; }
    }

    .fn-login__error {
      color: #ff6b7a;
      font-size: 0.8rem;
      text-align: center;
      margin-top: 0.5rem;
    }
  `],
})
export class LoginComponent {
  form: FormGroup;
  showPassword = false;
  loading = false;
  error = '';

  constructor(private fb: FormBuilder, private router: Router) {
    this.form = this.fb.group({
      email:    ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    this.error = '';

    // Simulated login — à remplacer par AuthService.login()
    setTimeout(() => {
      this.loading = false;
      // Fake token pour débloquer AuthGuard (dev only)
      localStorage.setItem('ci_access_token', 'dev-token');
      localStorage.setItem('ci_user', JSON.stringify({
        id: '1',
        email: this.form.value.email,
        name: 'Dev User',
        role: 'admin',
      }));
      this.router.navigate(['/dashboard']);
    }, 1000);
  }
}
