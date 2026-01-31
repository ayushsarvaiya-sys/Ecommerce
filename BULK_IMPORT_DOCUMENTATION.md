# Bulk Import Products Feature Documentation

## Overview

The Bulk Import Products feature allows administrators to efficiently import large quantities of products into the e-commerce system using Excel (.xlsx, .xls) or CSV (.csv) files. The system provides a two-step process: **Import Data** to preview the data, and **Upload Data** to perform the actual bulk insert using EFCore.BulkExtensions.

---

## Backend Implementation

### 1. NuGet Packages Added

The following packages have been added to `ECommerce.csproj`:

```xml
<PackageReference Include="EFCore.BulkExtensions" Version="10.0.0" />
<PackageReference Include="EPPlus" Version="7.4.2" />
<PackageReference Include="CsvHelper" Version="31.0.4" />
```

- **EFCore.BulkExtensions**: High-performance bulk operations for Entity Framework Core
- **EPPlus**: Excel file reading and writing
- **CsvHelper**: CSV file parsing

### 2. DTOs Created

**File**: `DTO/BulkProductImportDTO.cs`

```csharp
public class BulkProductImportDTO
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public string? CategoryName { get; set; }
    public bool? IsAvailable { get; set; }
}

public class BulkImportPreviewDTO
{
    public int TotalRecords { get; set; }
    public int ValidRecords { get; set; }
    public int InvalidRecords { get; set; }
    public List<BulkProductImportDTO> PreviewData { get; set; } = new();
    public List<string> Errors { get; set; } = new();
}

public class BulkImportResponseDTO
{
    public int TotalInserted { get; set; }
    public int TotalFailed { get; set; }
    public List<string> ErrorMessages { get; set; } = new();
    public string Message { get; set; } = "";
}
```

### 3. File Parsing Utility

**File**: `Utils/FileParsingHelper.cs`

Provides static methods for file parsing:

- `ParseExcelFileAsync(IFormFile file)` - Parses Excel files using EPPlus
- `ParseCsvFileAsync(IFormFile file)` - Parses CSV files using CsvHelper
- `ParseFileAsync(IFormFile file)` - Auto-detects file type and parses accordingly
- `ValidateProducts(List<BulkProductImportDTO> products)` - Validates product data with detailed error messages

**Validation Rules**:
- Product name is required
- Description is required
- Image URL is required and must be valid
- Price must be > 0
- Stock must be >= 0
- Category name must exist
- All errors are reported with row numbers

### 4. Service Interface

**File**: `Interfaces/IProductBulkService.cs`

```csharp
public interface IProductBulkService
{
    Task<BulkImportPreviewDTO> PreviewBulkImportAsync(IFormFile file);
    Task<BulkImportResponseDTO> BulkImportProductsAsync(IFormFile file);
}
```

### 5. Service Implementation

**File**: `Services/ProductBulkService.cs`

**PreviewBulkImportAsync**:
- Parses the file
- Validates all products
- Separates valid and invalid records
- Returns a preview with first 10 valid records
- Includes validation errors

**BulkImportProductsAsync**:
- Parses and validates the file
- Fetches all categories from the database
- Maps category names to category IDs
- Converts DTOs to ProductModel entities
- Uses `BulkInsertAsync` with batch size of 1000 for optimal performance
- Returns success/failure summary

### 6. API Endpoints

**File**: `Controllers/ProductController.cs`

**Endpoint 1: Preview Import Data**
```
POST /api/Product/BulkImport/Preview
Authorization: Bearer token
Content-Type: multipart/form-data
Body: file (Excel or CSV file)

Response:
{
  "statusCode": 200,
  "data": {
    "totalRecords": 100,
    "validRecords": 98,
    "invalidRecords": 2,
    "previewData": [...], // First 10 valid records
    "errors": ["Row 5: Invalid price format", "Row 12: Category not found"]
  },
  "message": "Preview generated successfully"
}
```

**Endpoint 2: Upload/Bulk Insert Data**
```
POST /api/Product/BulkImport/Upload
Authorization: Bearer token
Content-Type: multipart/form-data
Body: file (Excel or CSV file)

Response:
{
  "statusCode": 200,
  "data": {
    "totalInserted": 98,
    "totalFailed": 2,
    "errorMessages": ["Row 5: Invalid price format"],
    "message": "Successfully imported 98 products. 2 records failed due to validation errors."
  },
  "message": "Successfully imported 98 products. 2 records failed due to validation errors."
}
```

### 7. Dependency Injection

**File**: `Program.cs`

Added registration:
```csharp
builder.Services.AddScoped<IProductBulkService, ProductBulkService>();
```

---

## Frontend Implementation

### 1. Component Structure

**File**: `src/app/components/bulk-import-products/`

The component is a standalone Angular component with:
- File upload with drag-and-drop support
- Two-step process: Import Data → Upload Data
- Real-time validation and error handling
- Data preview with statistics
- Success/failure reporting

### 2. Component TypeScript

**File**: `bulk-import-products.component.ts`

**Key Methods**:

- `onFileSelected(event)` - Validates file format (.xlsx, .xls, .csv)
- `importData()` - Calls preview API endpoint
- `uploadData()` - Calls bulk insert API endpoint
- `resetForm()` - Clears all form data and states
- `downloadTemplate()` - Downloads CSV template for users

**State Management**:
```typescript
selectedFile: File | null = null;
previewData: BulkImportPreviewDTO | null = null;
importResult: BulkImportResponseDTO | null = null;
isLoading: boolean = false;
showPreview: boolean = false;
showResult: boolean = false;
errorMessage: string = '';
successMessage: string = '';
```

### 3. Component Template

**File**: `bulk-import-products.component.html`

**Sections**:

1. **File Upload Section**
   - File input with drag-and-drop style
   - Import Data button
   - Download Template button

2. **Preview Section** (shown after importing)
   - Statistics cards (Total, Valid, Invalid)
   - Validation errors list
   - Preview table with first 10 records
   - Upload Data button

3. **Result Section** (shown after uploading)
   - Success/failure statistics
   - Error details
   - Import Another File button

### 4. Component Styling

**File**: `bulk-import-products.component.css`

- Responsive grid layout
- Color-coded cards (success=green, error=red, warning=yellow)
- Hover effects and transitions
- Mobile-friendly design
- Dark theme compatible

### 5. Component Specification Tests

**File**: `bulk-import-products.component.spec.ts`

Tests cover:
- Component creation
- File validation
- Invalid file format rejection
- Form reset functionality

### 6. Routing

**File**: `src/app/app.routes.ts`

Added route:
```typescript
{ path: 'admin/bulk-import', component: BulkImportProductsComponent, canActivate: [AuthGuard] }
```

### 7. Navigation

**File**: `src/app/shared/header/header.component.html`

Added link in admin navigation:
```html
<a 
  *ngIf="currentUser()?.role === 'Admin'" 
  routerLink="/admin/bulk-import" 
  routerLinkActive="active"
  class="admin-link"
>
  Bulk Import
</a>
```

---

## File Format Specifications

### Excel File Format (.xlsx, .xls)

Column order (no headers needed, data starts from row 1):

| Column | Header | Data Type | Required | Notes |
|--------|--------|-----------|----------|-------|
| A | Name | String | Yes | Product name, max 100 chars |
| B | Description | String | Yes | Product description, max 500 chars |
| C | ImageUrl | String | Yes | Valid URL format (http/https) |
| D | Price | Decimal | Yes | Must be > 0 |
| E | Stock | Integer | Yes | Must be >= 0 |
| F | CategoryName | String | Yes | Must match existing category |
| G | IsAvailable | Boolean | No | true/false, yes/no, 1/0 (default: true) |

### CSV File Format (.csv)

Required headers (case-sensitive):
```csv
Name,Description,ImageUrl,Price,Stock,CategoryName,IsAvailable
```

Example:
```csv
Name,Description,ImageUrl,Price,Stock,CategoryName,IsAvailable
Laptop,High-performance laptop,https://example.com/laptop.jpg,1200.00,50,Electronics,true
Mouse,Wireless mouse,https://example.com/mouse.jpg,25.99,150,Electronics,true
```

### Download Template

The frontend provides a CSV template download functionality. Sample template:
```csv
Name,Description,ImageUrl,Price,Stock,CategoryName,IsAvailable
Sample Product,This is a sample product,https://example.com/image.jpg,99.99,100,Electronics,true
```

---

## Usage Workflow

### Step 1: Access Bulk Import Page
1. Admin logs in
2. Click "Bulk Import" in header navigation
3. Page displays file upload interface

### Step 2: Select and Preview File
1. Click file input or drag-and-drop file
2. Supported formats: .xlsx, .xls, .csv
3. Click "Import Data" button
4. System validates and shows preview:
   - Statistics (Total, Valid, Invalid records)
   - Any validation errors (by row)
   - First 10 valid records in table

### Step 3: Review and Upload
1. Review preview data and any errors
2. If errors exist, fix the file and re-import
3. Click "Upload Data" button
4. System performs bulk insert using BulkInsertAsync
5. Shows result summary:
   - Number of products successfully inserted
   - Number of records failed
   - Detailed error messages

### Step 4: Verify Results
1. Check the success message
2. Navigate to "Manage Products" to verify imported products
3. Can import another file by clicking "Import Another File"

---

## Error Handling

### Validation Errors

All validation is performed before any database operations:

```
Row 2: Product name is required
Row 5: Invalid image URL format (http://example.com/broken.jpg is invalid)
Row 8: Category 'NonExistent' not found
Row 12: Price must be greater than 0
Row 15: Stock cannot be negative
```

### API Error Responses

```json
{
  "statusCode": 400,
  "message": "File is empty"
}
```

```json
{
  "statusCode": 400,
  "message": "Unsupported file format. Please use .xlsx, .xls, or .csv files"
}
```

### Frontend Error Handling

- File format validation before upload
- API error messages displayed to user
- Specific row number identification for data errors
- Graceful handling of network failures

---

## Performance Considerations

### Backend

- **Batch Size**: 1000 products per batch (configurable in BulkConfig)
- **BulkInsertAsync**: High-performance bulk insert using EFCore.BulkExtensions
- **Statistics**: Calculated automatically by bulk operation
- **Transaction**: All records inserted in single transaction (atomic)

### Frontend

- **Preview Data**: First 10 records shown (to avoid UI lag)
- **File Size**: No hard limit, but realistic max ~50MB
- **Processing**: Validation occurs in background with loading state

---

## Example Import Session

**File**: `products.xlsx`

**Contents**:
```
Name,Description,ImageUrl,Price,Stock,CategoryName,IsAvailable
Laptop,High-end laptop,https://cdn.example.com/laptop.jpg,1500,25,Electronics,true
Keyboard,Mechanical keyboard,https://cdn.example.com/keyboard.jpg,150,100,Electronics,true
Monitor,4K Monitor,https://cdn.example.com/monitor.jpg,500,30,Electronics,true
```

**Step 1**: Admin uploads file and clicks "Import Data"

**Preview Response**:
```json
{
  "totalRecords": 3,
  "validRecords": 3,
  "invalidRecords": 0,
  "previewData": [
    {
      "name": "Laptop",
      "price": 1500,
      "stock": 25,
      "categoryName": "Electronics"
    },
    // ...more records
  ],
  "errors": []
}
```

**Step 2**: Admin reviews preview and clicks "Upload Data"

**Upload Response**:
```json
{
  "totalInserted": 3,
  "totalFailed": 0,
  "errorMessages": [],
  "message": "Successfully imported 3 products. 0 records failed due to validation errors."
}
```

**Result**: 3 products are now in the database and visible in "Manage Products"

---

## Security Considerations

1. **Authorization**: Only Admin users can access bulk import (checked in controller and routes)
2. **File Validation**: Only .xlsx, .xls, .csv files accepted
3. **Data Validation**: All data validated before database insertion
4. **SQL Injection Prevention**: Using parameterized queries via EF Core
5. **CORS**: Protected by CORS policy (localhost:4200)

---

## Future Enhancements

1. **Async Background Processing**: For very large files (100k+ records)
2. **Email Notifications**: Send admin email with import results
3. **Import History**: Track all import operations with timestamps
4. **Duplicate Detection**: Flag duplicate products before import
5. **Image Upload Integration**: Automatically upload images to Cloudinary
6. **Rollback Functionality**: Ability to rollback failed imports
7. **Batch Scheduling**: Schedule imports for off-peak hours
8. **Progress Tracking**: Real-time progress for large imports

---

## Troubleshooting

### Common Issues

**Issue**: "Unsupported file format"
- **Solution**: Use .xlsx, .xls, or .csv files only

**Issue**: "Category not found"
- **Solution**: Ensure category names in file match exactly with system categories

**Issue**: "Invalid image URL format"
- **Solution**: URLs must start with http:// or https://

**Issue**: "Price must be greater than 0"
- **Solution**: Price field must contain positive numbers only

**Issue**: Large file causes browser to hang
- **Solution**: Split file into smaller chunks (max 50MB recommended)

---

## API Integration Example (cURL)

### Preview Request
```bash
curl -X POST http://localhost:5050/api/Product/BulkImport/Preview \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "file=@products.xlsx"
```

### Upload Request
```bash
curl -X POST http://localhost:5050/api/Product/BulkImport/Upload \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "file=@products.xlsx"
```

---

## Files Modified/Created

### Backend Files

**Created**:
- `DTO/BulkProductImportDTO.cs` - DTOs for bulk import
- `Utils/FileParsingHelper.cs` - File parsing utilities
- `Interfaces/IProductBulkService.cs` - Service interface
- `Services/ProductBulkService.cs` - Service implementation

**Modified**:
- `ECommerce.csproj` - Added NuGet packages
- `Controllers/ProductController.cs` - Added API endpoints
- `Program.cs` - Registered service in DI container

### Frontend Files

**Created**:
- `src/app/components/bulk-import-products/bulk-import-products.component.ts` - Component logic
- `src/app/components/bulk-import-products/bulk-import-products.component.html` - Template
- `src/app/components/bulk-import-products/bulk-import-products.component.css` - Styling
- `src/app/components/bulk-import-products/bulk-import-products.component.spec.ts` - Tests

**Modified**:
- `src/app/app.routes.ts` - Added bulk import route
- `src/app/shared/header/header.component.html` - Added navigation link

---

## Conclusion

The Bulk Import Products feature provides a robust, user-friendly way for administrators to efficiently manage large product inventories. With comprehensive validation, error reporting, and high-performance bulk operations, it handles both small and large-scale product imports reliably.
