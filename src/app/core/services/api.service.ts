import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigService } from './config.service';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(
    private http: HttpClient,
    private config: ConfigService
  ) {}

  get<T>(path: string, params: Record<string, string> = {}): Observable<T> {
    const httpParams = new HttpParams({ fromObject: params });
    return this.http.get<T>(`${this.config.apiUrl}${path}`, { params: httpParams });
  }

  post<T>(path: string, body: unknown): Observable<T> {
    return this.http.post<T>(`${this.config.apiUrl}${path}`, body);
  }

  put<T>(path: string, body: unknown): Observable<T> {
    return this.http.put<T>(`${this.config.apiUrl}${path}`, body);
  }

  patch<T>(path: string, body: unknown): Observable<T> {
    return this.http.patch<T>(`${this.config.apiUrl}${path}`, body);
  }

  delete<T>(path: string): Observable<T> {
    return this.http.delete<T>(`${this.config.apiUrl}${path}`);
  }
}
