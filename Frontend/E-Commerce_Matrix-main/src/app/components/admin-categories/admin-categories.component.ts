import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import { ProductService, Category } from '../../services/product.service';

export interface CategoryRequest {
  name: string;
  description?: string;
}

export interface UpdateCategoryRequest {
  categoryId: number;
  newName: string;
  description?: string;
}

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: '../admin-categories/admin-categories.component.html',
  styleUrls: ['../admin-categories/admin-categories.component.css'],
})
export class AdminCategoriesComponent implements OnInit {
  private productService = inject(ProductService);
  private cdr = inject(ChangeDetectorRef);

  categories: Category[] = [];
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

  // Filter for deleted categories
  includeDeleted = false;

  // Modal states
  showAddCategoryModal = false;
  showEditCategoryModal = false;
  showDeleteConfirm = false;

  // Form data
  newCategory: CategoryRequest = {
    name: '',
    description: '',
  };

  editCategory: UpdateCategoryRequest = {
    categoryId: 0,
    newName: '',
  };

  editCategoryDescription = '';
  deleteCategoryId = 0;
  deleteCategoryName = '';

  constructor() {}

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(page: number = 1): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.currentPage = page;

    this.productService.getAllCategoriesAdmin(this.includeDeleted).subscribe({
      next: (response) => {
        if (response?.data) {
          const categories = Array.isArray(response.data) ? response.data : [];
          console.log('Fetched categories:', categories);
          
          this.categories = categories;
          this.totalCount = this.categories.length;
          // Calculate totalPages from totalCount and pageSize
          this.totalPages = Math.ceil(this.totalCount / this.pageSize);
        } else {
          this.categories = [];
          this.totalCount = 0;
          this.totalPages = 0;
        }
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.errorMessage = 'Failed to load categories. Please try again.';
        this.categories = [];
        this.totalCount = 0;
        this.totalPages = 0;
        console.error('Error loading categories:', error);
        this.isLoading = false;
      },
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadCategories(1);
  }

  clearSearch(): void {
    this.searchTerm = '';
    this.onSearch();
  }

  applyFilters(): void {
    this.currentPage = 1;
    this.loadCategories(1);
  }

  resetFilters(): void {
    this.searchTerm = '';
    this.includeDeleted = false;
    this.currentPage = 1;
    this.loadCategories(1);
    this.cdr.detectChanges();
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.loadCategories(this.currentPage - 1);
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.loadCategories(this.currentPage + 1);
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.loadCategories(page);
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

  getFilteredCategories(): Category[] {
    if (!this.searchTerm.trim()) {
      return this.categories;
    }
    return this.categories.filter(
      (cat) =>
        cat.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        (cat.description && cat.description.toLowerCase().includes(this.searchTerm.toLowerCase()))
    );
  }

  // Modal operations
  openAddCategoryModal(): void {
    this.newCategory = {
      name: '',
      description: '',
    };
    this.errorMessage = '';
    this.showAddCategoryModal = true;
    this.cdr.detectChanges();
  }

  closeAddCategoryModal(): void {
    this.showAddCategoryModal = false;
    this.cdr.detectChanges();
  }

  addCategory(): void {
    if (!this.newCategory.name || this.newCategory.name.trim() === '') {
      this.errorMessage = 'Category name is required';
      return;
    }

    this.productService.addCategory(this.newCategory).subscribe({
      next: (response) => {
        this.successMessage = 'Category added successfully!';
        this.closeAddCategoryModal();
        this.loadCategories(1);
        this.cdr.detectChanges();
        setTimeout(() => (this.successMessage = ''), 3000);
      },
      error: (error) => {
        this.errorMessage = 'Failed to add category. ' + (error.error?.message || '');
        console.error('Error adding category:', error);
        this.cdr.detectChanges();
      },
    });
  }

  openEditCategoryModal(category: Category): void {
    this.editCategory = {
      categoryId: category.id,
      newName: category.name,
    };
    this.editCategoryDescription = category.description || '';
    this.errorMessage = '';
    this.showEditCategoryModal = true;
    this.cdr.detectChanges();
  }

  closeEditCategoryModal(): void {
    this.showEditCategoryModal = false;
    this.cdr.detectChanges();
  }

  updateCategory(): void {
    if (!this.editCategory.newName || this.editCategory.newName.trim() === '') {
      this.errorMessage = 'Category name is required';
      return;
    }

    // Include description in the update request
    this.editCategory.description = this.editCategoryDescription;

    this.productService.updateCategoryName(this.editCategory).subscribe({
      next: (response) => {
        this.successMessage = 'Category updated successfully!';
        this.closeEditCategoryModal();
        this.loadCategories(this.currentPage);
        this.cdr.detectChanges();
        setTimeout(() => (this.successMessage = ''), 3000);
      },
      error: (error) => {
        this.errorMessage = 'Failed to update category. ' + (error.error?.message || '');
        console.error('Error updating category:', error);
        this.cdr.detectChanges();
      },
    });
  }

  openDeleteConfirm(category: Category): void {
    this.deleteCategoryId = category.id;
    this.deleteCategoryName = category.name;
    this.errorMessage = '';
    this.showDeleteConfirm = true;
    this.cdr.detectChanges();
  }

  closeDeleteConfirm(): void {
    this.showDeleteConfirm = false;
    this.cdr.detectChanges();
  }

  confirmDelete(): void {
    this.productService.deleteCategory(this.deleteCategoryId).subscribe({
      next: (response) => {
        this.successMessage = 'Category deleted successfully!';
        this.closeDeleteConfirm();
        this.loadCategories(this.currentPage > 1 ? this.currentPage : 1);
        this.cdr.detectChanges();
        setTimeout(() => (this.successMessage = ''), 3000);
      },
      error: (error) => {
        this.errorMessage = 'Failed to delete category. ' + (error.error?.message || '');
        console.error('Error deleting category:', error);
        this.cdr.detectChanges();
      },
    });
  }

  restoreCategory(category: Category): void {
    this.productService.restoreCategory(category.id).subscribe({
      next: (response) => {
        this.successMessage = 'Category restored successfully!';
        this.loadCategories(this.currentPage);
        this.cdr.detectChanges();
        setTimeout(() => (this.successMessage = ''), 3000);
      },
      error: (error) => {
        this.errorMessage = 'Failed to restore category. ' + (error.error?.message || '');
        console.error('Error restoring category:', error);
        this.cdr.detectChanges();
      },
    });
  }
}
