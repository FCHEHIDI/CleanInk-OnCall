import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from '../../core/services/config.service';

/** Shape of the KPI snapshot returned by GET /api/dashboard/kpis */
export interface DashboardKpis {
  callsToday: number;
  openCalls: number;
  pendingInvoices: number;
  activeUsers: number;
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly base: string;

  constructor(private http: HttpClient, config: ConfigService) {
    this.base = config.apiUrl;
  }

  /**
   * Fetches live KPI counts for the current tenant's dashboard.
   * The JWT token is injected automatically by the AuthInterceptor.
   */
  getKpis(): Observable<DashboardKpis> {
    return this.http.get<DashboardKpis>(`${this.base}/api/dashboard/kpis`);
  }
}
