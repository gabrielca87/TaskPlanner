import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { authTokenInterceptor } from './core/interceptors/auth-token-interceptor';
import { errorResponseInterceptor } from './core/interceptors/error-response-interceptor';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([authTokenInterceptor, errorResponseInterceptor])),
    provideRouter(routes)
  ]
};
