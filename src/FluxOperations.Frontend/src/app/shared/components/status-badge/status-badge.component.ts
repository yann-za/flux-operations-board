import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FluxStatus } from '../../../core/models/flux.model';

const STATUS_CONFIG: Record<FluxStatus, { color: string; bg: string; icon: string; label: string }> = {
  Active:    { color: '#166534', bg: '#dcfce7', icon: '●', label: 'Active' },
  Warning:   { color: '#92400e', bg: '#fef3c7', icon: '▲', label: 'Warning' },
  Error:     { color: '#991b1b', bg: '#fee2e2', icon: '✕', label: 'Error' },
  Paused:    { color: '#374151', bg: '#f3f4f6', icon: '⏸', label: 'Paused' },
  Inactive:  { color: '#6b7280', bg: '#f9fafb', icon: '○', label: 'Inactive' },
  Completed: { color: '#1e40af', bg: '#dbeafe', icon: '✓', label: 'Completed' },
};

@Component({
  selector: 'fob-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="badge" [style.color]="config.color" [style.background-color]="config.bg">
      <span class="icon">{{ config.icon }}</span>
      {{ config.label }}
    </span>
  `,
  styles: [`
    .badge {
      display: inline-flex; align-items: center; gap: 4px;
      padding: 2px 10px; border-radius: 9999px;
      font-size: 12px; font-weight: 600; white-space: nowrap;
    }
    .icon { font-size: 10px; }
  `]
})
export class StatusBadgeComponent {
  @Input() set status(s: FluxStatus) { this.config = STATUS_CONFIG[s] ?? STATUS_CONFIG['Inactive']; }
  config = STATUS_CONFIG['Inactive'];
}
