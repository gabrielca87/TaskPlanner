import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthSession } from '../services/auth-session';

export const guestGuard: CanActivateFn = () => {
  const authSession = inject(AuthSession);
  const router = inject(Router);

  if (authSession.isAuthenticated()) {
    return router.createUrlTree(['/task-items']);
  }

  return true;
};
