import { Component, OnInit, OnDestroy, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService, Toast } from '../../services/notification.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-toast-notification',
  standalone: true,
  imports: [CommonModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="toast-container">
      <div *ngFor="let toast of toasts" 
           [ngClass]="'toast toast-' + toast.type">
        <div class="toast-content">
          <span class="toast-icon">
            <span *ngIf="toast.type === 'success'" class="icon-success">✓</span>
            <span *ngIf="toast.type === 'error'" class="icon-error">✕</span>
            <span *ngIf="toast.type === 'warning'" class="icon-warning">⚠</span>
            <span *ngIf="toast.type === 'info'" class="icon-info">ℹ</span>
          </span>
          <span class="toast-message">{{ toast.message }}</span>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      top: 20px;
      right: 20px;
      z-index: 9999;
      pointer-events: none;
    }

    .toast {
      min-width: 300px;
      max-width: 500px;
      margin-bottom: 10px;
      padding: 12px 16px;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
      display: flex;
      align-items: center;
      gap: 12px;
      pointer-events: auto;
      animation: slideInRight 0.3s ease-out;
    }

    @keyframes slideInRight {
      from {
        transform: translateX(400px);
        opacity: 0;
      }
      to {
        transform: translateX(0);
        opacity: 1;
      }
    }

    .toast-success {
      background-color: #d4edda;
      border: 1px solid #c3e6cb;
      color: #155724;
    }

    .toast-error {
      background-color: #f8d7da;
      border: 1px solid #f5c6cb;
      color: #721c24;
    }

    .toast-warning {
      background-color: #fff3cd;
      border: 1px solid #ffeaa7;
      color: #856404;
    }

    .toast-info {
      background-color: #d1ecf1;
      border: 1px solid #bee5eb;
      color: #0c5460;
    }

    .toast-content {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .toast-icon {
      font-weight: bold;
      font-size: 18px;
      flex-shrink: 0;
    }

    .toast-message {
      word-break: break-word;
      font-size: 14px;
      line-height: 1.4;
    }

    @media (max-width: 600px) {
      .toast {
        min-width: 280px;
        max-width: 90vw;
      }

      .toast-container {
        top: 10px;
        right: 10px;
        left: 10px;
      }
    }
  `]
})
export class ToastNotificationComponent implements OnInit, OnDestroy {
  toasts: Toast[] = [];
  private subscription: Subscription | null = null;

  constructor(
    private notificationService: NotificationService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    console.log('Toast component initialized');
    this.subscription = this.notificationService.toasts$.subscribe((toasts: Toast[]) => {
      console.log('Toasts updated:', toasts);
      this.toasts = toasts;
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
