# Cloudinary Setup Instructions

## Important: Create Upload Preset in Cloudinary Dashboard

To use unsigned uploads with Cloudinary, you need to create an upload preset. Follow these steps:

### Step 1: Login to Cloudinary Dashboard
1. Go to [https://cloudinary.com/console/](https://cloudinary.com/console/)
2. Login with your credentials (cloud name: `dtm30xegx`)

### Step 2: Create Upload Preset
1. In the left sidebar, click **Settings** (gear icon)
2. Go to **Upload** tab
3. Scroll down to **Upload presets** section
4. Click **"Add upload preset"** button
5. Configure the preset:
   - **Name**: `ecommerce_unsigned`
   - **Unsigned**: Toggle **ON** (this allows client-side uploads without signing)
   - **Folder**: `ecommerce/products` (optional, for organization)
   - Click **Save**

### Step 3: Verify Configuration
Your backend `appsettings.json` already has:
```json
"Cloudinary": {
  "CloudName": "dtm30xegx",
  "ApiKey": "766836563878781",
  "ApiSecret": "p-QsWIaJr36HlQk1wa9YegKJfqw",
  "UploadPreset": "ecommerce_unsigned"
}
```

Make sure the `UploadPreset` value matches the name you created.

### Step 4: How It Works Now

1. **Frontend** → Requests Cloudinary config from backend
2. **Backend** → Returns cloud name and upload preset
3. **Frontend** → Uploads image directly to Cloudinary using the preset
4. **Cloudinary** → Validates and accepts the image (CORS-enabled)
5. **Frontend** → Receives `secure_url` and sends it with product data to backend

### Benefits of This Approach
✅ No CORS issues (Cloudinary allows unsigned uploads)
✅ No need for signed URLs or complex signature generation
✅ Direct browser-to-Cloudinary upload (faster)
✅ Simpler code and fewer API calls
✅ No sensitive keys exposed to frontend

---

## If You Get CORS Errors

If you still see CORS errors after setup:
1. Go back to your upload preset settings
2. Ensure the preset is set to **Unsigned: ON**
3. Verify the preset name matches `ecommerce_unsigned` in appsettings.json
4. Clear browser cache and restart the dev server

## Testing the Upload

1. Start the backend server
2. Start the frontend (Angular)
3. Login as Admin
4. Go to "Manage Products" → "+ Add New Product"
5. Select an image and click "⬆ Upload Image"
6. The image should upload directly to Cloudinary and display the URL
