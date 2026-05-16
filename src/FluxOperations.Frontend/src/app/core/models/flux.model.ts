export type FluxStatus = 'Inactive' | 'Active' | 'Warning' | 'Error' | 'Paused' | 'Completed';
export type FluxType = 'DataPipeline' | 'ETL' | 'ApiIntegration' | 'BatchProcessing' | 'Streaming' | 'FileTransfer' | 'MessageQueue';
export type AlertSeverity = 'Info' | 'Warning' | 'Critical';

export interface Flux {
  id: string;
  name: string;
  description?: string;
  type: FluxType;
  status: FluxStatus;
  sourceSystem?: string;
  targetSystem?: string;
  scheduleCron?: string;
  lastExecutedAt?: string;
  nextExecutionAt?: string;
  throughputPerHour?: number;
  errorRatePercent?: number;
  isArchived: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface Alert {
  id: string;
  fluxId: string;
  fluxName?: string;
  severity: AlertSeverity;
  message: string;
  isResolved: boolean;
  resolvedAt?: string;
  resolvedBy?: string;
  resolutionNote?: string;
  createdAt: string;
}

export interface FluxStatusSummary {
  status: string;
  count: number;
  color: string;
}

export interface RecentAlert {
  alertId: string;
  fluxId: string;
  fluxName: string;
  severity: string;
  message: string;
  occurredAt: string;
}

export interface DashboardMetrics {
  totalFluxes: number;
  activeFluxes: number;
  fluxesInError: number;
  fluxesInWarning: number;
  activeAlerts: number;
  criticalAlerts: number;
  totalThroughputPerHour: number;
  averageErrorRate: number;
  fluxStatusBreakdown: FluxStatusSummary[];
  recentAlerts: RecentAlert[];
}

export interface PaginatedList<T> {
  items: T[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface CreateFluxRequest {
  name: string;
  type: FluxType;
  description?: string;
  sourceSystem?: string;
  targetSystem?: string;
  scheduleCron?: string;
}
