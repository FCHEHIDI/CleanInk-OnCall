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
    <div class="login-page">
      <div class="login-card">

        <!-- Logo -->
        <div class="login-brand">
          <div class="login-logo-wrap">
            <img src="assets/images/CleanInkLogo.png" alt="CleanInk OnCall" class="login-logo" />
          </div>
          <h1 class="login-title">CleanInk OnCall</h1>
          <p class="login-subtitle">Connectez-vous à votre espace</p>
        </div>

        <!-- Form -->
        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="login-form">

          <mat-form-field appearance="outline" class="w-full">
            <mat-label>Adresse email</mat-label>
            <input matInput type="email" formControlName="email" placeholder="vous@cleanink.fr" />
            <mat-icon matPrefix>mail_outline</mat-icon>
            <mat-error *ngIf="form.get('email')?.hasError('required')">Email requis</mat-error>
            <mat-error *ngIf="form.get('email')?.hasError('email')">Email invalide</mat-error>
          </mat-form-field>

          <mat-form-field appearance="outline" class="w-full">
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
            color="primary"
            type="submit"
            class="login-btn"
            [disabled]="form.invalid || loading"
          >
            <mat-spinner diameter="18" *ngIf="loading"></mat-spinner>
            <span *ngIf="!loading">Se connecter</span>
          </button>

          <p *ngIf="error" class="login-error">{{ error }}</p>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .login-page {
      min-height: 100vh;
      background: linear-gradient(135deg, var(--color-primary-dk) 0%, var(--color-primary) 100%);
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 1rem;
    }
    .login-card {
      background: var(--color-surface);
      border-radius: var(--radius-lg);
      padding: 2.5rem;
      width: 100%;
      max-width: 420px;
      box-shadow: var(--shadow-lg);
    }
    .login-brand {
      text-align: center;
      margin-bottom: 2rem;
    }
    .login-logo-wrap {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 1.25rem;
    }
    .login-logo {
      width: 180px;
      height: 180px;
      object-fit: contain;
    }
    .login-title {
      font-size: 1.375rem;
      font-weight: 700;
      color: var(--color-primary-dk);
      margin-bottom: 0.25rem;
    }
    .login-subtitle {
      font-size: 0.875rem;
      color: var(--color-text-light);
    }
    .login-form {
      display: flex;
      flex-direction: column;
      gap: 0.75rem;
    }
    .w-full { width: 100%; }
    .login-btn {
      width: 100%;
      height: 44px;
      font-weight: 600;
      font-size: 0.9rem;
      margin-top: 0.5rem;
    }
    .login-error {
      color: var(--color-warn);
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
