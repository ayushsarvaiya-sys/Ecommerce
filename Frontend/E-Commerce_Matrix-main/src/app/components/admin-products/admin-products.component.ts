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
  Category,
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

  // Categories
  categories: Category[] = [];
  categoriesLoading = false;

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
    imageUrl: '',
  };

  editProduct: UpdateProductRequest = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    categoryId: 1,
    imageUrl: '',
  };

  restockData: RestockProductRequest = {
    productId: 0,
    quantityToAdd: 0,
  };

  deleteProductId = 0;

  // Image upload properties
  selectedImageFile: File | null = null;
  selectedImageForEdit: File | null = null;
  imagePreviewUrl: string | ArrayBuffer | null = null;
  imagePreviewUrlEdit: string | ArrayBuffer | null = null;
  isUploadingImage = false;
  uploadedImageUrl = '';
  uploadedImageUrlEdit = '';

  constructor(
    private productService: ProductService,
    private cdr: ChangeDetectorRef
  ) {}

  // Image Upload Methods
  onImageSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      this.selectedImageFile = file;
      
      // Preview image
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreviewUrl = e.target.result;
        this.cdr.detectChanges();
      };
      reader.readAsDataURL(file);
    }
  }

  onImageSelectedEdit(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      this.selectedImageForEdit = file;
      
      // Preview image
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreviewUrlEdit = e.target.result;
        this.cdr.detectChanges();
      };
      reader.readAsDataURL(file);
    }
  }

  uploadImageAndAddProduct(): void {
    if (!this.selectedImageFile) {
      this.errorMessage = 'Please select an image';
      return;
    }

    this.uploadedImageUrl = '';
    this.isUploadingImage = true;
    this.errorMessage = '';

    // Step 1: Get Cloudinary configuration from backend
    this.productService.getCloudinaryConfig().subscribe({
      next: (response) => {
        if (response?.data?.cloudName && response?.data?.uploadPreset) {
          const cloudName = response.data.cloudName;
          const uploadPreset = response.data.uploadPreset;

          // Step 2: Upload image directly to Cloudinary
          this.productService.uploadImageToCloudinary(this.selectedImageFile!, cloudName, uploadPreset).subscribe({
            next: (cloudinaryResponse) => {
              console.log('Cloudinary response:', cloudinaryResponse);
              
              // Get the secure URL from Cloudinary response
              const imageUrl = cloudinaryResponse.secure_url || cloudinaryResponse.url;
              
              if (imageUrl) {
                this.uploadedImageUrl = imageUrl;
                this.newProduct.imageUrl = imageUrl;
                this.successMessage = 'Image uploaded successfully!';
                this.isUploadingImage = false;
                this.cdr.detectChanges();
                
                // Auto-submit the product form
                // setTimeout(() => {
                //   this.addProduct();
                // }, 500);
              } else {
                this.errorMessage = 'Failed to get image URL from Cloudinary';
                this.isUploadingImage = false;
                this.cdr.detectChanges();
              }
            },
            error: (error) => {
              console.error('Cloudinary upload error:', error);
              this.errorMessage = 'Failed to upload image: ' + (error.error?.message || error.message || 'Unknown error');
              this.isUploadingImage = false;
              this.cdr.detectChanges();
            }
          });
        } else {
          this.errorMessage = 'Failed to get Cloudinary configuration';
          this.isUploadingImage = false;
          this.cdr.detectChanges();
        }
      },
      error: (error) => {
        console.error('Cloudinary config error:', error);
        this.errorMessage = 'Failed to get Cloudinary config: ' + (error.error?.message || error.message || 'Unknown error');
        this.isUploadingImage = false;
        this.cdr.detectChanges();
      }
    });
  }

  uploadImageAndUpdateProduct(): void {
    if (!this.selectedImageForEdit) {
      this.errorMessage = 'Please select an image';
      return;
    }

    this.uploadedImageUrlEdit = '';
    this.isUploadingImage = true;
    this.errorMessage = '';

    // Step 1: Get Cloudinary configuration from backend
    this.productService.getCloudinaryConfig().subscribe({
      next: (response) => {
        if (response?.data?.cloudName && response?.data?.uploadPreset) {
          const cloudName = response.data.cloudName;
          const uploadPreset = response.data.uploadPreset;

          // Step 2: Upload image directly to Cloudinary
          this.productService.uploadImageToCloudinary(this.selectedImageForEdit!, cloudName, uploadPreset).subscribe({
            next: (cloudinaryResponse) => {
              console.log('Cloudinary response:', cloudinaryResponse);
              
              // Get the secure URL from Cloudinary response
              const imageUrl = cloudinaryResponse.secure_url || cloudinaryResponse.url;
              
              if (imageUrl) {
                this.uploadedImageUrlEdit = imageUrl;
                this.editProduct.imageUrl = imageUrl;
                this.successMessage = 'Image uploaded successfully!';
                this.isUploadingImage = false;
                this.cdr.detectChanges();
                
                // Auto-submit the product form
                setTimeout(() => {
                  this.updateProduct();
                }, 500);
              } else {
                this.errorMessage = 'Failed to get image URL from Cloudinary';
                this.isUploadingImage = false;
                this.cdr.detectChanges();
              }
            },
            error: (error) => {
              console.error('Cloudinary upload error:', error);
              this.errorMessage = 'Failed to upload image: ' + (error.error?.message || error.message || 'Unknown error');
              this.isUploadingImage = false;
              this.cdr.detectChanges();
            }
          });
        } else {
          this.errorMessage = 'Failed to get Cloudinary configuration';
          this.isUploadingImage = false;
          this.cdr.detectChanges();
        }
      },
      error: (error) => {
        console.error('Cloudinary config error:', error);
        this.errorMessage = 'Failed to get Cloudinary config: ' + (error.error?.message || error.message || 'Unknown error');
        this.isUploadingImage = false;
        this.cdr.detectChanges();
      }
    });
  }

  clearImageSelection(): void {
    this.selectedImageFile = null;
    this.imagePreviewUrl = null;
    this.uploadedImageUrl = '';
    this.cdr.detectChanges();
  }

  clearImageSelectionEdit(): void {
    this.selectedImageForEdit = null;
    this.imagePreviewUrlEdit = null;
    this.uploadedImageUrlEdit = '';
    this.cdr.detectChanges();
  }

  ngOnInit(): void {
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
