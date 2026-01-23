import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Clone request and add credentials for cookie support
    console.log('Interceptor running'); 
    request = request.clone({
      withCredentials: true
    });
    
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Interceptor caught error:', error);
        return throwError(() => error);
      })
    );
  }
}


