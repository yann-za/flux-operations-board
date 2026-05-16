import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'fob-root',
  standalone: true,
  imports: [
    RouterModule, CommonModule,
    MatToolbarModule, MatIconModule, MatButtonModule,
    MatSidenavModule, MatListModule
  ],
  template: `
    <mat-sidenav-container class="sidenav-container">
      <mat-sidenav mode="side" opened class="sidenav">
        <div class="sidenav-header">
          <mat-icon class="logo-icon">bolt</mat-icon>
          <span class="logo-text">FluxOps Board</span>
        </div>
        <mat-nav-list>
          <a mat-list-item routerLink="/dashboard" routerLinkActive="active-link">
            <mat-icon matListItemIcon>dashboard</mat-icon>
            <span matListItemTitle>Dashboard</span>
          </a>
          <a mat-list-item routerLink="/flux" routerLinkActive="active-link">
            <mat-icon matListItemIcon>account_tree</mat-icon>
            <span matListItemTitle>Flux Operations</span>
          </a>
          <a mat-list-item routerLink="/alerts" routerLinkActive="active-link">
            <mat-icon matListItemIcon>notifications</mat-icon>
            <span matListItemTitle>Alerts</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <mat-toolbar class="app-toolbar">
          <span class="toolbar-title">Flux Operations Board</span>
          <span class="spacer"></span>
          <span class="env-badge">v1.0.0</span>
        </mat-toolbar>
        <main>
          <router-outlet />
        </main>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [`
    .sidenav-container { height: 100vh; }
    .sidenav { width: 240px; background: #1e293b; color: white; }
    .sidenav-header { display: flex; align-items: center; gap: 10px; padding: 20px 16px; border-bottom: 1px solid rgba(255,255,255,0.1); }
    .logo-icon { color: #60a5fa; font-size: 28px; width: 28px; height: 28px; }
    .logo-text { font-size: 18px; font-weight: 700; color: white; }
    .sidenav mat-nav-list { padding-top: 8px; }
    .active-link { background: rgba(96,165,250,0.15) !important; color: #60a5fa !important; }
    .active-link mat-icon { color: #60a5fa !important; }
    .app-toolbar { background: white; border-bottom: 1px solid #e5e7eb; }
    .toolbar-title { font-weight: 600; color: #1e293b; }
    .spacer { flex: 1; }
    .env-badge { background: #e0e7ff; color: #3730a3; padding: 2px 10px; border-radius: 9999px; font-size: 12px; font-weight: 600; }
    main { background: #f1f5f9; min-height: calc(100vh - 64px); }
  `]
})
export class AppComponent {}
