# Bulk Import - Quick Reference Guide

## 🚀 Quick Start

### For Admins:
1. Go to **Bulk Import** in the header navigation (only visible to Admin users)
2. Click file input and select Excel (.xlsx, .xls) or CSV (.csv) file
3. Click **"Import Data"** to preview
4. Review the preview - check for errors and data validity
5. If satisfied, click **"Upload Data"** to insert products into database

### Supported Files:
- ✅ Microsoft Excel (.xlsx, .xls)
- ✅ CSV (.csv)
- ❌ Other formats not supported

---

## 📋 File Format Requirements

### Column Order (Excel/CSV)

| # | Column Name | Type | Required | Example |
|---|------------|------|----------|---------|
| 1 | Name | String | ✓ | "Laptop" |
| 2 | Description | String | ✓ | "High-performance laptop" |
| 3 | ImageUrl | String | ✓ | "https://example.com/img.jpg" |
| 4 | Price | Decimal | ✓ | 1200.00 |
| 5 | Stock | Integer | ✓ | 50 |
| 6 | CategoryName | String | ✓ | "Electronics" |
| 7 | IsAvailable | Boolean | ✗ | true/false |

### CSV Header Example:
```csv
Name,Description,ImageUrl,Price,Stock,CategoryName,IsAvailable
Laptop,High-performance laptop,https://example.com/laptop.jpg,1200.00,50,Electronics,true
Mouse,Wireless mouse,https://example.com/mouse.jpg,25.99,150,Electronics,true
```

---

## 📊 Workflow Overview

```
┌─────────────┐
│  Select     │
│   File      │
└──────┬──────┘
       │
       ▼
┌─────────────────────┐
│  Import Data        │  ← Preview validation
│  (Preview)          │  ← Show errors
│                     │  ← Show first 10 records
└──────┬──────────────┘
       │
       ├─ Fix Errors? ─→ Re-import ─┐
       │                            │
       └──────────────┬─────────────┘
                      ▼
             ┌──────────────────┐
             │  Upload Data     │  ← Bulk Insert
             │  (BulkInsertAsync)│
             └────────┬─────────┘
                      ▼
            ┌──────────────────┐
            │ Show Results     │
            │ Success/Errors   │
            └──────────────────┘
```

---

## ✅ Validation Rules

All of these must be met for each product:

1. **Name** - Required, cannot be empty
2. **Description** - Required, cannot be empty
3. **ImageUrl** - Required, must be valid HTTP/HTTPS URL
4. **Price** - Required, must be > 0
5. **Stock** - Required, must be >= 0 (non-negative)
6. **CategoryName** - Required, must match an existing category in system
7. **IsAvailable** - Optional (defaults to true if not provided)

### Examples of Validation Errors:
```
Row 5: Product name is required
Row 8: Invalid image URL format (www.example.com is invalid, use http/https)
Row 12: Category 'InvalidCategory' not found
Row 15: Price must be greater than 0
Row 18: Stock cannot be negative
```

---

## 🎯 Step-by-Step Instructions

### Step 1: Navigate to Bulk Import
1. Login as Admin
2. In header, click **"Bulk Import"** link
3. You'll see the bulk import form

### Step 2: Select File
1. Click the file upload area (or drag & drop)
2. Select your Excel or CSV file
3. File name will appear in the upload area

### Step 3: Import Data (Preview)
1. Click **"Import Data"** button
2. System validates all rows
3. Shows:
   - Total records in file
   - Number of valid records
   - Number of invalid records
   - Detailed error messages (with row numbers)
   - Preview table with first 10 valid records

### Step 4: Review Preview
- Check the statistics cards
- Read through all error messages
- If errors exist, fix your file and re-import
- If all valid, proceed to upload

### Step 5: Upload Data
1. Click **"Upload Data"** button
2. System performs bulk insert
3. Shows completion status:
   - Number of products inserted
   - Number of records that failed
   - Any error details

### Step 6: Verify Results
1. Go to **"Manage Products"**
2. Search for your newly imported products
3. Verify data was inserted correctly

---

## 💡 Tips & Tricks

### 1. Download Template
- Click **"Download Template"** to get a sample CSV file
- Use this as a starting point for your product list

### 2. Large Files
- If importing 1000+ products, split into multiple files
- Each file should be <50MB for best performance

### 3. Category Names
- Before importing, check existing categories in **"Manage Categories"**
- Your product file must use exact category names (case-sensitive)

### 4. Image URLs
- Ensure all URLs are complete: `https://example.com/image.jpg`
- Do not use relative URLs or file paths
- Test URLs in browser first if unsure

### 5. Pricing Format
- Use decimal format: `1200.00`, `29.99`, `150`
- No currency symbols: ❌ `$1200`, ✅ `1200`

### 6. Error Investigation
- Each error message shows the row number
- Fix the specific row in your file
- Re-import after making corrections

---

## 🔄 Common Scenarios

### Scenario 1: Importing 100 Products
**File**: `bulk_products.csv` (100 rows)
1. Download template
2. Fill in all 100 products
3. Click Import Data → Check preview
4. Click Upload Data
5. Result: "Successfully imported 100 products"

### Scenario 2: Partial Import with Errors
**File**: `products.xlsx` (10 rows, 2 have errors)
1. Click Import Data
2. See: "8 valid records, 2 invalid records"
3. Review errors: e.g., "Row 3: Category not found", "Row 7: Invalid price"
4. Fix only rows 3 and 7
5. Re-import → Should show all 10 valid now
6. Click Upload Data

### Scenario 3: No Categories Match
**Problem**: All products fail with "Category X not found"
**Solution**:
1. Go to Manage Categories
2. Create missing categories
3. Re-import with correct category names

---

## 🛠️ Technical Details

### Backend APIs

**Preview Endpoint**:
```
POST /api/Product/BulkImport/Preview
Authorization: Bearer {token}
Content-Type: multipart/form-data
```

**Upload Endpoint**:
```
POST /api/Product/BulkImport/Upload
Authorization: Bearer {token}
Content-Type: multipart/form-data
```

### Performance
- Bulk insert rate: ~1000 products per batch
- Average import time: 1-5 seconds per 1000 products
- All records inserted in single database transaction

---

## 🆘 Troubleshooting

| Problem | Solution |
|---------|----------|
| "Unsupported file format" | Use .xlsx, .xls, or .csv only |
| "Category not found" | Verify category name exists in system |
| "Invalid image URL" | URLs must start with http:// or https:// |
| "Price must be > 0" | Ensure price is positive number |
| "Stock cannot be negative" | Ensure stock is 0 or positive |
| File won't upload | Try smaller file, refresh page |
| Nothing happens | Check browser console for errors |
| Too many errors | Validate locally before uploading |

---

## 📝 Example CSV File

Save as `products.csv`:

```csv
Name,Description,ImageUrl,Price,Stock,CategoryName,IsAvailable
Apple MacBook Pro,High-performance laptop with M3 chip,https://example.com/macbook.jpg,1999.99,25,Electronics,true
Sony WH-1000XM5,Premium noise-canceling headphones,https://example.com/sony_headphones.jpg,399.99,50,Electronics,true
Samsung 4K Monitor,32-inch 4K UHD display,https://example.com/samsung_monitor.jpg,799.99,15,Electronics,true
Logitech MX Master 3,Advanced wireless mouse,https://example.com/logitech_mouse.jpg,99.99,100,Electronics,true
Google Pixel 8,Latest smartphone,https://example.com/pixel8.jpg,799.00,40,Electronics,true
```

---

## 🎓 Best Practices

1. **Validate Before Upload**: Use "Import Data" to preview first
2. **Fix Errors Locally**: Edit your CSV/Excel file on your computer
3. **Use Exact Names**: Match category names exactly (including case)
4. **Test URLs**: Verify image URLs work before importing
5. **Small Batches**: Import 100-500 products per session initially
6. **Keep Backup**: Save your import file in case you need to re-import

---

## 📞 Support

If you encounter issues:
1. Check this guide's troubleshooting section
2. Review error messages carefully (they include row numbers)
3. Validate your file format matches requirements
4. Try importing a smaller test file first
5. Contact system administrator if problems persist

---

## ✨ Features Summary

✅ **Two-step Process**: Preview then Upload  
✅ **File Format Support**: Excel and CSV  
✅ **Batch Operations**: Import 1000+ products efficiently  
✅ **Error Reporting**: Detailed row-by-row error messages  
✅ **Data Validation**: Comprehensive validation before insert  
✅ **Preview Data**: See first 10 records before committing  
✅ **Admin Only**: Secure role-based access control  
✅ **Responsive Design**: Works on desktop and mobile  

---

**Version**: 1.0  
**Last Updated**: January 2026  
**Framework**: Angular (Frontend), ASP.NET Core (Backend)
