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
import { NotificationService } from '../services/notification.service';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private notificationService: NotificationService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Clone request and add credentials for cookie support
    console.log('Interceptor running'); 
    
    // Don't add credentials to Cloudinary requests
    if (!request.url.includes('cloudinary.com')) {
      request = request.clone({
        withCredentials: true
      });
    }
    
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        console.error('Interceptor caught error:', error);
        
        // Handle Rate Limiting (429 Too Many Requests)
        if (error.status === 429) {
          const errorData = error.error;
          const message = errorData?.message || 'Too many requests. Please wait before making another request.';
          const retryAfter = errorData?.retryAfter || 60;
          
          // Show error notification with retry information (auto-dismiss after 5 seconds)
          this.notificationService.error(
            `${message} (Retry after ${Math.ceil(retryAfter)} seconds)`,
            5000 // Auto-dismiss after 5 seconds
          );
        }
        // Handle other HTTP errors
        else if (error.status >= 400) {
          const errorMessage = error.error?.message || `Error: ${error.statusText}`;
          this.notificationService.error(errorMessage); // Uses default 5 seconds
        }
        
        return throwError(() => error);
      })
    );
  }
}


