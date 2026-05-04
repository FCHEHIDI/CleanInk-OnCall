import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from '../../core/services/api.service';

export interface AuditEventDto {
  id: string;
  actorEmail: string;
  action: string;
  resourceType: string;
  resourceId: string;
  ipAddress?: string;
  recordedAt: string;
}

@Injectable({ providedIn: 'root' })
export class AuditService {
  constructor(private api: ApiService) {}

  getByResource(entityType: string, entityId: string): Observable<AuditEventDto[]> {
    return this.api.get<AuditEventDto[]>(`/api/audit/${entityType}/${entityId}`);
  }

  getByActor(
    actorId: string,
    from?: string,
    to?: string
  ): Observable<AuditEventDto[]> {
    const params: Record<string, string> = {};
    if (from) params['from'] = from;
    if (to) params['to'] = to;
    return this.api.get<AuditEventDto[]>(`/api/audit/actor/${actorId}`, params);
  }
}
