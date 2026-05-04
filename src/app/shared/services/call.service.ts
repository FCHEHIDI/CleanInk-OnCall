import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../core/services/api.service';

export interface CallDto {
  id: string;
  createdByUserId: string;
  assignedPractitionerId?: string;
  encounterId?: string;
  patientId?: string;
  subject: string;
  description: string;
  priority: string;
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

  getById(id: string): Observable<CallDto> {
    return this.api.get<CallDto>(`/api/calls/${id}`);
  }

  assignCall(callId: string, practitionerId: string): Observable<void> {
    return this.api.patch<void>(`/api/calls/${callId}/assign`, { practitionerId });
  }

  escalateCall(callId: string, reason: string): Observable<void> {
    return this.api.patch<void>(`/api/calls/${callId}/escalate`, { reason });
  }

  closeCall(callId: string, resolutionNote?: string): Observable<void> {
    return this.api.patch<void>(`/api/calls/${callId}/close`, { resolutionNote });
  }
}
