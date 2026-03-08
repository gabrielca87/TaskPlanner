import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'task-items' },
  {
    path: 'task-items',
    loadComponent: () =>
      import('./features/task-items/task-item-list/task-item-list').then((m) => m.TaskItemList)
  },
  {
    path: 'login',
    component: Login,
    title: 'Login'
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register').then((m) => m.Register)
  },
  { path: '**', redirectTo: 'task-items' }
];
