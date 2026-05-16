import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';

import { FluxService } from '../../../core/services/flux.service';
import { Flux, FluxStatus, FluxType } from '../../../core/models/flux.model';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';

@Component({
  selector: 'fob-flux-list',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatCardModule, MatButtonModule, MatIconModule,
    MatInputModule, MatSelectModule, MatFormFieldModule, MatMenuModule,
    MatSnackBarModule, StatusBadgeComponent
  ],
  template: `
    <div class="flux-list-container">
      <div class="page-header">
        <h2><mat-icon>account_tree</mat-icon> Flux Operations</h2>
        <button mat-raised-button color="primary">
          <mat-icon>add</mat-icon> New Flux
        </button>
      </div>

      <!-- Filters -->
      <mat-card class="filters-card">
        <mat-card-content>
          <div class="filters-row">
            <mat-form-field appearance="outline">
              <mat-label>Search</mat-label>
              <input matInput [(ngModel)]="searchTerm" (ngModelChange)="onFilterChange()" placeholder="Name, source, target...">
              <mat-icon matSuffix>search</mat-icon>
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Status</mat-label>
              <mat-select [(ngModel)]="statusFilter" (ngModelChange)="onFilterChange()">
                <mat-option [value]="null">All statuses</mat-option>
                @for (s of statuses; track s) {
                  <mat-option [value]="s">{{ s }}</mat-option>
                }
              </mat-select>
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Type</mat-label>
              <mat-select [(ngModel)]="typeFilter" (ngModelChange)="onFilterChange()">
                <mat-option [value]="null">All types</mat-option>
                @for (t of types; track t) {
                  <mat-option [value]="t">{{ t }}</mat-option>
                }
              </mat-select>
            </mat-form-field>
            <span class="results-count">{{ totalCount }} flux(es)</span>
          </div>
        </mat-card-content>
      </mat-card>

      <!-- Table -->
      <mat-card>
        <mat-card-content>
          <table class="flux-table">
            <thead>
              <tr>
                <th>Name</th><th>Type</th><th>Status</th>
                <th>Source → Target</th><th>Throughput/h</th>
                <th>Error Rate</th><th>Schedule</th><th>Actions</th>
              </tr>
            </thead>
            <tbody>
              @for (flux of fluxes; track flux.id) {
                <tr>
                  <td class="flux-name">{{ flux.name }}</td>
                  <td><span class="type-chip">{{ flux.type }}</span></td>
                  <td><fob-status-badge [status]="flux.status" /></td>
                  <td class="route">
                    @if (flux.sourceSystem) { <span>{{ flux.sourceSystem }}</span> }
                    @if (flux.sourceSystem && flux.targetSystem) { <mat-icon class="arrow">arrow_forward</mat-icon> }
                    @if (flux.targetSystem) { <span>{{ flux.targetSystem }}</span> }
                  </td>
                  <td>{{ flux.throughputPerHour | number }}</td>
                  <td [class.error]="(flux.errorRatePercent ?? 0) > 5">
                    {{ (flux.errorRatePercent | number:'1.1-2') ?? '—' }}%
                  </td>
                  <td class="cron">{{ flux.scheduleCron ?? 'Continuous' }}</td>
                  <td>
                    <button mat-icon-button [matMenuTriggerFor]="menu">
                      <mat-icon>more_vert</mat-icon>
                    </button>
                    <mat-menu #menu="matMenu">
                      @if (flux.status === 'Active') {
                        <button mat-menu-item (click)="pause(flux)">
                          <mat-icon>pause</mat-icon> Pause
                        </button>
                      }
                      @if (flux.status === 'Paused') {
                        <button mat-menu-item (click)="resume(flux)">
                          <mat-icon>play_arrow</mat-icon> Resume
                        </button>
                      }
                      <button mat-menu-item (click)="archive(flux)">
                        <mat-icon>archive</mat-icon> Archive
                      </button>
                    </mat-menu>
                  </td>
                </tr>
              }
              @if (fluxes.length === 0 && !loading) {
                <tr><td colspan="8" class="empty">No flux operations found.</td></tr>
              }
            </tbody>
          </table>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .flux-list-container { padding: 24px; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    .page-header h2 { display: flex; align-items: center; gap: 8px; margin: 0; }
    .filters-card { margin-bottom: 16px; }
    .filters-row { display: flex; gap: 16px; align-items: center; flex-wrap: wrap; }
    .filters-row mat-form-field { flex: 1; min-width: 160px; }
    .results-count { color: #6b7280; font-size: 13px; white-space: nowrap; }
    .flux-table { width: 100%; border-collapse: collapse; }
    .flux-table th { text-align: left; padding: 12px; background: #f8fafc; font-weight: 600; border-bottom: 2px solid #e5e7eb; }
    .flux-table td { padding: 12px; border-bottom: 1px solid #f1f5f9; }
    .flux-table tr:hover td { background: #f8fafc; }
    .flux-name { font-weight: 600; }
    .type-chip { background: #e0e7ff; color: #3730a3; padding: 2px 8px; border-radius: 4px; font-size: 12px; font-weight: 600; }
    .route { display: flex; align-items: center; gap: 4px; font-size: 13px; }
    .arrow { font-size: 16px; width: 16px; height: 16px; color: #9ca3af; }
    .cron { font-family: monospace; font-size: 12px; }
    .error { color: #ef4444; font-weight: 700; }
    .empty { text-align: center; color: #9ca3af; padding: 40px; }
  `]
})
export class FluxListComponent implements OnInit {
  private readonly fluxService = inject(FluxService);
  private readonly snackBar = inject(MatSnackBar);

  fluxes: Flux[] = [];
  totalCount = 0;
  loading = false;
  searchTerm = '';
  statusFilter: FluxStatus | null = null;
  typeFilter: FluxType | null = null;

  readonly statuses: FluxStatus[] = ['Active', 'Inactive', 'Warning', 'Error', 'Paused', 'Completed'];
  readonly types: FluxType[] = ['DataPipeline', 'ETL', 'ApiIntegration', 'BatchProcessing', 'Streaming', 'FileTransfer', 'MessageQueue'];

  ngOnInit(): void { this.loadFluxes(); }

  onFilterChange(): void { this.loadFluxes(); }

  loadFluxes(): void {
    this.loading = true;
    this.fluxService.getFluxes({
      search: this.searchTerm || undefined,
      status: this.statusFilter ?? undefined,
      type: this.typeFilter ?? undefined,
      pageSize: 100
    }).subscribe({
      next: p => { this.fluxes = p.items; this.totalCount = p.totalCount; this.loading = false; },
      error: () => this.loading = false
    });
  }

  pause(flux: Flux): void {
    this.fluxService.pauseFlux(flux.id).subscribe(() => {
      this.snackBar.open(`"${flux.name}" paused`, 'OK', { duration: 3000 });
      this.loadFluxes();
    });
  }

  resume(flux: Flux): void {
    this.fluxService.resumeFlux(flux.id).subscribe(() => {
      this.snackBar.open(`"${flux.name}" resumed`, 'OK', { duration: 3000 });
      this.loadFluxes();
    });
  }

  archive(flux: Flux): void {
    this.fluxService.deleteFlux(flux.id).subscribe(() => {
      this.snackBar.open(`"${flux.name}" archived`, 'OK', { duration: 3000 });
      this.loadFluxes();
    });
  }
}
