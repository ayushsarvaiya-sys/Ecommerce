# Filter System Architecture & Data Flow

## System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    ANGULAR FRONTEND                             │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │        AdminProductsComponent (TypeScript)               │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ Properties:                                              │  │
│  │  - searchTerm: string                                    │  │
│  │  - selectedCategoryId: number                            │  │
│  │  - minPrice: number                                      │  │
│  │  - maxPrice: number                                      │  │
│  │  - minQuantity: number                                   │  │
│  │  - maxQuantity: number                                   │  │
│  │  - sortByQuantity: string                                │  │
│  │  - includeDeleted: boolean                               │  │
│  │                                                          │  │
│  │ Methods:                                                 │  │
│  │  - loadProducts(page)                                    │  │
│  │  - applyFilters()                                        │  │
│  │  - resetFilters()                                        │  │
│  │  - restoreProduct(id)                                    │  │
│  └──────────────────────────────────────────────────────────┘  │
│                             ↓                                    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │      ProductService (Angular Service)                    │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ Methods:                                                 │  │
│  │  - getProductsAdminPaginated(...filters)                 │  │
│  │  - restoreProduct(id)                                    │  │
│  └──────────────────────────────────────────────────────────┘  │
│                             ↓                                    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │    HTML Template (admin-products.component.html)         │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │  - Filter Panel Section                                  │  │
│  │  - Products Table with Status Column                     │  │
│  │  - Restore Button for Deleted Products                   │  │
│  └──────────────────────────────────────────────────────────┘  │
│                             ↓                                    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │    CSS Styles (admin-products.component.css)             │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │  - .filters-section                                      │  │
│  │  - .filter-group, .filter-input                          │  │
│  │  - .badge-active, .badge-deleted                         │  │
│  │  - .btn-restore                                          │  │
│  └──────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                             ↓ HTTP
┌─────────────────────────────────────────────────────────────────┐
│                   .NET BACKEND API                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │    ProductController (ASP.NET Core)                      │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ Endpoints:                                               │  │
│  │  [GET] /api/Product/GetPaginatedAdmin                    │  │
│  │  [POST] /api/Product/Restore/{id}                        │  │
│  │                                                          │  │
│  │ Receives filter parameters:                              │  │
│  │  - categoryId, minPrice, maxPrice                        │  │
│  │  - minQuantity, maxQuantity                              │  │
│  │  - sortByQuantity, includeDeleted                        │  │
│  └──────────────────────────────────────────────────────────┘  │
│                             ↓                                    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │    ProductService (Business Logic)                       │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ Methods:                                                 │  │
│  │  - GetProductsAdminPaginatedService(request)             │  │
│  │  - RestoreProductService(id)                             │  │
│  │                                                          │  │
│  │ Validates and maps data                                  │  │
│  └──────────────────────────────────────────────────────────┘  │
│                             ↓                                    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │    ProductRepository (Data Access)                       │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ Methods:                                                 │  │
│  │  - GetAllProductsPaginatedWithFilters(request)           │  │
│  │    ├─ Filters by: Category, Price, Quantity             │  │
│  │    ├─ Sorts by: Stock (ASC/DESC)                         │  │
│  │    ├─ Handles: IsDeleted flag                            │  │
│  │    ├─ Applies: Pagination (Skip/Take)                    │  │
│  │    └─ Includes: Category navigation property             │  │
│  │                                                          │  │
│  │  - RestoreProduct(id)                                    │  │
│  │    └─ Sets IsDeleted = false                             │  │
│  └──────────────────────────────────────────────────────────┘  │
│                             ↓                                    │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │    Entity Framework DbContext                            │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │  - Products DbSet<ProductModel>                          │  │
│  └──────────────────────────────────────────────────────────┘  │
│                             ↓                                    │
└─────────────────────────────────────────────────────────────────┘
                             ↓ SQL
┌─────────────────────────────────────────────────────────────────┐
│                   SQL SERVER DATABASE                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │           Products Table                                 │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ Columns:                                                 │  │
│  │  - ProductId (PK)                                        │  │
│  │  - Name                                                  │  │
│  │  - Description                                           │  │
│  │  - Price      ← Used for price filtering                 │  │
│  │  - Stock      ← Used for quantity filtering & sorting    │  │
│  │  - CategoryId ← Used for category filtering              │  │
│  │  - IsDeleted  ← Used for soft delete filtering           │  │
│  │  - ImageUrl                                              │  │
│  │  - IsAvailable                                           │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │        Categories Table                                  │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ Columns:                                                 │  │
│  │  - CategoryId (PK)                                       │  │
│  │  - Name                                                  │  │
│  │  - Description                                           │  │
│  └──────────────────────────────────────────────────────────┘  │
│                                                                 │
│  Recommended Indexes:                                           │
│  - idx_products_deleted ON IsDeleted                            │
│  - idx_products_category ON CategoryId                          │
│  - idx_products_price ON Price                                  │
│  - idx_products_stock ON Stock                                  │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## Data Flow - Step by Step

### Filter Application Flow

```
User Action in UI
        ↓
Admin clicks "Apply Filters"
        ↓
Component.applyFilters()
    - Sets currentPage = 1
    - Calls loadProducts(1)
        ↓
Component.loadProducts(page)
    - Collects all filter values
    - Calls ProductService.getProductsAdminPaginated(...)
        ↓
ProductService.getProductsAdminPaginated(...)
    - Builds URL with query parameters
    - Makes HTTP GET request
        ↓
HTTP Request to Backend
    GET /api/Product/GetPaginatedAdmin?
        page=1&pageSize=10&
        categoryId=2&minPrice=100&maxPrice=5000&
        minQuantity=5&maxQuantity=100&
        sortByQuantity=asc&includeDeleted=false
        ↓
ProductController.GetProductsAdminPaginated()
    - Receives parameters
    - Validates page/pageSize
    - Creates PaginationRequest object with all filters
    - Calls ProductService.GetProductsAdminPaginatedService()
        ↓
ProductService.GetProductsAdminPaginatedService()
    - Calls ProductRepository.GetAllProductsPaginatedWithFilters()
    - Maps results to AdminProductResponseDTO
    - Returns PaginatedResponse
        ↓
ProductRepository.GetAllProductsPaginatedWithFilters()
    1. Start: var query = context.Products.AsQueryable()
    2. Filter: WHERE !IsDeleted (admin with includeDeleted=false)
    3. Filter: WHERE Name LIKE searchTerm
    4. Filter: WHERE CategoryId = categoryId
    5. Filter: WHERE Price >= minPrice AND Price <= maxPrice
    6. Filter: WHERE Stock >= minQuantity AND Stock <= maxQuantity
    7. Count: totalCount = COUNT(*)
    8. Sort: ORDER BY Stock ASC (if sortByQuantity=asc)
    9. Include: .Include(Category)
    10. Paginate: .Skip(offset).Take(limit)
    11. Execute: .ToListAsync()
        ↓
Database Query Execution
    SELECT * FROM Products
    WHERE IsDeleted = 0
    AND Name LIKE '%search%'
    AND CategoryId = 2
    AND Price BETWEEN 100 AND 5000
    AND Stock BETWEEN 5 AND 100
    ORDER BY Stock ASC
    OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
    JOIN Categories ON Products.CategoryId = Categories.CategoryId
        ↓
Results Returned to Repository
        ↓
Service Maps Results
    ProductModel → AdminProductResponseDTO
    - Includes isDeleted property
    - Includes categoryName
        ↓
Response Sent to Frontend
    {
      statusCode: 200,
      data: {
        data: [...products with isDeleted property],
        totalCount: 45,
        offset: 0,
        limit: 10,
        currentPageCount: 10,
        hasMore: true
      }
    }
        ↓
Frontend Receives Response
        ↓
Component.loadProducts() - subscribe.next()
    - Sets this.products = response.data.data
    - Sets this.totalCount = response.data.totalCount
    - Calculates totalPages
        ↓
Template Updates
    - Products table re-renders
    - Status column shows badges
    - Restore button appears for deleted products
    - Action buttons hidden for deleted products
        ↓
UI Displays Filtered Results
```

---

## Restore Product Flow

```
User clicks "Restore" button on deleted product
        ↓
Component.restoreProduct(productId)
        ↓
ProductService.restoreProduct(productId)
    - Makes HTTP POST request
        ↓
HTTP Request
    POST /api/Product/Restore/5
        ↓
ProductController.RestoreProduct(id)
    - Validates authorization (Admin only)
    - Creates PaginationRequest
    - Calls ProductService.RestoreProductService(id)
        ↓
ProductService.RestoreProductService(id)
    - Calls ProductRepository.RestoreProduct(id)
        ↓
ProductRepository.RestoreProduct(id)
    1. Find product by ID
    2. Set IsDeleted = false
    3. Save changes
    4. Return true/false
        ↓
Database Update
    UPDATE Products SET IsDeleted = 0 WHERE ProductId = 5
        ↓
Response Sent: { statusCode: 200, data: true, message: "..." }
        ↓
Frontend .subscribe.next()
    - Shows success message
    - Reloads product list
    - Component.loadProducts()
        ↓
Product Now Shows as Active
    - Disappears from deleted list
    - Status badge changes to green "Active"
    - Edit/Restock/Delete buttons appear
```

---

## Filter Parameter Flow

### Example: Find Low-Stock Electronics

```
Frontend State:
  selectedCategoryId: 3      (Electronics)
  minPrice: 0
  maxPrice: 999999
  minQuantity: 0
  maxQuantity: 10           ← Find products with ≤10 stock
  sortByQuantity: "asc"     ← Sort lowest to highest
  includeDeleted: false

↓ (HTTP Request)

URL:
  /api/Product/GetPaginatedAdmin?
    page=1&
    pageSize=10&
    categoryId=3&
    minPrice=0&
    maxPrice=999999&
    minQuantity=0&
    maxQuantity=10&
    sortByQuantity=asc&
    includeDeleted=false

↓ (Query Building)

LINQ Query:
  Products
    .Where(p => !p.IsDeleted)                    // exclude deleted
    .Where(p => p.CategoryId == 3)               // Electronics
    .Where(p => p.Price >= 0)                    // min price
    .Where(p => p.Price <= 999999)               // max price
    .Where(p => p.Stock >= 0)                    // min qty
    .Where(p => p.Stock <= 10)                   // max qty ← KEY
    .OrderBy(p => p.Stock)                       // sort ASC
    .Skip(0)
    .Take(10)

↓ (SQL Generated)

SELECT * FROM Products
WHERE IsDeleted = 0
AND CategoryId = 3
AND Price >= 0
AND Price <= 999999
AND Stock >= 0
AND Stock <= 10              ← Low stock items
ORDER BY Stock ASC
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY

↓ (Results)

Products returned in order:
  Product 1: Stock = 2
  Product 2: Stock = 3
  Product 3: Stock = 5
  Product 4: Stock = 7
  Product 5: Stock = 10
  ...
```

---

## DTOs & Request/Response Structure

### Request (PaginationRequest)

```csharp
{
  "offset": 0,
  "limit": 10,
  "searchTerm": "laptop",
  "categoryId": 2,
  "minPrice": 500.00,
  "maxPrice": 100000.00,
  "minQuantity": 5,
  "maxQuantity": 100,
  "sortByQuantity": "asc",
  "includeDeleted": false
}
```

### Response (AdminProductResponse in PaginatedResponse)

```typescript
{
  statusCode: 200,
  data: {
    data: [
      {
        id: 1,
        name: "Laptop Pro",
        description: "High-performance laptop",
        price: 75000.00,
        stock: 7,
        categoryId: 2,
        categoryName: "Electronics",
        imageUrl: "https://...",
        isDeleted: false,        ← New property
        createdDate: "2026-01-20",
        updatedDate: "2026-01-28"
      },
      ...more products
    ],
    totalCount: 45,
    offset: 0,
    limit: 10,
    currentPageCount: 10,
    hasMore: true
  },
  message: "Products retrieved successfully"
}
```

---

## Key Design Decisions

1. **Soft Delete** - Products marked not deleted, allowing recovery
2. **Server-Side Filtering** - All filters applied on backend for performance
3. **Query Building** - Progressive WHERE clauses for maintainability
4. **Pagination First** - Efficient handling of large datasets
5. **Optional Filters** - All filters optional, sensible defaults
6. **Admin Privilege** - Quantity/Delete features admin-only

---

## Performance Optimization

```
Optimization                    Benefit
─────────────────────────────────────────────────────
Count before pagination         Get accurate total before slice
Eager loading Category          Avoid N+1 queries
Filter before pagination        Reduce data transfer
Index on IsDeleted              Fast deleted product filtering
Index on CategoryId             Fast category filtering
Index on Stock                  Fast quantity filtering
Index on Price                  Fast price filtering
LINQ to Entities                Compiled to efficient SQL
Async/Await                     Non-blocking I/O
```

---

**Architecture Version**: 1.0
**Last Updated**: January 28, 2026
**Status**: Production Ready
