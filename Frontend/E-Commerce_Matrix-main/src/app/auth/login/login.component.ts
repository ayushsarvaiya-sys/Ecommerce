import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  private authService = inject(AuthService);
  private cartService = inject(CartService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  loginForm: FormGroup;
  submitted = false;
  loginError = '';
  showPassword = false;
  isLoading = false;

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)
]],
    });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    this.submitted = true;
    this.loginError = '';

    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    const { email, password } = this.loginForm.value;

    this.authService.login(email, password).subscribe({
      next: (response) => {
        this.isLoading = false;
        
        // Load cart data for the logged-in user
        this.cartService.loadCart();
        
        this.router.navigate(['/home']);   
      },
      error: (error: any) => {
        this.isLoading = false;
        // Extract message from backend error response
        if (error?.error?.message) {
          this.loginError = error.error.message;
        } else if (error?.message) {
          this.loginError = error.message;
        } else {
          this.loginError = 'An error occurred during login. Please try again.';
        }
        console.error('Login failed:', error);
      }
    });
  }

  getAdminDemoCredentials(): void {
    this.loginForm.reset({
      email: 'admin@ecommerce.com',
      password: 'Admin@123',
    });
  }

  getUserDemoCredentials(): void {
    this.loginForm.reset({
      email: 'user@ecommerce.com',
      password: 'User@123',
    });
  }
}