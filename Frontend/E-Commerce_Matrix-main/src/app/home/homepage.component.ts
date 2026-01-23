import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-homepage',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.css'],
})
export class HomepageComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  currentUser = computed(() => this.authService.currentUser());
  userRole = computed(() => this.authService.userRole());

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
