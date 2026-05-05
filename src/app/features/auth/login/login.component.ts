import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="fn-login">

      <div class="fn-login__deco fn-login__deco--owl">
        <img src="assets/images/logo_chouette_fractale.png" alt="" />
      </div>
      <div class="fn-login__deco fn-login__deco--fougere">
        <img src="assets/images/motif_fougere_fractale.png" alt="" />
      </div>
      <div class="fn-login__deco fn-login__deco--rosace">
        <img src="assets/images/motif_rosace_cellulaire.png" alt="" />
      </div>

      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="fn-form">

        <div class="fn-field-wrap">
          <label class="fn-label">Email</label>
          <input
            class="fn-input"
            type="email"
            formControlName="email"
            placeholder="vous@cleanink.fr"
            autocomplete="email"
          />
        </div>

        <div class="fn-field-wrap">
          <label class="fn-label">Mot de passe</label>
          <input
            class="fn-input"
            [type]="showPassword ? 'text' : 'password'"
            formControlName="password"
            autocomplete="current-password"
          />
          <button type="button" class="fn-eye" (click)="showPassword = !showPassword">
            {{ showPassword ? '&#128584;' : '&#128065;' }}
          </button>
        </div>

        <button type="submit" class="fn-btn" [disabled]="form.invalid || loading">
          <span *ngIf="loading" class="fn-spinner"></span>
          <span *ngIf="!loading">Se connecter</span>
        </button>

        <p *ngIf="error" class="fn-error">{{ error }}</p>
      </form>

      <div class="fn-logo-bottom">
        <img src="assets/images/logo_cioc.png" alt="CleanInk OnCall" />
      </div>

    </div>
  `,
  styles: [`
    .fn-login {
      min-height: 100vh;
      background: #060c1a;
      display: flex;
      align-items: center;
      justify-content: center;
      position: relative;
      overflow: hidden;
    }

    .fn-login__deco { position: absolute; pointer-events: none; }
    .fn-login__deco img { display: block; object-fit: contain; }
    .fn-login__deco--owl   { top: -180px; left: 50%; transform: translateX(-50%); opacity: .07; }
    .fn-login__deco--owl img { width: 700px; height: 700px; }
    .fn-login__deco--fougere { bottom: -140px; left: -120px; transform: rotate(-15deg); opacity: .09; }
    .fn-login__deco--fougere img { width: 560px; height: 560px; }
    .fn-login__deco--rosace  { bottom: -100px; right: -100px; opacity: .08; }
    .fn-login__deco--rosace img { width: 480px; height: 480px; }

    .fn-form {
      position: relative;
      z-index: 3;
      display: flex;
      flex-direction: column;
      gap: 1.25rem;
      width: 100%;
      max-width: 360px;
      padding: 0 1.5rem;
    }

    .fn-field-wrap {
      position: relative;
      display: flex;
      flex-direction: column;
      gap: .4rem;
    }

    .fn-label {
      font-size: .75rem;
      letter-spacing: .1em;
      text-transform: uppercase;
      color: rgba(140,165,210,.6);
    }

    .fn-input {
      background: transparent;
      border: none;
      border-bottom: 1px solid rgba(61,232,176,.25);
      outline: none;
      color: #c8d8f0;
      font-size: 1rem;
      font-family: 'Inter', sans-serif;
      padding: .55rem 0;
      width: 100%;
      transition: border-color .2s;
      caret-color: #3de8b0;
    }
    .fn-input::placeholder { color: rgba(140,165,210,.3); }
    .fn-input:focus { border-bottom-color: rgba(61,232,176,.7); }
    .fn-input:-webkit-autofill {
      -webkit-box-shadow: 0 0 0 100px #060c1a inset;
      -webkit-text-fill-color: #c8d8f0;
    }

    .fn-eye {
      position: absolute;
      right: 0;
      bottom: .4rem;
      background: none;
      border: none;
      cursor: pointer;
      font-size: .9rem;
      opacity: .5;
      padding: 0;
      line-height: 1;
    }
    .fn-eye:hover { opacity: .9; }

    .fn-btn {
      margin-top: .5rem;
      background: transparent;
      border: 1px solid rgba(61,232,176,.35);
      border-radius: 4px;
      color: #3de8b0;
      font-size: .85rem;
      font-family: 'Inter', sans-serif;
      letter-spacing: .12em;
      text-transform: uppercase;
      padding: .75rem;
      cursor: pointer;
      transition: background .2s, color .2s;
    }
    .fn-btn:hover:not([disabled]) {
      background: rgba(61,232,176,.08);
      border-color: rgba(61,232,176,.7);
    }
    .fn-btn[disabled] { opacity: .35; cursor: default; }

    .fn-spinner {
      display: inline-block;
      width: 14px; height: 14px;
      border: 2px solid rgba(61,232,176,.3);
      border-top-color: #3de8b0;
      border-radius: 50%;
      animation: spin .7s linear infinite;
    }
    @keyframes spin { to { transform: rotate(360deg); } }

    .fn-error {
      color: #ff6b7a;
      font-size: .8rem;
      text-align: center;
    }

    .fn-logo-bottom {
      position: fixed;
      bottom: 2rem;
      left: 50%;
      transform: translateX(-50%);
      z-index: 3;
      opacity: .55;
      transition: opacity .2s;
    }
    .fn-logo-bottom:hover { opacity: .85; }
    .fn-logo-bottom img {
      width: 120px;
      height: 120px;
      object-fit: contain;
      filter: drop-shadow(0 0 10px rgba(61,232,176,.2));
    }
  `],
})
export class LoginComponent {
  form: FormGroup;
  showPassword = false;
  loading = false;
  error = '';

  constructor(private fb: FormBuilder, private router: Router, private auth: AuthService) {
    this.form = this.fb.group({
      email:    ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    this.error = '';
    const { email, password } = this.form.value;
    this.auth.login(email, password).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.loading = false;
        this.error = err?.error?.error ?? 'Identifiants invalides.';
      },
    });
  }
}
