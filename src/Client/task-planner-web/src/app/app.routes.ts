import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { TaskItemList } from './features/task-items/task-item-list/task-item-list';
import { Register } from './features/auth/register/register';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'task-items' },
  {
    path: 'task-items',
    component: TaskItemList,
    title: 'Task Items'
  },
  {
    path: 'login',
    component: Login,
    title: 'Login'
  },
  {
    path: 'register',
    component: Register,
    title: 'Register'
  },
  { path: '**', redirectTo: 'task-items' }
];
