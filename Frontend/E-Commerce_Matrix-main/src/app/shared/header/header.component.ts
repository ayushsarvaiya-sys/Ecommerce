import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  // 🔥 Use computed signal for reactive updates
  currentUser = this.authService.currentUser;
  isLoggedIn = this.authService.isLoggedIn;

  logout() {
    // ✅ Subscribe to logout API call
    this.authService.logout().subscribe({
      next: () => {
        console.log('Logout successful');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('Logout failed:', err);
        // Still navigate even if API fails
        this.router.navigate(['/login']);
      }
    });
  }
}
