import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({ providedIn: 'root' })
export class AdminGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(): boolean | UrlTree {
    const user = this.auth.currentUser;
    if (user?.role?.toLowerCase() === 'admin') {
      return true;
    }
    return this.router.createUrlTree(['/dashboard']);
  }
}
