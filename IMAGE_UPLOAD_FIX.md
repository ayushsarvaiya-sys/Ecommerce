# Image Upload Fix - Unsigned Cloudinary Upload

## Problem Fixed
- ❌ Presigned URL approach had CORS issues
- ❌ 401 Unauthorized errors on Cloudinary
- ❌ "Upload preset must be specified" error

## Solution Implemented
✅ **Switched to Unsigned Upload Preset** (Cloudinary's recommended approach)

This method is:
- **Simpler**: No complex signature generation
- **Faster**: Direct browser-to-Cloudinary upload
- **CORS-compliant**: Cloudinary handles CORS for unsigned uploads
- **More Secure**: Backend only returns preset name, not API keys

## Changes Made

### Backend Changes

1. **CloudinarySettings.cs** - Added `UploadPreset` property
2. **CloudinaryService.cs** - Simplified to just return cloud name and preset
3. **CloudinaryController.cs** - Changed endpoint from `GetPresignedUrl` to `GetUploadConfig`
4. **appsettings.json** - Added `"UploadPreset": "ecommerce_unsigned"`

### Frontend Changes

1. **product.service.ts**:
   - Added `CloudinaryConfig` interface
   - Changed `getPresignedUrl()` → `getCloudinaryConfig()`
   - Simplified `uploadImageToCloudinary()` to use upload preset

2. **admin-products.component.ts**:
   - Updated `uploadImageAndAddProduct()` to use new config-based approach
   - Updated `uploadImageAndUpdateProduct()` similarly
   - Removed presigned URL logic

## What You Need to Do

⚠️ **IMPORTANT**: Create upload preset in Cloudinary Dashboard:

1. Go to https://cloudinary.com/console/settings/upload
2. Click "Add upload preset"
3. Set:
   - Name: `ecommerce_unsigned`
   - Unsigned: **ON**
   - Folder: `ecommerce/products` (optional)
4. Save

See [CLOUDINARY_SETUP.md](./CLOUDINARY_SETUP.md) for detailed instructions.

## How It Works Now

```
User selects image
    ↓
Frontend requests Cloudinary config from backend
    ↓
Backend returns cloud name and preset name
    ↓
Frontend uploads directly to Cloudinary using preset
    ↓
Cloudinary accepts and returns image URL
    ↓
Frontend sends product with image URL to backend
    ↓
Product saved with image
```

## Testing

1. Make sure the upload preset `ecommerce_unsigned` is created in Cloudinary
2. Restart backend server
3. Login as Admin
4. Go to "Manage Products" → "+ Add New Product"
5. Select and upload an image - it should work now! ✅
