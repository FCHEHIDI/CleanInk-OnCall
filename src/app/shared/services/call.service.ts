import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../core/services/api.service';

export interface CallDto {
  id: string;
  customerId: string;
  patientId?: string;
  operatorId?: string;
  subject: string;
  description: string;
  status: 'Pending' | 'InProgress' | 'Resolved' | 'Escalated' | 'Cancelled';
  aiTriageTag?: string;
  aiSummary?: string;
  createdAt: string;
  updatedAt: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

@Injectable({ providedIn: 'root' })
export class CallService {
  constructor(private api: ApiService) {}

  getCalls(page = 1, pageSize = 20): Observable<PagedResult<CallDto>> {
    return this.api.get<PagedResult<CallDto>>('/api/calls', {
      page: String(page),
      pageSize: String(pageSize),
    });
  }

  assignCall(callId: string, operatorId: string): Observable<void> {
    return this.api.patch<void>(`/api/calls/${callId}/assign`, { operatorId });
  }

  escalateCall(callId: string, escalatedBy: string, reason: string): Observable<void> {
    return this.api.patch<void>(`/api/calls/${callId}/escalate`, { escalatedBy, reason });
  }

  closeCall(callId: string, closedBy: string, summary: string): Observable<void> {
    return this.api.patch<void>(`/api/calls/${callId}/close`, { closedBy, summary });
  }
}
