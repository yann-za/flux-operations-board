import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Flux, Alert, DashboardMetrics, PaginatedList,
  CreateFluxRequest, FluxStatus, FluxType, AlertSeverity
} from '../models/flux.model';

@Injectable({ providedIn: 'root' })
export class FluxService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;

  // --- Flux endpoints ---

  getFluxes(options?: {
    pageNumber?: number;
    pageSize?: number;
    status?: FluxStatus;
    type?: FluxType;
    search?: string;
    includeArchived?: boolean;
  }): Observable<PaginatedList<Flux>> {
    let params = new HttpParams();
    if (options?.pageNumber) params = params.set('pageNumber', options.pageNumber);
    if (options?.pageSize) params = params.set('pageSize', options.pageSize);
    if (options?.status) params = params.set('status', options.status);
    if (options?.type) params = params.set('type', options.type);
    if (options?.search) params = params.set('search', options.search);
    if (options?.includeArchived) params = params.set('includeArchived', true);
    return this.http.get<PaginatedList<Flux>>(`${this.base}/flux`, { params });
  }

  getFluxById(id: string): Observable<Flux> {
    return this.http.get<Flux>(`${this.base}/flux/${id}`);
  }

  createFlux(request: CreateFluxRequest): Observable<Flux> {
    return this.http.post<Flux>(`${this.base}/flux`, request);
  }

  updateFlux(id: string, request: Partial<CreateFluxRequest>): Observable<Flux> {
    return this.http.put<Flux>(`${this.base}/flux/${id}`, { id, ...request });
  }

  deleteFlux(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/flux/${id}`);
  }

  pauseFlux(id: string): Observable<void> {
    return this.http.post<void>(`${this.base}/flux/${id}/pause`, {});
  }

  resumeFlux(id: string): Observable<void> {
    return this.http.post<void>(`${this.base}/flux/${id}/resume`, {});
  }

  // --- Alert endpoints ---

  getActiveAlerts(options?: { fluxId?: string; severity?: AlertSeverity }): Observable<Alert[]> {
    let params = new HttpParams();
    if (options?.fluxId) params = params.set('fluxId', options.fluxId);
    if (options?.severity) params = params.set('severity', options.severity);
    return this.http.get<Alert[]>(`${this.base}/alert`, { params });
  }

  resolveAlert(id: string, resolvedBy: string, resolutionNote?: string): Observable<void> {
    return this.http.post<void>(`${this.base}/alert/${id}/resolve`, { resolvedBy, resolutionNote });
  }

  // --- Dashboard endpoint ---

  getDashboardMetrics(): Observable<DashboardMetrics> {
    return this.http.get<DashboardMetrics>(`${this.base}/dashboard/metrics`);
  }
}
