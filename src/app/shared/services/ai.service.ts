import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../core/services/api.service';

export interface AiResponse {
  result: string;
}

@Injectable({ providedIn: 'root' })
export class AiService {
  constructor(private api: ApiService) {}

  triage(text: string): Observable<AiResponse> {
    return this.api.post<AiResponse>('/api/ai/triage', { text });
  }

  summary(text: string): Observable<AiResponse> {
    return this.api.post<AiResponse>('/api/ai/summary', { text });
  }

  compliance(text: string): Observable<AiResponse> {
    return this.api.post<AiResponse>('/api/ai/compliance', { text });
  }
}
