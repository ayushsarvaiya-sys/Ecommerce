import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { ChangeDetectorRef } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CartService, Cart, CartItem } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.css'],
})
export class ShoppingCartComponent implements OnInit, OnDestroy {
  cart: Cart | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  isUpdating = false;

  // Pagination for cart items
  currentPage = 1;
  itemsPerPage = 5;
  displayedItems: CartItem[] = [];
  totalPages = 1;

  private destroy$ = new Subject<void>();

  constructor(
    private cartService: CartService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCart();
    // Subscribe to cart updates
    this.cartService.cart$
      .pipe(takeUntil(this.destroy$))
      .subscribe((cart) => {
        this.cart = cart;
        this.updatePaginatedItems();
        this.cdr.detectChanges();
      });

    // Subscribe to loading state
    this.cartService.loading$
      .pipe(takeUntil(this.destroy$))
      .subscribe((loading) => {
        this.isLoading = loading;
        this.cdr.detectChanges();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Load cart from service
   */
  loadCart(): void {
    this.cartService.loadCart();
  }

  /**
   * Update paginated items based on current page
   */
  private updatePaginatedItems(): void {
    if (this.cart && this.cart.cartItems.length > 0) {
      this.totalPages = Math.ceil(this.cart.cartItems.length / this.itemsPerPage);
      const startIdx = (this.currentPage - 1) * this.itemsPerPage;
      const endIdx = startIdx + this.itemsPerPage;
      this.displayedItems = this.cart.cartItems.slice(startIdx, endIdx);
    } else {
      this.displayedItems = [];
      this.totalPages = 1;
    }
  }

  /**
   * Go to previous page
   */
  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePaginatedItems();
    }
  }

  /**
   * Go to next page
   */
  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updatePaginatedItems();
    }
  }

  /**
   * Go to specific page
   */
  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updatePaginatedItems();
    }
  }

  /**
   * Get page numbers for pagination display
   */
  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPages = 5;
    let start = Math.max(1, this.currentPage - Math.floor(maxPages / 2));
    let end = Math.min(this.totalPages, start + maxPages - 1);

    if (end - start + 1 < maxPages) {
      start = Math.max(1, end - maxPages + 1);
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }

  /**
   * Update cart item quantity
   */
  updateQuantity(cartItem: CartItem, newQuantity: number): void {
    if (newQuantity < 1) {
      this.errorMessage = 'Quantity must be at least 1';
      return;
    }

    this.isUpdating = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.cartService.updateCartItem(cartItem.id, newQuantity).subscribe({
      next: (response) => {
        this.successMessage = 'Cart updated successfully';
        this.isUpdating = false;
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
          this.cdr.detectChanges();
        }, 3000);
      },
      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to update cart item';
        this.isUpdating = false;
        this.cdr.detectChanges();
      }
    });
  }

  /**
   * Remove item from cart
   */
  removeItem(cartItemId: number, productName?: string): void {
    if (!confirm(`Are you sure you want to remove this item from your cart?`)) {
      return;
    }

    this.isUpdating = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.cartService.removeCartItem(cartItemId).subscribe({
      next: (response) => {
        this.successMessage = 'Item removed from cart';
        this.isUpdating = false;
        this.currentPage = 1; // Reset to first page
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
          this.cdr.detectChanges();
        }, 3000);
      },
      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to remove item from cart';
        this.isUpdating = false;
        this.cdr.detectChanges();
      }
    });
  }

  /**
   * Clear entire cart
   */
  clearCart(): void {
    if (!confirm('Are you sure you want to clear your entire cart?')) {
      return;
    }

    this.isUpdating = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.cartService.clearCart().subscribe({
      next: (response) => {
        this.successMessage = 'Cart cleared successfully';
        this.isUpdating = false;
        this.currentPage = 1; // Reset to first page
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
          this.cdr.detectChanges();
        }, 3000);
      },
      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to clear cart';
        this.isUpdating = false;
        this.cdr.detectChanges();
      }
    });
  }

  /**
   * Proceed to checkout
   */
  proceedToCheckout(): void {
    if (!this.cart || this.cart.cartItems.length === 0) {
      this.errorMessage = 'Your cart is empty';
      return;
    }

    // Navigate to checkout page (you can create this component later)
    this.router.navigate(['/checkout']);
  }

  /**
   * Continue shopping
   */
  continueShopping(): void {
    this.router.navigate(['/products']);
  }

  /**
   * Check if user is logged in
   */
  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  /**
   * Get total savings (if applicable)
   */
  getTotalSavings(): number {
    if (!this.cart) return 0;
    // You can implement actual savings calculation here
    return 0;
  }
}
