import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { interval, Subscription, startWith, switchMap, catchError, of } from 'rxjs';

import { FluxService } from '../../../core/services/flux.service';
import { DashboardMetrics, Flux } from '../../../core/models/flux.model';
import { KpiCardComponent } from '../../../shared/components/kpi-card/kpi-card.component';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'fob-dashboard',
  standalone: true,
  imports: [
    CommonModule, MatCardModule, MatIconModule, MatButtonModule,
    MatChipsModule, MatProgressSpinnerModule,
    KpiCardComponent, StatusBadgeComponent
  ],
  template: `
    <div class="dashboard-container">
      <header class="dashboard-header">
        <div>
          <h1 class="dashboard-title">
            <mat-icon>dashboard</mat-icon>
            Flux Operations Board
          </h1>
          <p class="dashboard-subtitle">Real-time supervision of operational data flows</p>
        </div>
        <div class="header-actions">
          <span class="refresh-label">
            <mat-icon class="spin" *ngIf="loading">sync</mat-icon>
            Auto-refresh: {{ refreshInterval / 1000 }}s
          </span>
        </div>
      </header>

      @if (metrics) {
        <!-- KPI Row -->
        <div class="kpi-grid">
          <fob-kpi-card
            label="Total Fluxes"
            [value]="metrics.totalFluxes"
            icon="account_tree"
            accentColor="#3b82f6" />
          <fob-kpi-card
            label="Active"
            [value]="metrics.activeFluxes"
            icon="play_circle"
            accentColor="#22c55e"
            [subtitle]="metrics.totalThroughputPerHour | number" />
          <fob-kpi-card
            label="In Warning"
            [value]="metrics.fluxesInWarning"
            icon="warning"
            accentColor="#f59e0b" />
          <fob-kpi-card
            label="In Error"
            [value]="metrics.fluxesInError"
            icon="error"
            accentColor="#ef4444" />
          <fob-kpi-card
            label="Active Alerts"
            [value]="metrics.activeAlerts"
            icon="notifications_active"
            accentColor="#8b5cf6"
            [subtitle]="metrics.criticalAlerts + ' critical'" />
          <fob-kpi-card
            label="Avg Error Rate"
            [value]="(metrics.averageErrorRate | number:'1.1-2') + '%'"
            icon="percent"
            [accentColor]="metrics.averageErrorRate > 5 ? '#ef4444' : '#6b7280'" />
        </div>

        <!-- Status Breakdown + Recent Alerts -->
        <div class="widgets-row">
          <mat-card class="widget-card">
            <mat-card-header>
              <mat-card-title>
                <mat-icon>donut_large</mat-icon> Status Breakdown
              </mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <div class="status-bars">
                @for (s of metrics.fluxStatusBreakdown; track s.status) {
                  <div class="status-row">
                    <div class="status-label">
                      <span class="dot" [style.background]="s.color"></span>
                      {{ s.status }}
                    </div>
                    <div class="status-bar-track">
                      <div class="status-bar-fill"
                        [style.width.%]="(s.count / metrics!.totalFluxes) * 100"
                        [style.background]="s.color">
                      </div>
                    </div>
                    <span class="status-count">{{ s.count }}</span>
                  </div>
                }
              </div>
            </mat-card-content>
          </mat-card>

          <mat-card class="widget-card">
            <mat-card-header>
              <mat-card-title>
                <mat-icon color="warn">notifications</mat-icon> Recent Alerts
              </mat-card-title>
            </mat-card-header>
            <mat-card-content>
              @if (metrics.recentAlerts.length === 0) {
                <p class="no-alerts">No active alerts — all systems nominal.</p>
              }
              @for (alert of metrics.recentAlerts; track alert.alertId) {
                <div class="alert-row" [class]="'severity-' + alert.severity.toLowerCase()">
                  <div class="alert-info">
                    <span class="alert-flux">{{ alert.fluxName }}</span>
                    <span class="alert-msg">{{ alert.message }}</span>
                  </div>
                  <span class="alert-badge" [class]="'badge-' + alert.severity.toLowerCase()">
                    {{ alert.severity }}
                  </span>
                </div>
              }
            </mat-card-content>
          </mat-card>
        </div>

        <!-- Flux Table -->
        <mat-card class="flux-table-card">
          <mat-card-header>
            <mat-card-title>
              <mat-icon>storage</mat-icon> Active Flux Operations
            </mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <table class="flux-table">
              <thead>
                <tr>
                  <th>Name</th><th>Type</th><th>Status</th>
                  <th>Throughput/h</th><th>Error Rate</th><th>Last Run</th>
                </tr>
              </thead>
              <tbody>
                @for (flux of fluxes; track flux.id) {
                  <tr>
                    <td class="flux-name">{{ flux.name }}</td>
                    <td><span class="type-chip">{{ flux.type }}</span></td>
                    <td><fob-status-badge [status]="flux.status" /></td>
                    <td>{{ flux.throughputPerHour | number }}</td>
                    <td [class.error-rate]="(flux.errorRatePercent ?? 0) > 5">
                      {{ flux.errorRatePercent | number:'1.1-2' }}%
                    </td>
                    <td>{{ flux.lastExecutedAt | date:'short' }}</td>
                  </tr>
                }
              </tbody>
            </table>
          </mat-card-content>
        </mat-card>
      }

      @if (loading && !metrics) {
        <div class="loading-container">
          <mat-spinner diameter="48"></mat-spinner>
          <p>Loading dashboard data...</p>
        </div>
      }
    </div>
  `,
  styles: [`
    .dashboard-container { padding: 24px; max-width: 1600px; margin: 0 auto; }
    .dashboard-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 24px; }
    .dashboard-title { display: flex; align-items: center; gap: 8px; font-size: 28px; font-weight: 700; margin: 0; }
    .dashboard-subtitle { color: #6b7280; margin: 4px 0 0; }
    .header-actions { display: flex; align-items: center; gap: 12px; }
    .refresh-label { color: #6b7280; font-size: 13px; display: flex; align-items: center; gap: 4px; }
    .spin { animation: spin 1s linear infinite; }
    @keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }

    .kpi-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 16px; margin-bottom: 24px; }

    .widgets-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 24px; }
    .widget-card mat-card-title { display: flex; align-items: center; gap: 8px; font-size: 16px; }

    .status-bars { display: flex; flex-direction: column; gap: 12px; padding-top: 8px; }
    .status-row { display: flex; align-items: center; gap: 12px; }
    .status-label { display: flex; align-items: center; gap: 6px; width: 90px; font-size: 13px; font-weight: 500; }
    .dot { width: 10px; height: 10px; border-radius: 50%; flex-shrink: 0; }
    .status-bar-track { flex: 1; height: 8px; background: #f1f5f9; border-radius: 4px; overflow: hidden; }
    .status-bar-fill { height: 100%; border-radius: 4px; transition: width 0.5s ease; }
    .status-count { width: 24px; text-align: right; font-size: 13px; font-weight: 600; }

    .alert-row { display: flex; align-items: center; justify-content: space-between;
      padding: 10px 12px; border-radius: 8px; margin-bottom: 8px; background: #f8fafc; }
    .alert-row.severity-critical { background: #fef2f2; }
    .alert-row.severity-warning { background: #fffbeb; }
    .alert-info { display: flex; flex-direction: column; gap: 2px; flex: 1; }
    .alert-flux { font-weight: 600; font-size: 13px; }
    .alert-msg { font-size: 12px; color: #6b7280; }
    .alert-badge { font-size: 11px; font-weight: 700; padding: 2px 8px; border-radius: 4px; }
    .badge-critical { background: #fee2e2; color: #991b1b; }
    .badge-warning { background: #fef3c7; color: #92400e; }
    .badge-info { background: #dbeafe; color: #1e40af; }
    .no-alerts { color: #22c55e; font-weight: 600; text-align: center; padding: 16px; }

    .flux-table-card { margin-bottom: 24px; }
    .flux-table { width: 100%; border-collapse: collapse; font-size: 14px; }
    .flux-table th { text-align: left; padding: 12px 16px; background: #f8fafc; font-weight: 600; color: #374151; border-bottom: 2px solid #e5e7eb; }
    .flux-table td { padding: 12px 16px; border-bottom: 1px solid #f1f5f9; }
    .flux-table tr:hover td { background: #f8fafc; }
    .flux-name { font-weight: 600; }
    .type-chip { background: #e0e7ff; color: #3730a3; padding: 2px 8px; border-radius: 4px; font-size: 12px; font-weight: 600; }
    .error-rate { color: #ef4444; font-weight: 700; }

    .loading-container { display: flex; flex-direction: column; align-items: center; gap: 16px; padding: 60px; }
  `]
})
export class DashboardComponent implements OnInit, OnDestroy {
  private readonly fluxService = inject(FluxService);

  metrics: DashboardMetrics | null = null;
  fluxes: Flux[] = [];
  loading = false;
  readonly refreshInterval = environment.refreshIntervalMs;

  private subscription?: Subscription;

  ngOnInit(): void {
    this.subscription = interval(this.refreshInterval).pipe(
      startWith(0),
      switchMap(() => {
        this.loading = true;
        return this.fluxService.getDashboardMetrics().pipe(catchError(() => of(null)));
      })
    ).subscribe(data => {
      if (data) this.metrics = data;
      this.loading = false;
    });

    this.fluxService.getFluxes({ pageSize: 50, status: 'Active' }).subscribe(p => {
      this.fluxes = p.items;
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
