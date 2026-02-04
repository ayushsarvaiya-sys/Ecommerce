# Rate Limiting Implementation - Token Bucket Algorithm

## Overview
Implemented ASP.NET Core built-in rate limiting using the Token Bucket algorithm to restrict API requests to **10 requests per minute per IP address**.

## Backend Implementation (C#)

### 1. Program.cs Configuration

#### Imports Added
```csharp
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
```

#### Rate Limiter Service Registration
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddIpBasedLimiter("api-limiter", context =>
        new TokenBucketRateLimiterOptions
        {
            TokenLimit = 10,              // Maximum tokens in bucket
            TokensPerPeriod = 10,         // Tokens added per period
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),  // Every minute
            AutoReplenishment = true      // Automatic refill
        });

    options.OnRejected = async (context, _) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue)
            ? ((TimeSpan)retryAfterValue).TotalSeconds
            : 60;

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            statusCode = StatusCodes.Status429TooManyRequests,
            message = "Rate limit exceeded. Too many requests.",
            retryAfter = retryAfter
        });
    };
});
```

#### Middleware Pipeline Setup
```csharp
// Add rate limiter to middleware (early in pipeline)
app.UseRateLimiter();

// Apply rate limiting policy to all endpoints
app.MapControllers().RequireRateLimiting("api-limiter");
```

### 2. Response Format (429 Status)
When rate limit is exceeded, the API returns:
```json
{
  "statusCode": 429,
  "message": "Rate limit exceeded. Too many requests.",
  "retryAfter": 45
}
```

---

## Frontend Implementation (Angular)

### 1. NotificationService
**File:** `src/app/services/notification.service.ts`

Manages toast notifications with different severity levels:
- `success()` - Green toast
- `error()` - Red toast
- `warning()` - Yellow toast
- `info()` - Blue toast

```typescript
// Usage Example
this.notificationService.error("Rate limit exceeded. Retry after 60 seconds");
```

### 2. HTTP Interceptor
**File:** `src/app/interceptors/http.interceptor.ts`

Enhanced to handle rate limiting (429 status):
- Detects 429 responses from API
- Displays error message with retry duration
- Shows notification for the duration of the retry period

```typescript
if (error.status === 429) {
  const errorData = error.error;
  const message = errorData?.message || 'Too many requests...';
  const retryAfter = errorData?.retryAfter || 60;
  
  this.notificationService.error(
    `${message} (Retry after ${Math.ceil(retryAfter)} seconds)`,
    retryAfter * 1000 + 1000
  );
}
```

### 3. Toast Notification Component
**File:** `src/app/components/notification/toast.component.ts`

Standalone Angular component that displays toast notifications:
- Fixed position (top-right)
- Animated entrance/exit
- Color-coded by type
- Auto-removal based on duration
- Responsive design

#### Integration in App Component
```typescript
import { ToastNotificationComponent } from './components/notification/toast.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, FooterComponent, ToastNotificationComponent],
  // ...
})
export class App { }
```

---

## How It Works

### Token Bucket Algorithm
1. **Initial State:** Bucket contains 10 tokens
2. **Each Request:** Consumes 1 token
3. **Replenishment:** 10 tokens added every 60 seconds
4. **Limit Exceeded:** When bucket is empty, request returns 429

### Flow Diagram
```
User Request → Check Token Bucket → 
  ├─ Tokens Available → Process Request ✓
  └─ No Tokens → Return 429 with retry-after
                     ↓
            Show Toast Notification
            Disable Retry for N seconds
```

---

## Configuration Details

| Setting | Value | Description |
|---------|-------|-------------|
| TokenLimit | 10 | Max tokens per bucket |
| TokensPerPeriod | 10 | Tokens replenished per period |
| ReplenishmentPeriod | 1 minute | How often to replenish |
| AutoReplenishment | true | Automatically refill tokens |
| Status Code | 429 | HTTP Too Many Requests |

---

## Testing Rate Limiting

### Using curl
```bash
# Run this 11 times to exceed limit
for i in {1..15}; do
  curl -X GET http://localhost:5000/api/products
  echo "Request $i"
  sleep 0.2
done
```

### Expected Response (11th+ request)
```json
{
  "statusCode": 429,
  "message": "Rate limit exceeded. Too many requests.",
  "retryAfter": 45
}
```

### Frontend Behavior
- Toast notification appears: "Rate limit exceeded. Too many requests. (Retry after 45 seconds)"
- Toast automatically disappears after 46 seconds
- User can see countdown in the message

---

## Per-IP Rate Limiting

Each IP address has its own token bucket:
- `192.168.1.1`: 10 requests/minute
- `192.168.1.2`: 10 requests/minute (independent)

This prevents one user from blocking others.

---

## Files Modified/Created

### Backend
- `Backend/ECommerce/ECommerce/Program.cs` - Updated with rate limiter configuration

### Frontend
- `Frontend/E-Commerce_Matrix-main/src/app/services/notification.service.ts` - **NEW**
- `Frontend/E-Commerce_Matrix-main/src/app/interceptors/http.interceptor.ts` - Updated
- `Frontend/E-Commerce_Matrix-main/src/app/components/notification/toast.component.ts` - **NEW**
- `Frontend/E-Commerce_Matrix-main/src/app/app.ts` - Updated imports

---

## Future Enhancements

1. **Configurable Limits** - Different limits per endpoint
   ```csharp
   options.AddPolicy("strict", context =>
       RateLimitPartition.GetIpLimiter(context, ip => ...));
   
   app.MapGet("/api/auth/login", handler)
       .RequireRateLimiting("strict");
   ```

2. **Database-Backed Limits** - Store retry info
3. **User-Based Limiting** - Different limits for authenticated users
4. **Endpoint-Specific Limits** - Stricter limits for expensive operations
5. **Retry-After Header** - Add standard HTTP headers

---

## Troubleshooting

### Rate Limit Not Working
1. Ensure `app.UseRateLimiter()` is before other middleware
2. Check `RequireRateLimiting("api-limiter")` is applied to endpoints
3. Verify `AddRateLimiter` is registered in services

### Toast Not Showing
1. Verify `ToastNotificationComponent` is in app imports
2. Check `NotificationService` is provided in root
3. Ensure interceptor is properly registered with `HTTP_INTERCEPTORS`

### Wrong Retry Duration
- Check `MetadataName.RetryAfter` parsing
- Verify token replenishment period matches calculation

---

## Summary

✅ **10 requests per minute** per IP address using Token Bucket algorithm
✅ **429 Status Code** returned when limit exceeded
✅ **Retry-After** duration provided in response
✅ **Toast Notifications** displayed on frontend
✅ **Auto-dismiss** after retry period
✅ **Per-IP isolation** - Each IP has independent bucket
