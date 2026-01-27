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
    search?: string
  ): Observable<ApiResponse<PaginatedResponse<ProductResponse>>> {
    let url = `${this.apiUrl}/GetPaginated?page=${page}&pageSize=${pageSize}`;
    if (search) {
      url += `&search=${encodeURIComponent(search)}`;
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
    search?: string
  ): Observable<ApiResponse<PaginatedResponse<AdminProductResponse>>> {
    let url = `${this.apiUrl}/GetPaginatedAdmin?page=${page}&pageSize=${pageSize}`;
    if (search) {
      url += `&search=${encodeURIComponent(search)}`;
    }
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
  getAllCategories(): Observable<ApiResponse<Category[]>> {
    return this.http.get<ApiResponse<Category[]>>(
      `https://localhost:7067/api/Category/GetAll`
    );
  }
}
