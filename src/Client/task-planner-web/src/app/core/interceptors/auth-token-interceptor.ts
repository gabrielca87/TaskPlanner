import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { AuthSession } from '../services/auth-session';

export const authTokenInterceptor: HttpInterceptorFn = (request, next) => {
  const authSession = inject(AuthSession);
  const accessToken = authSession.getAccessToken();
  const isApiRequest = request.url.startsWith(environment.apiBaseUrl);

  if (!accessToken || !isApiRequest) {
    return next(request);
  }

  const authorizedRequest = request.clone({
    setHeaders: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  return next(authorizedRequest);
};
