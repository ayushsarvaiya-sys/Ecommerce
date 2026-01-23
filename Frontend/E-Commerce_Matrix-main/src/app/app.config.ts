// import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
// import { provideRouter } from '@angular/router';
// import { provideHttpClient, withInterceptors, HTTP_INTERCEPTORS } from '@angular/common/http';
// import { HttpErrorInterceptor } from './interceptors/http.interceptor';

// import { routes } from './app.routes';

// export const appConfig: ApplicationConfig = {
//   providers: [
//     provideBrowserGlobalErrorListeners(),
//     provideRouter(routes),
//     provideHttpClient(),
//     { provide: HTTP_INTERCEPTORS, useClass: HttpErrorInterceptor, multi: true }
//   ]
// };


import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import {
  provideHttpClient,
  withInterceptorsFromDi,
  HTTP_INTERCEPTORS
} from '@angular/common/http';

import { HttpErrorInterceptor } from './interceptors/http.interceptor';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),

    // DI-based interceptor
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpErrorInterceptor,
      multi: true
    }
  ]
};
