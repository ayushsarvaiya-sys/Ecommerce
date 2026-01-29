import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
  category?: {
    id: number;
    name: string;
  };
}

export interface ProductResponse {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
  categoryName?: string;
  imageUrl?: string;
}

export interface AdminProductResponse extends ProductResponse {
  createdDate?: string;
  updatedDate?: string;
  isDeleted?: boolean;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
  imageUrl?: string;
}

export interface UpdateProductRequest {
  id: number;
  name: string;
  description: string;
  price: number;
  categoryId: number;
  imageUrl?: string;
}

export interface RestockProductRequest {
  productId: number;
  quantityToAdd: number;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  offset: number;
  limit: number;
  currentPageCount: number;
  hasMore: boolean;
}

export interface ApiResponse<T> {
  statusCode: number;
  data: T;
  message: string;
}

export interface PresignedUrlResponse {
  presignedUrl: string;
  publicId: string;
  timestamp: number;
}

export interface CloudinaryConfig {
  cloudName: string;
  uploadPreset: string;
}

export interface ImageUploadRequest {
  imageName: string;
  resourceType?: string;
}

export interface Category {
  id: number;
  name: string;
  description?: string;
  isDeleted?: boolean;
  createdDate?: string;
  updatedDate?: string;
}

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = 'https://localhost:7067/api/Product';
  private cloudinaryUrl = 'https://localhost:7067/api/Cloudinary';

  constructor(private http: HttpClient) {}

  // Cloudinary Image Upload Methods
  getCloudinaryConfig(): Observable<ApiResponse<CloudinaryConfig>> {
    return this.http.get<ApiResponse<CloudinaryConfig>>(
      `${this.cloudinaryUrl}/GetUploadConfig`
    );
  }

  uploadImageToCloudinary(file: File, cloudName: string, uploadPreset: string): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('upload_preset', uploadPreset);
    
    const uploadUrl = `https://api.cloudinary.com/v1_1/${cloudName}/image/upload`;
    
    return this.http.post<any>(uploadUrl, formData);
  }

  // User endpoints
  getProductsPaginated(
    page: number = 1,
    pageSize: number = 10,
    search?: string,
    categoryId?: number,
    minPrice?: number,
    maxPrice?: number
  ): Observable<ApiResponse<PaginatedResponse<ProductResponse>>> {
    let url = `${this.apiUrl}/GetPaginated?page=${page}&pageSize=${pageSize}`;
    if (search) {
      url += `&search=${encodeURIComponent(search)}`;
    }
    if (categoryId && categoryId > 0) {
      url += `&categoryId=${categoryId}`;
    }
    if (minPrice !== undefined && minPrice >= 0) {
      url += `&minPrice=${minPrice}`;
    }
    if (maxPrice !== undefined && maxPrice >= 0) {
      url += `&maxPrice=${maxPrice}`;
    }
    return this.http.get<ApiResponse<PaginatedResponse<ProductResponse>>>(url);
  }

  getProductById(id: number): Observable<ApiResponse<ProductResponse>> {
    return this.http.get<ApiResponse<ProductResponse>>(
      `${this.apiUrl}/GetById/${id}`
    );
  }

  getProductsByCategory(
    categoryId: number
  ): Observable<ApiResponse<ProductResponse[]>> {
    return this.http.get<ApiResponse<ProductResponse[]>>(
      `${this.apiUrl}/GetByCategory/${categoryId}`
    );
  }

  // Admin endpoints
  getProductsAdminPaginated(
    page: number = 1,
    pageSize: number = 10,
    search?: string,
    categoryId?: number,
    minPrice?: number,
    maxPrice?: number,
    minQuantity?: number,
    maxQuantity?: number,
    sortByPrice?: string,
    sortByQuantity?: string,
    includeDeleted: boolean = false
  ): Observable<ApiResponse<PaginatedResponse<AdminProductResponse>>> {
    let url = `${this.apiUrl}/GetPaginatedAdmin?page=${page}&pageSize=${pageSize}`;
    if (search) {
      url += `&search=${encodeURIComponent(search)}`;
    }
    if (categoryId && categoryId > 0) {
      url += `&categoryId=${categoryId}`;
    }
    if (minPrice !== undefined && minPrice >= 0) {
      url += `&minPrice=${minPrice}`;
    }
    if (maxPrice !== undefined && maxPrice >= 0) {
      url += `&maxPrice=${maxPrice}`;
    }
    if (minQuantity !== undefined && minQuantity >= 0) {
      url += `&minQuantity=${minQuantity}`;
    }
    if (maxQuantity !== undefined && maxQuantity >= 0) {
      url += `&maxQuantity=${maxQuantity}`;
    }
    if (sortByPrice) {
      url += `&sortByPrice=${encodeURIComponent(sortByPrice)}`;
    }
    if (sortByQuantity) {
      url += `&sortByQuantity=${encodeURIComponent(sortByQuantity)}`;
    }
    // Always include includeDeleted parameter (both true and false)
    url += `&includeDeleted=${includeDeleted}`;
    return this.http.get<ApiResponse<PaginatedResponse<AdminProductResponse>>>(url);
  }

  addProduct(
    product: CreateProductRequest
  ): Observable<ApiResponse<ProductResponse>> {
    return this.http.post<ApiResponse<ProductResponse>>(
      `${this.apiUrl}/Add`,
      product
    );
  }

  updateProduct(
    product: UpdateProductRequest
  ): Observable<ApiResponse<ProductResponse>> {
    return this.http.put<ApiResponse<ProductResponse>>(
      `${this.apiUrl}/Update`,
      product
    );
  }

  deleteProduct(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(
      `${this.apiUrl}/Delete/${id}`
    );
  }

  restoreProduct(id: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(
      `${this.apiUrl}/Restore/${id}`,
      {}
    );
  }

  restockProduct(
    request: RestockProductRequest
  ): Observable<ApiResponse<ProductResponse>> {
    console.log('Restock request:', request);
    return this.http.put<ApiResponse<ProductResponse>>(
      `${this.apiUrl}/Restock`,
      request
    );
  }

  // Category endpoints
  private categoryUrl = 'https://localhost:7067/api/Category';

  getAllCategories(): Observable<ApiResponse<Category[]>> {
    return this.http.get<ApiResponse<Category[]>>(
      `${this.categoryUrl}/GetAll`
    );
  }

  getAllCategoriesAdmin(includeDeleted: boolean = false): Observable<ApiResponse<Category[]>> {
    const url = `${this.categoryUrl}/GetAllAdmin?includeDeleted=${includeDeleted}`;
    return this.http.get<ApiResponse<Category[]>>(url);
  }

  addCategory(
    category: { name: string; description?: string }
  ): Observable<ApiResponse<Category>> {
    return this.http.post<ApiResponse<Category>>(
      `${this.categoryUrl}/Add`,
      category
    );
  }

  updateCategoryName(
    request: { categoryId: number; newName: string }
  ): Observable<ApiResponse<Category>> {
    return this.http.put<ApiResponse<Category>>(
      `${this.categoryUrl}/ChangeName`,
      request
    );
  }

  deleteCategory(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(
      `${this.categoryUrl}/Delete/${id}`
    );
  }

  restoreCategory(id: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(
      `${this.categoryUrl}/Restore/${id}`,
      {}
    );
  }

  getCategoryById(id: number): Observable<ApiResponse<Category>> {
    return this.http.get<ApiResponse<Category>>(
      `${this.categoryUrl}/GetById/${id}`
    );
  }
}
