import { Component, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

interface BulkProductImportDTO {
  name?: string;
  description?: string;
  imageUrl?: string;
  price?: number;
  stock?: number;
  categoryName?: string;
  isAvailable?: boolean;
}

interface BulkImportPreviewDTO {
  totalRecords: number;
  validRecords: number;
  invalidRecords: number;
  previewData: BulkProductImportDTO[];
  errors: string[];
}

interface BulkImportResponseDTO {
  totalInserted: number;
  totalUpdated: number;
  totalFailed: number;
  errorMessages: string[];
  message: string;
}

interface ApiResponse<T> {
  statusCode: number;
  data: T;
  message: string;
}

@Component({
  selector: 'app-bulk-import-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './bulk-import-products.component.html',
  styleUrls: ['./bulk-import-products.component.css']
})
export class BulkImportProductsComponent {
  selectedFile: File | null = null;
  previewData: BulkImportPreviewDTO | null = null;
  importResult: BulkImportResponseDTO | null = null;
  isLoading: boolean = false;
  showPreview: boolean = false;
  showResult: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  private apiUrl = 'https://localhost:7067/api/Product';

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      const validExtensions = ['.xlsx', '.xls', '.csv'];
      const fileName = file.name.toLowerCase();
      const hasValidExtension = validExtensions.some(ext => fileName.endsWith(ext));

      if (hasValidExtension) {
        this.selectedFile = file;
        this.errorMessage = '';
        this.previewData = null;
        this.importResult = null;
      } else {
        this.errorMessage = 'Invalid file format. Please use Excel (.xlsx, .xls) or CSV (.csv) files.';
        this.selectedFile = null;
      }
    }
  }

  importData(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Please select a file first';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    this.http.post<ApiResponse<BulkImportPreviewDTO>>(
      `${this.apiUrl}/BulkImport/Preview`,
      formData
    ).subscribe({
      next: (response) => {
        console.log('Preview response:', response);
        console.log('Preview data from response:', response.data);
        this.previewData = response.data;
        this.showPreview = true;
        this.isLoading = false;
        console.log('showPreview set to:', this.showPreview);
        console.log('previewData set to:', this.previewData);
        
        if (this.previewData.errors.length > 0) {
          this.successMessage = `Preview loaded: ${this.previewData.validRecords} valid records, ${this.previewData.invalidRecords} invalid records`;
        } else {
          this.successMessage = `Preview loaded successfully: ${this.previewData.totalRecords} records ready to import`;
        }
        
        this.cdr.markForCheck();
        console.log('Change detection triggered');
      },
      error: (error) => {
        console.error('Preview error details:', error);
        this.isLoading = false;
        this.errorMessage = error.error?.message || error.message || 'Failed to preview data';
        console.log('Error message set to:', this.errorMessage);
        this.showPreview = false;
        this.cdr.markForCheck();
      }
    });
  }

  uploadData(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Please select a file first';
      return;
    }

    if (!this.previewData || this.previewData.validRecords === 0) {
      this.errorMessage = 'Please import and preview the data first';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    this.http.post<ApiResponse<BulkImportResponseDTO>>(
      `${this.apiUrl}/BulkImport/Upload`,
      formData
    ).subscribe({
      next: (response) => {
        console.log('Upload response:', response);
        this.importResult = response.data;
        this.showResult = true;
        this.isLoading = false;
        this.successMessage = response.data.message;
        
        // Reset file after successful upload
        this.selectedFile = null;
        this.previewData = null;
        this.showPreview = false;
        this.cdr.markForCheck();
        console.log('Upload completed, change detection triggered');
      },
      error: (error) => {
        console.error('Upload error details:', error);
        this.isLoading = false;
        this.errorMessage = error.error?.message || error.message || 'Failed to upload data';
        console.log('Error message set to:', this.errorMessage);
        this.showResult = false;
        this.cdr.markForCheck();
      }
    });
  }

  resetForm(): void {
    this.selectedFile = null;
    this.previewData = null;
    this.importResult = null;
    this.showPreview = false;
    this.showResult = false;
    this.errorMessage = '';
    this.successMessage = '';
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  downloadTemplate(): void {
    // Create a sample CSV template
    const csvContent = `Name,Description,ImageUrl,Price,Stock,CategoryName,IsAvailable
Sample Product,This is a sample product,https://example.com/image.jpg,99.99,100,Electronics,true`;

    const blob = new Blob([csvContent], { type: 'text/csv' });
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = 'product-import-template.csv';
    link.click();
  }
}
