# Implementation Details - Technical Reference

## Database Model

### ProductModel Properties Used

```csharp
public class ProductModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? Price { get; set; }
    public int? Stock { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int? CategoryId { get; set; }
    public bool IsDeleted { get; set; } = false;  // ← Used for soft delete
    
    [ForeignKey("CategoryId")]
    public CategoryModel? Category { get; set; }
}
```

**Key Points:**
- `IsDeleted` field handles soft deletes (product is marked, not permanently removed)
- `CategoryId` is used for category filtering
- `Price` is used for price range filtering
- `Stock` is used for quantity filtering and sorting

## DTO Updates

### PaginationRequest DTO Structure

```csharp
public class PaginationRequest
{
    // Existing properties
    public int Offset { get; set; }
    public int Limit { get; set; }
    public string? SearchTerm { get; set; }
    
    // New filter properties
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public string? SortByQuantity { get; set; }
    public bool IncludeDeleted { get; set; } = false;
}
```

### Response DTOs

**ProductResponseDTO** (User View)
```typescript
interface ProductResponse {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
  categoryName?: string;
  imageUrl?: string;
}
```

**AdminProductResponseDTO** (Admin View)
```typescript
interface AdminProductResponse extends ProductResponse {
  createdDate?: string;
  updatedDate?: string;
  isDeleted?: boolean;  // ← New property
}
```

## Repository Implementation Details

### GetAllProductsPaginatedWithFilters Method Logic

```csharp
public async Task<(int totalCount, IEnumerable<ProductModel> products)> 
    GetAllProductsPaginatedWithFilters(PaginationRequest request, bool isAdmin = false)
{
    // 1. Start with Products table
    var query = _context.Products.AsQueryable();
    
    // 2. Filter by deleted status
    if (!isAdmin)
    {
        // User view: exclude deleted products
        query = query.Where(p => !p.IsDeleted);
    }
    else if (!request.IncludeDeleted)
    {
        // Admin view with includeDeleted=false: exclude deleted
        query = query.Where(p => !p.IsDeleted);
    }
    // Admin with includeDeleted=true: show all
    
    // 3. Apply search filter
    if (!string.IsNullOrWhiteSpace(request.SearchTerm))
    {
        query = query.Where(p => p.Name.Contains(request.SearchTerm));
    }
    
    // 4. Apply category filter
    if (request.CategoryId.HasValue && request.CategoryId > 0)
    {
        query = query.Where(p => p.CategoryId == request.CategoryId);
    }
    
    // 5. Apply price range filters
    if (request.MinPrice.HasValue && request.MinPrice >= 0)
    {
        query = query.Where(p => p.Price >= request.MinPrice);
    }
    if (request.MaxPrice.HasValue && request.MaxPrice >= 0)
    {
        query = query.Where(p => p.Price <= request.MaxPrice);
    }
    
    // 6. Apply quantity filters (admin only)
    if (isAdmin)
    {
        if (request.MinQuantity.HasValue && request.MinQuantity >= 0)
        {
            query = query.Where(p => p.Stock >= request.MinQuantity);
        }
        if (request.MaxQuantity.HasValue && request.MaxQuantity >= 0)
        {
            query = query.Where(p => p.Stock <= request.MaxQuantity);
        }
    }
    
    // 7. Get total count before pagination
    var totalCount = await query.CountAsync();
    
    // 8. Apply sorting
    if (isAdmin && !string.IsNullOrWhiteSpace(request.SortByQuantity))
    {
        if (request.SortByQuantity.ToLower() == "asc")
        {
            query = query.OrderBy(p => p.Stock);
        }
        else if (request.SortByQuantity.ToLower() == "desc")
        {
            query = query.OrderByDescending(p => p.Stock);
        }
        else
        {
            query = query.OrderByDescending(p => p.Id);
        }
    }
    else
    {
        // Default: newest products first
        query = query.OrderByDescending(p => p.Id);
    }
    
    // 9. Apply pagination
    var products = await query
        .Include(p => p.Category)
        .Skip(request.Offset)
        .Take(request.Limit)
        .ToListAsync();
    
    // 10. Return results
    return (totalCount, products);
}
```

## Query Optimization

The implementation uses LINQ to Entities efficiently:

1. **Where Clauses First**: All filters are applied before pagination
2. **Count After Filtering**: Total count is calculated after all filters
3. **Eager Loading**: Category is eager loaded with `.Include()`
4. **Pagination Last**: Skip/Take are applied last to minimize data transfer
5. **Deferred Execution**: Query is not executed until `.ToListAsync()`

## Service Layer Pattern

```
UserRequest
    ↓
ProductController (parameter binding)
    ↓
PaginationRequest (DTO)
    ↓
ProductService (validation & mapping)
    ↓
ProductRepository (data access)
    ↓
Database
    ↓
Results returned & mapped back up
```

## Error Handling

- **Null Checks**: All optional filter parameters checked for null
- **Value Validation**: Price and quantity have `HasValue` checks
- **String Validation**: Search term trimmed and checked for whitespace
- **Pagination Validation**: Offset and limit validated in controller

## Performance Considerations

1. **Index Recommendations**: Consider adding indexes on:
   - `CategoryId` (foreign key)
   - `IsDeleted` (soft delete filter)
   - `Stock` (quantity sort)
   - `Price` (price range filter)

2. **Query Complexity**: The method handles multiple filters efficiently by:
   - Building query progressively
   - Only executing `.ToListAsync()` once
   - Using eager loading for Category

3. **Pagination**: Always uses offset/limit to prevent large data transfers

## Code Flow Example

**Scenario: Find low-stock electronics under $1000**

1. **Frontend**: Admin sets filters and clicks "Apply Filters"
2. **Component**: Calls `loadProducts(1)` with filter values
3. **Service**: Calls `getProductsAdminPaginated(1, 10, undefined, 3, 0, 1000, 0, 10, 'asc', false)`
4. **Controller**: Creates `PaginationRequest` with all filter values
5. **Service**: Validates and calls repository method with `isAdmin: true`
6. **Repository**: 
   - Starts with all products
   - Excludes deleted products (includeDeleted=false)
   - Filters by categoryId=3
   - Filters by minPrice=0, maxPrice=1000
   - Filters by minQuantity=0, maxQuantity=10
   - Counts total matching products
   - Sorts by stock ascending
   - Applies pagination (skip 0, take 10)
   - Includes category information
7. **Service**: Maps to AdminProductResponseDTO
8. **Controller**: Returns paginated response
9. **Frontend**: Displays products with status badges and restore buttons
