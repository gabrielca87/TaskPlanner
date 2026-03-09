import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthSession } from '../services/auth-session';

export const authGuard: CanActivateFn = () => {
  const authSession = inject(AuthSession);
  const router = inject(Router);

  if (authSession.isAuthenticated()) {
    return true;
  }

  return router.createUrlTree(['/login']);
};
