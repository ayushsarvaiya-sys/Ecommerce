# Manage Categories - Status & Restore Feature

## Overview
Added status functionality (Active/Deleted) and restore capability to the Category Management feature, matching the "Manage Products" implementation.

## Changes Made

### 1. Category Interface Update
**File**: `src/app/services/product.service.ts`

Updated the `Category` interface to include status fields:
```typescript
export interface Category {
  id: number;
  name: string;
  description?: string;
  isDeleted?: boolean;
  createdDate?: string;
  updatedDate?: string;
}
```

### 2. Product Service - New Methods
**File**: `src/app/services/product.service.ts`

Added category API methods:
- `addCategory()` - Create new category
- `updateCategoryName()` - Update category name
- `deleteCategory()` - Delete category (soft delete)
- `restoreCategory()` - Restore deleted category
- `getCategoryById()` - Get single category

### 3. Admin Categories Component - TypeScript
**File**: `src/app/components/admin-categories/admin-categories.component.ts`

#### New Properties:
- `includeDeleted` - Filter flag to show/hide deleted categories
- Added `applyFilters()` method - Apply filter changes
- Added `resetFilters()` method - Reset all filters
- Added `restoreCategory()` method - Restore deleted categories

### 4. Admin Categories Component - Template
**File**: `src/app/components/admin-categories/admin-categories.component.html`

#### UI Enhancements:
1. **Search Bar Update**:
   - Added "Show Deleted" checkbox filter
   - Allows toggling display of deleted categories

2. **Table Update**:
   - Added "Status" column
   - Status badges showing "Active" or "Deleted"
   - Conditional action buttons:
     - Active categories: Edit & Delete buttons
     - Deleted categories: Restore button only

3. **Styling Classes**:
   - `deleted-row` - Applied to deleted category rows
   - Status badges with color-coded styling

### 5. Admin Categories Component - Styling
**File**: `src/app/components/admin-categories/admin-categories.component.css`

#### New Styles:
1. **Status Badges**:
   ```css
   .status-badge - Base style
   .status-active - Green badge for active categories
   .status-deleted - Red badge for deleted categories
   ```

2. **Restore Button**:
   ```css
   .btn-restore - Teal button for restore action
   ```

3. **Deleted Row**:
   ```css
   .deleted-row - Light gray background with reduced opacity
   ```

4. **Checkbox Filter**:
   ```css
   .include-deleted-label - Styled checkbox with hover effects
   ```

5. **Responsive Updates**:
   - Mobile-friendly layout adjustments
   - Flexible button sizing
   - Proper spacing for small screens

## Features

### Status Display
- **Active Categories**: 
  - Green "Active" badge
  - Normal row styling
  - Edit and Delete buttons available

- **Deleted Categories**:
  - Red "Deleted" badge
  - Grayed-out row styling
  - Only Restore button available
  - Cannot be edited when deleted

### Filter Functionality
- **Show Deleted Toggle**:
  - Checkbox in search bar
  - Real-time filter application
  - Persists during browsing

### Action Buttons
- **Edit**: Only for active categories
- **Delete**: Only for active categories
- **Restore**: Only for deleted categories

## User Workflow

### Deleting a Category
1. Admin navigates to Manage Categories
2. Finds the active category
3. Clicks "Delete" button
4. Confirms deletion in modal
5. Category status changes to "Deleted"
6. Category appears grayed out in table

### Restoring a Category
1. Admin enables "Show Deleted" checkbox
2. Deleted categories become visible
3. Clicks "Restore" button on deleted category
4. Category status changes back to "Active"
5. Category returns to normal styling

## Backend API Integration
The component expects the following backend endpoints:
- `POST /api/Category/Add` - Create category
- `GET /api/Category/GetAll` - Get all categories
- `GET /api/Category/GetById/{id}` - Get single category
- `PUT /api/Category/ChangeName` - Update category name
- `DELETE /api/Category/Delete/{id}` - Soft delete category
- `POST /api/Category/Restore/{id}` - Restore deleted category

**Note**: The backend should support soft deletes with an `isDeleted` flag.

## UI/UX Improvements
- Consistent styling with Manage Products feature
- Clear visual distinction between active and deleted items
- Intuitive filter mechanism
- Confirmation dialogs for destructive actions
- Auto-dismissing success/error messages
- Responsive design for all screen sizes

## Color Scheme
- **Active Badge**: Green (#d4edda text #155724)
- **Deleted Badge**: Red (#f8d7da text #721c24)
- **Restore Button**: Teal (#20c997)
- **Deleted Row Background**: Light Gray (#f8f9fa)

## Testing Checklist
- [ ] Active categories display with "Active" badge
- [ ] Deleted categories display with "Deleted" badge
- [ ] "Show Deleted" checkbox filters categories correctly
- [ ] Delete button changes category status to deleted
- [ ] Restore button changes category status to active
- [ ] Edit/Delete buttons hidden for deleted categories
- [ ] Restore button only visible for deleted categories
- [ ] UI responsive on mobile devices
- [ ] Success messages appear and auto-dismiss
- [ ] Error messages display with proper feedback
