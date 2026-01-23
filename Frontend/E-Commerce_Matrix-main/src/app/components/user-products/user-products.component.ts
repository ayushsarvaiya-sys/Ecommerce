import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import {
  ProductService,
  ProductResponse,
  PaginatedResponse,
} from '../../services/product.service';

@Component({
  selector: 'app-user-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
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

//   constructor(private productService: ProductService) {}
constructor(
  private productService: ProductService,
  private cdr: ChangeDetectorRef
) {}


  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(page: number = 1): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.currentPage = page;

    this.productService
      .getProductsPaginated(page, this.pageSize, this.searchTerm)
      .subscribe({
        next: (response) => {
          console.log('API Response:', response);
          console.log('Response Data:', response?.data);
          if (response && response.data) {
            const paginatedData = response.data as PaginatedResponse<ProductResponse>;
            console.log('Paginated Data:', paginatedData);
            console.log('Items:', paginatedData.data);
            this.products = Array.isArray(paginatedData.data) ? paginatedData.data : [];
            this.totalCount = paginatedData.totalCount || 0;
            // Calculate totalPages from totalCount and pageSize
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
}
