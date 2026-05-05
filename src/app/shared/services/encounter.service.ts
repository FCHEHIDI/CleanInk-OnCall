import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiService } from '../../core/services/api.service';
import { PagedResult } from './call.service';

export interface EncounterDto {
  id: string;
  patientId: string;
  practitionerId: string;
  status: 'InProgress' | 'Finished' | 'Cancelled';
  encounterClass: string; // AMB | EMER | IMP | HH
  reasonText?: string;
  clinicalNote?: string;
  periodStart: string;
  periodEnd?: string;
  createdAt: string;
  updatedAt: string;
}

export interface StartEncounterRequest {
  patientId: string;
  practitionerId: string;
  encounterClass: string;
  reasonText?: string;
}

export interface FinishEncounterRequest {
  clinicalNote: string;
}

export interface CancelEncounterRequest {
  reason: string;
}

@Injectable({ providedIn: 'root' })
export class EncounterService {
  constructor(private api: ApiService) {}

  getAll(page = 1, pageSize = 20, status?: string): Observable<PagedResult<EncounterDto>> {
    const params: Record<string, string> = {
      page: String(page),
      pageSize: String(pageSize),
    };
    if (status) params['status'] = status;
    return this.api.get<PagedResult<EncounterDto>>('/api/encounters', params);
  }

  getByPatient(patientId: string, status?: string): Observable<EncounterDto[]> {
    const params: Record<string, string> = {};
    if (status) params['status'] = status;
    return this.api.get<EncounterDto[]>(`encounters/patient/${patientId}`, params);
  }

  getById(id: string): Observable<EncounterDto> {
    return this.api.get<EncounterDto>(`encounters/${id}`);
  }

  start(payload: StartEncounterRequest): Observable<EncounterDto> {
    return this.api.post<EncounterDto>('encounters', payload);
  }

  finish(id: string, payload: FinishEncounterRequest): Observable<EncounterDto> {
    return this.api.post<EncounterDto>(`encounters/${id}/finish`, payload);
  }

  cancel(id: string, payload: CancelEncounterRequest): Observable<EncounterDto> {
    return this.api.post<EncounterDto>(`encounters/${id}/cancel`, payload);
  }
}
