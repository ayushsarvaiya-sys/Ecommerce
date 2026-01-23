import { Injectable } from '@angular/core';
import { signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';

export interface User {
  id: number;
  email: string;
  fullName: string;
  role: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegistrationRequest {
  email: string;
  fullName: string;
  password: string;
  role: string;
}

export interface AuthResponse {
  statusCode: number;
  data: User;
  message: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSignal = signal<User | null>(this.loadUserFromStorage());
  private apiUrl = 'https://localhost:7067/api/Auth';
  
  currentUser = computed(() => this.currentUserSignal());
  isLoggedIn = computed(() => this.currentUserSignal() !== null);
  userRole = computed(() => this.currentUserSignal()?.role ?? null);

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<AuthResponse> {
    const loginRequest: LoginRequest = { email, password };
    return this.http.post<AuthResponse>(`${this.apiUrl}/Login`, loginRequest, {
      // withCredentials: true    //Implemented in interceptor
    }).pipe(
      tap((response) => {
        if (response && response.data) {
          this.currentUserSignal.set(response.data);
          this.saveUserToStorage(response.data);
        }
      }),
      catchError((error) => {
        console.error('Login error:', error);
        return throwError(() => error);
      })
    );
  }

  register(
    email: string,
    fullName: string,
    password: string,
    role: string
  ): Observable<AuthResponse> {
    const registrationRequest: RegistrationRequest = {
      email,
      fullName,
      password,
      role,
    };
    return this.http.post<AuthResponse>(`${this.apiUrl}/Registor`, registrationRequest, {
      // withCredentials: true    //Implemented in interceptor
    }).pipe(
      tap((response) => {
        if (response && response.data) {
          this.currentUserSignal.set(response.data);
          this.saveUserToStorage(response.data);
        }
      }),
      catchError((error) => {
        console.error('Registration error:', error);
        return throwError(() => error);
      })
    );
  }

  logout(): Observable<any> {
    return this.http.post(`${this.apiUrl}/Logout`, {}, {
      // withCredentials: true    //Implemented in interceptor
    }).pipe(
      tap(() => {
        this.currentUserSignal.set(null);
        localStorage.removeItem('currentUser');
      }),
      catchError((error) => {
        console.error('Logout error:', error);
        this.currentUserSignal.set(null);
        localStorage.removeItem('currentUser');
        return throwError(() => error);
      })
    );
  }

  private saveUserToStorage(user: User): void {
    localStorage.setItem('currentUser', JSON.stringify(user));
  }

  private loadUserFromStorage(): User | null {
    const stored = localStorage.getItem('currentUser');
    return stored ? JSON.parse(stored) : null;
  }
}
