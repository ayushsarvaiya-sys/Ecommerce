# API Integration Implementation Guide

## Overview
The Frontend and Backend API integration has been successfully implemented. The Angular frontend now makes HTTP requests to the C# .NET backend for authentication operations (login and registration).

## Changes Made

### 1. Frontend - Auth Service Updates
**File:** [src/app/services/auth.service.ts](Frontend/E-Commerce_Matrix-main/src/app/services/auth.service.ts)

#### Key Changes:
- Replaced mock authentication with actual HTTP calls
- Added `HttpClient` dependency injection
- Created TypeScript interfaces for API requests/responses:
  - `LoginRequest` - Email and password
  - `RegistrationRequest` - Email, fullName, password, and role
  - `AuthResponse` - API response structure with statusCode, data (User), and message
  
#### Methods:
- **`login(email: string, password: string)`** - Returns `Observable<AuthResponse>`
  - Sends POST request to `http://127.0.0.1:5000/api/auth/Login`
  - Automatically stores user in localStorage on success
  - Returns Observable for component subscription
  
- **`register(email: string, fullName: string, password: string, role: string)`** - Returns `Observable<AuthResponse>`
  - Sends POST request to `http://127.0.0.1:5000/api/auth/Registor`
  - Automatically stores user in localStorage on success
  - Returns Observable for component subscription

### 2. App Configuration
**File:** [src/app/app.config.ts](Frontend/E-Commerce_Matrix-main/src/app/app.config.ts)

#### Changes:
- Added `provideHttpClient()` to the application configuration
- This enables HttpClient functionality across the application

### 3. Login Component Updates
**File:** [src/app/auth/login/login.component.ts](Frontend/E-Commerce_Matrix-main/src/app/auth/login/login.component.ts)

#### Key Changes:
- Added `isLoading` property to track request status
- Updated `onSubmit()` method to:
  - Subscribe to the Observable returned by `authService.login()`
  - Handle success (navigate to home) and error (display error message) cases
  - Disable form during submission
  
#### HTML Updates:
- Button now shows "Logging in..." during loading
- Button is disabled during submission and when form is invalid

### 4. Registration Component Updates
**File:** [src/app/auth/registration/registration.component.ts](Frontend/E-Commerce_Matrix-main/src/app/auth/registration/registration.component.ts)

#### Key Changes:
- Added `isLoading` property to track request status
- Updated role default value from 'user' to 'User' (matches backend enum)
- Updated `onSubmit()` method to:
  - Subscribe to the Observable returned by `authService.register()`
  - Handle success (navigate to home) and error (display error message) cases
  - Disable form during submission
  - Only pass 4 parameters to register (removed confirmPassword)

#### HTML Updates:
- Button now shows "Registering..." during loading
- Button is disabled during submission and when form is invalid
- Updated role options: "Regular User" (User) and "Admin"

## Backend Endpoints

### Login Endpoint
- **URL:** `POST /api/auth/Login`
- **Request Body:**
```json
{
  "email": "string",
  "password": "string"
}
```
- **Response:** `ApiResponse<AuthResponseDTO>`

### Registration Endpoint
- **URL:** `POST /api/auth/Registor`
- **Request Body:**
```json
{
  "email": "string",
  "fullName": "string",
  "password": "string",
  "role": "User|Admin"
}
```
- **Response:** `ApiResponse<AuthResponseDTO>`

## CORS Configuration
The backend is configured to accept requests from `http://127.0.0.1:4200` (Angular development server).

**Important:** Ensure the backend is running on `http://127.0.0.1:5000`

## Demo Credentials

### Admin Account
- **Email:** admin@ecommerce.com
- **Password:** Admin@123

### User Account
- **Email:** user@ecommerce.com
- **Password:** User@123

## Running the Application

### Backend (C# .NET)
```bash
cd Backend/ECommerce/ECommerce
dotnet run
# Backend will run on http://127.0.0.1:5000
```

### Frontend (Angular)
```bash
cd Frontend/E-Commerce_Matrix-main
npm install
ng serve
# Frontend will run on http://127.0.0.1:4200
```

## Data Flow

1. **User submits login/registration form**
   - Component validates form
   - Sets `isLoading = true`
   - Calls `authService.login()` or `authService.register()`

2. **AuthService makes HTTP request**
   - Sends POST request to backend endpoint
   - Backend validates and processes request
   - Returns `ApiResponse` with user data or error

3. **Component handles response**
   - On success: Stores user in signal/localStorage, navigates to home
   - On error: Displays error message from backend
   - Sets `isLoading = false`

## Error Handling

- HTTP errors are caught and displayed to the user
- Error messages from backend are extracted and shown in the UI
- Frontend validation prevents invalid data from being sent to backend
- Backend also validates all incoming requests

## Type Safety

- All API requests and responses are strongly typed
- TypeScript interfaces ensure data consistency
- Angular's HttpClient provides compile-time type checking

## Next Steps

1. Test login and registration flows end-to-end
2. Verify localStorage persistence
3. Add JWT token authentication if needed
4. Implement route guards to protect pages
5. Add loading states/spinners to improve UX
6. Implement error toast notifications
