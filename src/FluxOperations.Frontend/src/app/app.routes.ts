import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadComponent: () =>
      import('./features/dashboard/components/dashboard.component')
        .then(m => m.DashboardComponent),
    title: 'Dashboard — Flux Operations Board'
  },
  {
    path: 'flux',
    loadComponent: () =>
      import('./features/flux/components/flux-list.component')
        .then(m => m.FluxListComponent),
    title: 'Flux Operations — Flux Operations Board'
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
