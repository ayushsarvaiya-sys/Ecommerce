# Filter Usage Guide

## How to Use the New Filters

### For Users (getProductsPaginated)

Users can filter products using:

```typescript
// Example: Get products by category and price range
this.productService.getProductsPaginated(
  page: 1,
  pageSize: 10,
  search: 'laptop',
  categoryId: 2,
  minPrice: 500,
  maxPrice: 100000
)
```

**Available Filters:**
- `search` - Product name search
- `categoryId` - Filter by category ID
- `minPrice` - Minimum price filter
- `maxPrice` - Maximum price filter

### For Admin (getProductsAdminPaginated)

Admins have access to all filters:

```typescript
// Example: Get low-stock products of a specific category, sorted by quantity
this.productService.getProductsAdminPaginated(
  page: 1,
  pageSize: 10,
  search: undefined,
  categoryId: 1,
  minPrice: undefined,
  maxPrice: undefined,
  minQuantity: 0,
  maxQuantity: 10,
  sortByQuantity: 'asc',
  includeDeleted: false
)
```

**Available Filters:**
- `search` - Product name search
- `categoryId` - Filter by category ID
- `minPrice` - Minimum price filter
- `maxPrice` - Maximum price filter
- `minQuantity` - Minimum stock quantity
- `maxQuantity` - Maximum stock quantity
- `sortByQuantity` - 'asc' or 'desc' (sort by stock level)
- `includeDeleted` - true/false (show deleted products)

## UI Features

### Filter Panel

Located in the admin products page, the filter panel includes:

1. **Category Filter**
   - Dropdown with all available categories
   - Select "All Categories" for no category filter

2. **Price Range Filters**
   - Min Price input (₹)
   - Max Price input (₹)
   - Both optional

3. **Quantity Filters** (Admin Only)
   - Min Quantity input
   - Max Quantity input
   - Useful for finding low-stock or high-stock items

4. **Quantity Sort** (Admin Only)
   - "No Sort" - Default sort by ID (newest first)
   - "Lowest Stock First" - Products with least stock first
   - "Highest Stock First" - Products with most stock first

5. **Show Deleted Products Checkbox** (Admin Only)
   - Enable to view deleted products in the list
   - Deleted products appear grayed out

### Buttons

- **Apply Filters** - Apply current filter selections and reload list
- **Reset All** - Clear all filters and reload with defaults

### Product Status Display

Each product row now shows:

- **Status Badge**
  - 🟢 Green "Active" for active products
  - 🔴 Red "Deleted" for deleted products

- **Action Buttons**
  - Active products: Edit, Restock, Delete buttons
  - Deleted products: Restore button only

## Common Use Cases

### Find Low-Stock Products
1. Set Min Quantity: leave empty
2. Set Max Quantity: 10
3. Set Sort by Quantity: "Lowest Stock First"
4. Click "Apply Filters"

### Find Expensive Products in a Category
1. Select Category: "Electronics"
2. Set Min Price: 50000
3. Click "Apply Filters"

### Search and Filter by Category
1. Enter search term: "iPhone"
2. Select Category: "Mobile Phones"
3. Click "Apply Filters"

### View Deleted Products
1. Check "Show Deleted Products"
2. Click "Apply Filters"
3. Deleted products now appear grayed out with "Deleted" badge
4. Click "Restore" button to undelete

### Sort by Stock Level
1. Set Sort by Quantity: "Lowest Stock First" (or "Highest Stock First")
2. Click "Apply Filters"
3. Products are now sorted by quantity level

## Filter Behavior

- **Default**: Deleted products are hidden, filters are empty
- **Search**: Works with product names (partial matching)
- **Category**: Filter by exact category ID
- **Price**: Filters are inclusive (min <= price <= max)
- **Quantity**: Filters are inclusive (min <= quantity <= max)
- **Sort**: Only applies when a value is selected (not "No Sort")
- **Deleted**: Only admins can see and manage deleted products

## API URL Examples

### User Product Search with Filters
```
GET https://localhost:7067/api/Product/GetPaginated?page=1&pageSize=10&search=laptop&categoryId=2&minPrice=500&maxPrice=100000
```

### Admin Product Search with All Filters
```
GET https://localhost:7067/api/Product/GetPaginatedAdmin?page=1&pageSize=10&search=&categoryId=1&minPrice=0&maxPrice=50000&minQuantity=0&maxQuantity=20&sortByQuantity=asc&includeDeleted=false
```

### View Deleted Products
```
GET https://localhost:7067/api/Product/GetPaginatedAdmin?page=1&pageSize=10&includeDeleted=true
```

### Restore a Deleted Product
```
POST https://localhost:7067/api/Product/Restore/5
```

## Notes

- All filter parameters are optional
- Leaving a filter blank means "no filter" for that field
- Multiple filters can be combined
- Filter state resets when page reloads or "Reset All" is clicked
- Deleted products cannot be edited, restocked, or deleted again (until restored)
