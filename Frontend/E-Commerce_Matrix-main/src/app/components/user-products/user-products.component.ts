import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import { NgxSliderModule, Options, LabelType } from '@angular-slider/ngx-slider';
import {
  ProductService,
  ProductResponse,
  PaginatedResponse,
} from '../../services/product.service';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-user-products',
  standalone: true,
  imports: [CommonModule, FormsModule, NgxSliderModule],
  templateUrl: './user-products.component.html',
  styleUrls: ['./user-products.component.css'],
})
export class UserProductsComponent implements OnInit {
  products: ProductResponse[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  // Pagination properties
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  totalCount = 0;

  // Search
  searchTerm = '';

  // Filters
  selectedCategoryId: number | null = null;
  minPrice = 0;
  maxPrice = 20000;
  categories: any[] = [];
  categoriesLoading = false;

  // Admin-only filters
  minQuantity = 0;
  maxQuantity = 1000;
  sortByPrice: string = ''; // '' | 'asc' | 'desc'
  sortByQuantity: string = ''; // '' | 'asc' | 'desc'
  includeDeleted = false;

  // Slider options
  priceSliderOptions: Options = {
    floor: 0,
    ceil: 20000,
    step: 10,
    translate: (value: number, label: LabelType): string => {
      return '\u20b9' + value.toLocaleString('en-IN');
    },
  };

  quantitySliderOptions: Options = {
    floor: 0,
    ceil: 1000,
    step: 1,
  };

  // User role
  userRole: string | null = null;

  // Add to Cart properties
  cartQuantities: { [productId: number]: number } = {};
  addingToCart: { [productId: number]: boolean } = {};
  cartAddedSuccess: { [productId: number]: boolean } = {};

//   constructor(private productService: ProductService) {}
constructor(
  private productService: ProductService,
  private cdr: ChangeDetectorRef,
  private authService: AuthService,
  private cartService: CartService
) {}


  ngOnInit(): void {
    this.userRole = this.authService.userRole();
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories(): void {
    this.categoriesLoading = true;
    this.productService.getAllCategories().subscribe({
      next: (response) => {
        if (response?.data) {
          this.categories = response.data;
        }
        this.categoriesLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        this.categoriesLoading = false;
      }
    });
  }

  loadProducts(page: number = 1): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.currentPage = page;

    // For admin users, use the admin endpoint with all filters
    if (this.userRole === 'Admin') {
      this.productService
        .getProductsAdminPaginated(
          page,
          this.pageSize,
          this.searchTerm || undefined,
          this.selectedCategoryId || undefined,
          this.minPrice !== undefined ? this.minPrice : undefined,
          this.maxPrice !== undefined ? this.maxPrice : undefined,
          this.minQuantity !== undefined && this.minQuantity > 0 ? this.minQuantity : undefined,
          this.maxQuantity !== undefined && this.maxQuantity < 10000 ? this.maxQuantity : undefined,
          this.sortByPrice || undefined,
          this.sortByQuantity || undefined,
          this.includeDeleted
        )
        .subscribe({
          next: (response) => {
            console.log('API Response:', response);
            console.log('Response Data:', response?.data);
            if (response && response.data) {
              const paginatedData = response.data as PaginatedResponse<ProductResponse>;
              console.log('Paginated Data:', paginatedData);
              console.log('Items:', paginatedData.data);
              this.products = Array.isArray(paginatedData.data) ? paginatedData.data : [];
              
              // Initialize quantity for each product to 1
              this.products.forEach(product => {
                if (!this.cartQuantities[product.id]) {
                  this.cartQuantities[product.id] = 1;
                }
              });
              
              this.totalCount = paginatedData.totalCount || 0;
              this.totalPages = Math.ceil(this.totalCount / this.pageSize);
              console.log('Products set to:', this.products);
              console.log('Products length:', this.products.length);
            } else {
              console.log('No data in response');
              this.products = [];
              this.totalCount = 0;
              this.totalPages = 0;
            }
            this.isLoading = false;
            console.log('isLoading set to false');
            this.cdr.detectChanges();
          },
          error: (error) => {
            this.errorMessage = 'Failed to load products. Please try again.';
            this.products = [];
            this.totalCount = 0;
            this.totalPages = 0;
            console.error('Error loading products:', error);
            this.isLoading = false;
          },
        });
    } else {
      // For regular users, use the regular endpoint with basic filters
      this.productService
        .getProductsPaginated(
          page, 
          this.pageSize, 
          this.searchTerm || undefined,
          this.selectedCategoryId || undefined,
          this.minPrice || undefined,
          this.maxPrice || undefined
        )
        .subscribe({
          next: (response) => {
            console.log('API Response:', response);
            console.log('Response Data:', response?.data);
            if (response && response.data) {
              const paginatedData = response.data as PaginatedResponse<ProductResponse>;
              console.log('Paginated Data:', paginatedData);
              console.log('Items:', paginatedData.data);
              this.products = Array.isArray(paginatedData.data) ? paginatedData.data : [];
              
              // Initialize quantity for each product to 1
              this.products.forEach(product => {
                if (!this.cartQuantities[product.id]) {
                  this.cartQuantities[product.id] = 1;
                }
              });
              
              this.totalCount = paginatedData.totalCount || 0;
              this.totalPages = Math.ceil(this.totalCount / this.pageSize);
              console.log('Products set to:', this.products);
              console.log('Products length:', this.products.length);
            } else {
              console.log('No data in response');
              this.products = [];
              this.totalCount = 0;
              this.totalPages = 0;
            }
            this.isLoading = false;
            console.log('isLoading set to false');
            this.cdr.detectChanges();
          },
          error: (error) => {
            this.errorMessage = 'Failed to load products. Please try again.';
            this.products = [];
            this.totalCount = 0;
            this.totalPages = 0;
            console.error('Error loading products:', error);
            this.isLoading = false;
          },
        });
    }
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadProducts(1);
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.onSearch();
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.loadProducts(this.currentPage - 1);
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.loadProducts(this.currentPage + 1);
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.loadProducts(page);
    }
  }

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

  applyFilters(): void {
    this.currentPage = 1;
    this.loadProducts(1);
  }

  resetFilters(): void {
    this.searchTerm = '';
    this.selectedCategoryId = null;
    this.minPrice = 0;
    this.maxPrice = 1000000;
    this.minQuantity = 0;
    this.maxQuantity = 10000;
    this.sortByPrice = '';
    this.sortByQuantity = '';
    this.includeDeleted = false;
    this.currentPage = 1;
    this.loadProducts(1);
    this.cdr.detectChanges();
  }

  /**
   * Initialize cart quantity for a product (default 1)
   */
  initializeCartQuantity(productId: number): number {
    if (!this.cartQuantities[productId]) {
      this.cartQuantities[productId] = 1;
    }
    return this.cartQuantities[productId];
  }

  /**
   * Increment cart quantity for a product
   */
  incrementQuantity(productId: number): void {
    if (!this.cartQuantities[productId]) {
      this.cartQuantities[productId] = 1;
    }
    this.cartQuantities[productId]++;
  }

  /**
   * Decrement cart quantity for a product
   */
  decrementQuantity(productId: number): void {
    if (this.cartQuantities[productId] && this.cartQuantities[productId] > 1) {
      this.cartQuantities[productId]--;
    }
  }

  /**
   * Get available stock - handles both user view (stockStatus) and admin view (stock)
   * For users: stockStatus can be numeric string, "In Stock", or "Out of Stock"
   * For admins: stock is numeric value
   */
  getAvailableStock(product: any): number {
    // Admin view: uses numeric 'stock' field
    if (this.userRole === 'Admin' && product.stock !== undefined) {
      return product.stock;
    }
    
    // User view: uses 'stockStatus' field
    if (!product.stockStatus) return 0;
    
    // If stockStatus is a number (string representation of number)
    const stockAsNumber = parseInt(product.stockStatus, 10);
    if (!isNaN(stockAsNumber)) {
      return stockAsNumber;
    }
    
    // If stockStatus is "In Stock"
    if (product.stockStatus === 'In Stock') {
      return 999999; // Large number to indicate stock available
    }
    
    // If stockStatus is "Out of Stock" or anything else
    return 0;
  }

  /**
   * Get stock status text for display
   * For admins: shows numeric stock value
   * For users: shows formatted stock status
   */
  getStockStatusText(product: any): string {
    const stock = this.getAvailableStock(product);
    
    // Admin view: always show numeric stock
    if (this.userRole === 'Admin') {
      return `Stock: ${stock}`;
    }
    
    // User view: show formatted status
    if (stock === 0) return 'Out of Stock';
    if (stock >= 999999) return 'In Stock';
    return `Stock: ${stock}`;
  }

  /**
   * Check if product is in stock and available for purchase
   * Admins cannot purchase, so always return false for admins
   */
  isProductAvailable(product: any): boolean {
    if (this.userRole === 'Admin') {
      return false; // Admins cannot purchase
    }
    return this.getAvailableStock(product) > 0;
  }

  /**
   * Add product to cart
   */
  addToCart(product: ProductResponse): void {
    const quantity = this.cartQuantities[product.id] || 1;
    const availableStock = this.getAvailableStock(product);

    if (quantity <= 0) {
      this.errorMessage = 'Quantity must be at least 1';
      return;
    }

    if (availableStock === 0) {
      this.errorMessage = 'This product is out of stock';
      return;
    }

    if (availableStock < 999999 && quantity > availableStock) {
      this.errorMessage = `Only ${availableStock} items available`;
      return;
    }

    this.addingToCart[product.id] = true;
    this.errorMessage = '';

    this.cartService.addToCart(product.id, quantity).subscribe({
      next: (response) => {
        this.successMessage = `${product.name} added to cart!`;
        this.cartAddedSuccess[product.id] = true;
        this.addingToCart[product.id] = false;
        this.cartQuantities[product.id] = 1; // Reset quantity
        this.cdr.detectChanges();

        // Hide success message after 3 seconds
        setTimeout(() => {
          this.cartAddedSuccess[product.id] = false;
          this.successMessage = '';
          this.cdr.detectChanges();
        }, 3000);
      },
      error: (error) => {
        this.errorMessage = error?.error?.message || 'Failed to add to cart';
        this.addingToCart[product.id] = false;
        this.cdr.detectChanges();
      }
    });
  }
}
