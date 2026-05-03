import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';
import { PagedResult } from './call.service';

export interface PatientDto {
  id: string;
  lastName: string;
  firstName: string;
  dateOfBirth: string;
  nirMasked?: string;
  phoneMasked?: string;
  emailMasked?: string;
  consent: string;
  isArchived: boolean;
  createdAt: string;
}

export interface RegisterPatientRequest {
  lastName: string;
  firstName: string;
  dateOfBirth: string;
  nir?: string;
  phone?: string;
  email?: string;
}

@Injectable({ providedIn: 'root' })
export class PatientService {
  constructor(private api: ApiService) {}

  searchPatients(q = '', page = 1, pageSize = 20): Observable<PagedResult<PatientDto>> {
    return this.api.get<PagedResult<PatientDto>>('/api/patients', {
      q,
      page: String(page),
      pageSize: String(pageSize),
    });
  }

  getById(id: string): Observable<PatientDto> {
    return this.api.get<PatientDto>(`/api/patients/${id}`);
  }

  registerPatient(payload: RegisterPatientRequest): Observable<PatientDto> {
    return this.api.post<PatientDto>('/api/patients', payload);
  }
}
