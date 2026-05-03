import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { map } from 'rxjs/operators';
import { ConfigService } from './config.service';

export interface User {
  id: string;
  email: string;
  name: string;
  role: string;
}

/** Matches CleanInk.OnCall.Application.Auth.DTOs.TokenResponse */
interface TokenResponse {
  accessToken: string;
  expiresAt: string;
  userId: string;
  email: string;
  fullName: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'ci_access_token';
  private readonly USER_KEY = 'ci_user';

  private currentUserSubject = new BehaviorSubject<User | null>(this.loadUser());
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router, private config: ConfigService) {}

  get isLoggedIn(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }

  get token(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  get currentUser(): User | null {
    return this.currentUserSubject.value;
  }

  login(email: string, password: string): Observable<User> {
    return this.http
      .post<TokenResponse>(`${this.config.apiUrl}/api/auth/login`, { email, password })
      .pipe(
        tap((res) => {
          const user: User = {
            id: res.userId,
            email: res.email,
            name: res.fullName,
            role: res.role,
          };
          localStorage.setItem(this.TOKEN_KEY, res.accessToken);
          localStorage.setItem(this.USER_KEY, JSON.stringify(user));
          this.currentUserSubject.next(user);
        }),
        map((res) => ({
          id: res.userId,
          email: res.email,
          name: res.fullName,
          role: res.role,
        }))
      );
  }

  register(
    email: string,
    password: string,
    firstName: string,
    lastName: string,
    role: string
  ): Observable<User> {
    return this.http
      .post<TokenResponse>(`${this.config.apiUrl}/api/auth/register`, {
        email,
        password,
        firstName,
        lastName,
        role,
      })
      .pipe(
        tap((res) => {
          const user: User = {
            id: res.userId,
            email: res.email,
            name: res.fullName,
            role: res.role,
          };
          localStorage.setItem(this.TOKEN_KEY, res.accessToken);
          localStorage.setItem(this.USER_KEY, JSON.stringify(user));
          this.currentUserSubject.next(user);
        }),
        map((res) => ({
          id: res.userId,
          email: res.email,
          name: res.fullName,
          role: res.role,
        }))
      );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  private loadUser(): User | null {
    try {
      const raw = localStorage.getItem(this.USER_KEY);
      return raw ? (JSON.parse(raw) as User) : null;
    } catch {
      return null;
    }
  }
}

