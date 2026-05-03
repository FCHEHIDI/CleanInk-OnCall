import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../core/services/api.service';
import { PagedResult } from './call.service';

export interface InvoiceDto {
  id: string;
  callId: string;
  customerId: string;
  amount: number;
  currency: string;
  status: 'Draft' | 'Issued' | 'Paid' | 'Overdue' | 'Cancelled';
  issuedAt?: string;
  paidAt?: string;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  constructor(private api: ApiService) {}

  getInvoices(page = 1, pageSize = 20): Observable<PagedResult<InvoiceDto>> {
    return this.api.get<PagedResult<InvoiceDto>>('/api/invoices', {
      page: String(page),
      pageSize: String(pageSize),
    });
  }
}
