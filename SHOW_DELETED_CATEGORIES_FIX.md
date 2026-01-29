# Category Management - Show Deleted Categories Fix

## Summary
Fixed the "Show Deleted" feature to properly display only deleted categories when selected, respecting the global query filter on the Category model.

## Backend Changes

### 1. CategoryRepository (`ECommerce/Repositories/CategoryRepository.cs`)
- Added `GetAllCategoriesAdmin(bool includeDeleted = false)` method
  - Uses `IgnoreQueryFilters()` to bypass the global soft delete filter when `includeDeleted = true`
  - Shows only deleted categories when requested
- Added `RestoreCategory(int id)` method
  - Uses `IgnoreQueryFilters()` to find deleted categories
  - Sets `IsDeleted = false` to restore

### 2. ICategoryRepository Interface (`ECommerce/Interfaces/ICategoryRepository.cs`)
- Added `GetAllCategoriesAdmin(bool includeDeleted = false)` method signature
- Added `RestoreCategory(int id)` method signature

### 3. CategoryService (`ECommerce/Services/CategoryService.cs`)
- Added `GetAllCategoriesAdminService(bool includeDeleted = false)` method
  - Delegates to repository and maps to DTOs
- Added `RestoreCategoryService(int id)` method

### 4. ICategoryService Interface (`ECommerce/Interfaces/ICategoryService.cs`)
- Added `GetAllCategoriesAdminService(bool includeDeleted = false)` method signature
- Added `RestoreCategoryService(int id)` method signature

### 5. CategoryController (`ECommerce/Controllers/CategoryController.cs`)
- Added new API endpoint `[HttpGet("GetAllAdmin")]`
  - Accepts `includeDeleted` query parameter
  - Protected with `[Authorize(Roles = "Admin")]`
  - Returns all categories (active or deleted) based on parameter
- Added new API endpoint `[HttpPost("Restore/{id}")]`
  - Restores a deleted category by setting `IsDeleted = false`
  - Protected with `[Authorize(Roles = "Admin")]`

## Frontend Changes

### 1. ProductService (`src/app/services/product.service.ts`)
- Added `getAllCategoriesAdmin(includeDeleted: boolean = false)` method
  - Calls new backend endpoint `/api/Category/GetAllAdmin?includeDeleted=`
  - Returns categories based on the includeDeleted parameter

### 2. AdminCategoriesComponent (`src/app/components/admin-categories/admin-categories.component.ts`)
- Updated `loadCategories()` method:
  - Now calls `getAllCategoriesAdmin(this.includeDeleted)` instead of `getAllCategories()`
  - Filters results based on `includeDeleted` flag:
    - When `false`: Shows only active (non-deleted) categories
    - When `true`: Shows only deleted categories
  - Properly handles the backend response

## API Endpoints

### New Endpoint for Admin Category Management
```
GET /api/Category/GetAllAdmin?includeDeleted=false|true
Authorization: Bearer {token}
Roles: Admin
```

Query Parameters:
- `includeDeleted` (bool, optional, default: false)
  - `false`: Returns only non-deleted categories
  - `true`: Returns only deleted categories (bypasses global filter)

### Restore Endpoint
```
POST /api/Category/Restore/{id}
Authorization: Bearer {token}
Roles: Admin
```

## How It Works

1. **Global Query Filter**: The DbContext has a query filter that excludes deleted items by default
   - `modelBuilder.Entity<CategoryModel>().HasQueryFilter(m => !m.IsDeleted);`

2. **Show Deleted Toggle**:
   - User checks "Show Deleted" checkbox
   - `includeDeleted` is set to `true`
   - Component calls `getAllCategoriesAdmin(true)`
   - Backend calls `GetAllCategoriesAdmin(true)` which uses `IgnoreQueryFilters()`
   - Only deleted categories are returned and displayed

3. **Restore Action**:
   - User clicks "Restore" button on a deleted category
   - Component calls `restoreCategory(id)`
   - Backend finds the deleted category using `IgnoreQueryFilters()`
   - Sets `IsDeleted = false`
   - Category reappears in the active list

## User Experience

### Before Fix
- "Show Deleted" checkbox didn't filter the display properly
- Deleted categories were not showing when checkbox was selected

### After Fix
- Unchecked: Shows only active categories
- Checked: Shows only deleted categories
- Restore button appears only for deleted categories
- Edit/Delete buttons appear only for active categories
- Seamless recovery of deleted categories

## Technical Details

### Query Filter Bypass
```csharp
// Instead of:
var categories = await _context.Categories.ToListAsync(); // Applies filter

// We use:
var categories = await _context.Categories
    .IgnoreQueryFilters()  // Bypasses the global filter
    .ToListAsync();        // Gets all including deleted
```

### Client-Side Filtering
The frontend also filters the response to ensure proper display:
```typescript
if (!this.includeDeleted) {
  categories = categories.filter(c => !c.isDeleted);
} else {
  categories = categories.filter(c => c.isDeleted);
}
```

## Testing

To test the functionality:
1. Create several categories
2. Delete some categories
3. Without "Show Deleted": Should see only active categories
4. With "Show Deleted": Should see only deleted categories
5. Click Restore on a deleted category
6. Category should reappear in the active list
