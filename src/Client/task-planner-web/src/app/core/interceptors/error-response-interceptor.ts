import { HttpInterceptorFn, HttpStatusCode } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthSession } from '../services/auth-session';

const authEndpointPrefix = `${environment.apiBaseUrl}/auth`;

export const errorResponseInterceptor: HttpInterceptorFn = (request, next) => {
  const authSession = inject(AuthSession);
  const router = inject(Router);

  return next(request).pipe(
    catchError((error) => {
      const isAuthEndpoint = request.url.startsWith(authEndpointPrefix);

      if (error.status === HttpStatusCode.Unauthorized && !isAuthEndpoint) {
        authSession.clear();
        void router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );
};
