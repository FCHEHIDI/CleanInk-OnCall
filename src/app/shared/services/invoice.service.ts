import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../core/services/api.service';
import { PagedResult } from './call.service';

export interface InvoiceDto {
  id: string;
  patientId: string;
  encounterId?: string;
  reference: string;
  amountCents: number;
  vatCents: number;
  status: 'Draft' | 'Issued' | 'Paid' | 'Cancelled';
  issuedAt: string;
  dueAt?: string;
  paidAt?: string;
  updatedAt: string;
}

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  constructor(private api: ApiService) {}

  getInvoices(patientId?: string, status?: string, page = 1, pageSize = 20): Observable<PagedResult<InvoiceDto>> {
    const params: Record<string, string> = { page: String(page), pageSize: String(pageSize) };
    if (patientId) params['patientId'] = patientId;
    if (status) params['status'] = status;
    return this.api.get<PagedResult<InvoiceDto>>('/api/invoices', params);
  }

  getById(id: string): Observable<InvoiceDto> {
    return this.api.get<InvoiceDto>(`/api/invoices/${id}`);
  }
}
