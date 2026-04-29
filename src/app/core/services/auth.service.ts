import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';

export interface User {
  id: string;
  email: string;
  name: string;
  role: 'admin' | 'agent' | 'billing';
  avatar?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  user: User;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'ci_access_token';
  private readonly REFRESH_KEY = 'ci_refresh_token';

  private currentUserSubject = new BehaviorSubject<User | null>(this.loadUser());
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  get isLoggedIn(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }

  get token(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  get currentUser(): User | null {
    return this.currentUserSubject.value;
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>('/api/auth/login', { email, password }).pipe(
      tap((res) => {
        localStorage.setItem(this.TOKEN_KEY, res.accessToken);
        localStorage.setItem(this.REFRESH_KEY, res.refreshToken);
        localStorage.setItem('ci_user', JSON.stringify(res.user));
        this.currentUserSubject.next(res.user);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_KEY);
    localStorage.removeItem('ci_user');
    this.currentUserSubject.next(null);
    this.router.navigate(['/auth/login']);
  }

  private loadUser(): User | null {
    try {
      const raw = localStorage.getItem('ci_user');
      return raw ? (JSON.parse(raw) as User) : null;
    } catch {
      return null;
    }
  }
}
