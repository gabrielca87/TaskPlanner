import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { guestGuard } from './core/guards/guest-guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'task-items' },
  {
    path: 'task-items',
    loadComponent: () => import('./features/task-items/task-item-list/task-item-list').then(m => m.TaskItemList),
    canActivate: [authGuard],
    title: 'Task Items'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login').then(m => m.Login),
    canActivate: [guestGuard],
    title: 'Login'
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/register/register').then(m => m.Register),
    canActivate: [guestGuard],
    title: 'Register'
  },
  { path: '**', redirectTo: 'task-items' }
];
