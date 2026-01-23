import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css'],
})
export class RegistrationComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  registrationForm: FormGroup;
  submitted = false;
  registerError = '';
  showPassword = false;
  showConfirmPassword = false;
  isLoading = false;

  constructor() {
    this.registrationForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/)]],
      confirmPassword: ['', Validators.required],
      role: ['User', Validators.required],
    }, {
      validators: this.passwordMatchValidator
    });
  }

  get name() {
    return this.registrationForm.get('name');
  }

  get email() {
    return this.registrationForm.get('email');
  }

  get password() {
    return this.registrationForm.get('password');
  }

  get confirmPassword() {
    return this.registrationForm.get('confirmPassword');
  }

  get role() {
    return this.registrationForm.get('role');
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  onSubmit(): void {
    this.submitted = true;
    this.registerError = '';

    if (this.registrationForm.invalid) {
      return;
    }

    this.isLoading = true;
    const { email, name, password, role } = this.registrationForm.value;

    this.authService.register(email, name, password, role).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.router.navigate(['/login']);
      },
      error: (error: any) => {
        this.isLoading = false;
        // Extract message from backend error response
        if (error?.error?.message) {
          this.registerError = error.error.message;
        } else if (error?.message) {
          this.registerError = error.message;
        } else {
          this.registerError = 'An error occurred during registration. Please try again.';
        }
        console.error('Registration failed:', error);
      }
    });
  }
}
