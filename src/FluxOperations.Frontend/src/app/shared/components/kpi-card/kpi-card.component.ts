import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'fob-kpi-card',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule],
  template: `
    <mat-card class="kpi-card" [style.border-left-color]="accentColor">
      <mat-card-content>
        <div class="kpi-header">
          <mat-icon class="kpi-icon" [style.color]="accentColor">{{ icon }}</mat-icon>
          <span class="kpi-label">{{ label }}</span>
        </div>
        <div class="kpi-value" [style.color]="accentColor">{{ value | number }}</div>
        @if (subtitle) {
          <div class="kpi-subtitle">{{ subtitle }}</div>
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .kpi-card {
      border-left: 4px solid;
      min-height: 120px;
      transition: transform 0.2s, box-shadow 0.2s;
      &:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0,0,0,0.15); }
    }
    .kpi-header { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; }
    .kpi-icon { font-size: 20px; width: 20px; height: 20px; }
    .kpi-label { font-size: 13px; color: #666; font-weight: 500; text-transform: uppercase; letter-spacing: 0.5px; }
    .kpi-value { font-size: 36px; font-weight: 700; line-height: 1.2; }
    .kpi-subtitle { font-size: 12px; color: #999; margin-top: 4px; }
  `]
})
export class KpiCardComponent {
  @Input() label = '';
  @Input() value: number | string = 0;
  @Input() icon = 'analytics';
  @Input() accentColor = '#3b82f6';
  @Input() subtitle?: string;
}
