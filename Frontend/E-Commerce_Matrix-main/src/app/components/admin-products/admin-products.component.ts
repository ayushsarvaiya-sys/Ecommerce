import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import {
  ProductService,
  AdminProductResponse,
  PaginatedResponse,
  CreateProductRequest,
  UpdateProductRequest,
  RestockProductRequest,
} from '../../services/product.service';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-products.component.html',
  styleUrls: ['./admin-products.component.css'],
})
export class AdminProductsComponent implements OnInit {
  products: AdminProductResponse[] = [];
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

  // Modal states
  showAddProductModal = false;
  showEditProductModal = false;
  showRestockModal = false;
  showDeleteConfirm = false;

  // Form data
  newProduct: CreateProductRequest = {
    name: '',
    description: '',
    price: 0,
    stock: 0,
    categoryId: 1,
  };

  editProduct: UpdateProductRequest = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    categoryId: 1,
  };

  restockData: RestockProductRequest = {
    productId: 0,
    quantityToAdd: 0,
  };

  deleteProductId = 0;

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
      .getProductsAdminPaginated(page, this.pageSize, this.searchTerm)
      .subscribe({
        next: (response) => {
          console.log('API Response:', response);
          console.log('Response Data:', response?.data);
          if (response && response.data) {
            const paginatedData = response.data as PaginatedResponse<AdminProductResponse>;
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

  // Modal operations
  openAddProductModal(): void {
    this.newProduct = {
      name: '',
      description: '',
      price: 0,
      stock: 0,
      categoryId: 1,
    };
    this.showAddProductModal = true;
    this.cdr.detectChanges();
  }

  closeAddProductModal(): void {
    this.showAddProductModal = false;
    this.cdr.detectChanges();
  }

  addProduct(): void {
    if (
      !this.newProduct.name ||
      !this.newProduct.description ||
      this.newProduct.price <= 0 ||
      this.newProduct.stock < 0
    ) {
      this.errorMessage = 'Please fill in all required fields correctly';
      return;
    }

    this.productService.addProduct(this.newProduct).subscribe({
      next: (response) => {
        this.successMessage = 'Product added successfully!';
        this.closeAddProductModal();
        this.loadProducts(1);
        this.cdr.detectChanges();
        setTimeout(() => (this.successMessage = ''), 3000);
      },
      error: (error) => {
        this.errorMessage = 'Failed to add product. ' + (error.error?.message || '');
        console.error('Error adding product:', error);
        this.cdr.detectChanges();
      },
    });
  }

  openEditProductModal(product: AdminProductResponse): void {
    this.editProduct = {
      id: product.id,
      name: product.name,
      description: product.description,
      price: product.price,
      categoryId: product.categoryId,
    };
    this.showEditProductModal = true;
    this.cdr.detectChanges();
  }

  closeEditProductModal(): void {
    this.showEditProductModal = false;
    this.cdr.detectChanges();
  }

  updateProduct(): void {
    if (
      !this.editProduct.name ||
      !this.editProduct.description ||
      this.editProduct.price <= 0
    ) {
      this.errorMessage = 'Please fill in all required fields correctly';
      return;
    }

    this.productService.updateProduct(this.editProduct).subscribe({
      next: (response) => {
        this.successMessage = 'Product updated successfully!';
        this.closeEditProductModal();
        this.loadProducts(this.currentPage);
        this.cdr.detectChanges();
        setTimeout(() => (this.successMessage = ''), 3000);
      },
      error: (error) => {
        this.errorMessage = 'Failed to update product. ' + (error.error?.message || '');
        console.error('Error updating product:', error);
        this.cdr.detectChanges();
      },
    });
  }

  openRestockModal(product: AdminProductResponse): void {
    this.restockData = {
      productId: product.id,
      quantityToAdd: 0,
    };
    this.showRestockModal = true;
    this.cdr.detectChanges();
  }

  closeRestockModal(): void {
    this.showRestockModal = false;
    this.cdr.detectChanges();
  }

  restockProduct(): void {
    if (this.restockData.quantityToAdd <= 0) {
      this.errorMessage = 'Please enter a valid quantity';
      return;
    }

    // this.restockData.productId = this.restockData.productId;

    this.productService.restockProduct(this.restockData).subscribe({
      next: (response) => {
        this.successMessage = 'Product restocked successfully!';
        this.closeRestockModal();
        this.loadProducts(this.currentPage);
        this.cdr.detectChanges();
        setTimeout(() => (this.successMessage = ''), 3000);
      },
      error: (error) => {
        this.errorMessage = 'Failed to restock product. ' + (error.error?.message || '');
        console.error('Error restocking product:', error);
        this.cdr.detectChanges();
      },
    });
  }

  openDeleteConfirm(productId: number): void {
    this.deleteProductId = productId;
    this.showDeleteConfirm = true;
    this.cdr.detectChanges();
  }

  closeDeleteConfirm(): void {
    this.showDeleteConfirm = false;
    this.cdr.detectChanges();
  }

  confirmDelete(): void {
    this.productService.deleteProduct(this.deleteProductId).subscribe({
      next: (response) => {
        this.successMessage = 'Product deleted successfully!';
        this.closeDeleteConfirm();
        this.loadProducts(this.currentPage > 1 ? this.currentPage : 1);
        this.cdr.detectChanges();
        setTimeout(() => (this.successMessage = ''), 3000);
      },
      error: (error) => {
        this.errorMessage = 'Failed to delete product. ' + (error.error?.message || '');
        console.error('Error deleting product:', error);
        this.cdr.detectChanges();
      },
    });
  }
}
