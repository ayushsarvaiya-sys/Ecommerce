# Rate Limiting - Quick Reference & Testing Guide

## What Was Implemented
- ✅ Token Bucket Algorithm for rate limiting
- ✅ 10 requests per minute per IP address
- ✅ Built-in ASP.NET Core rate limiting (no custom middleware needed)
- ✅ 429 (Too Many Requests) status code response
- ✅ Toast notifications on frontend when limit exceeded
- ✅ Retry-after information displayed to user

---

## Backend Files Modified

### Program.cs
**Location:** `Backend/ECommerce/ECommerce/Program.cs`

**Key Changes:**
1. Added imports:
   - `using Microsoft.AspNetCore.RateLimiting;`
   - `using System.Threading.RateLimiting;`

2. Registered rate limiter service (lines 177-202)
3. Added middleware (line 217)
4. Applied policy to all endpoints (line 241)

**Configuration Details:**
```csharp
TokenLimit = 10              // Max 10 tokens
TokensPerPeriod = 10         // Replenish 10 tokens
ReplenishmentPeriod = TimeSpan.FromMinutes(1)  // Every 60 seconds
AutoReplenishment = true     // Auto-refill enabled
```

---

## Frontend Files Modified/Created

### 1. NotificationService
**File:** `src/app/services/notification.service.ts` (NEW)
**Purpose:** Centralized toast notification management
**Methods:**
- `success(message, duration?)`
- `error(message, duration?)`
- `warning(message, duration?)`
- `info(message, duration?)`

### 2. HTTP Interceptor
**File:** `src/app/interceptors/http.interceptor.ts` (UPDATED)
**What's New:**
- Detects 429 status responses
- Extracts retry-after duration from response
- Calls notification service to show error toast
- Toast displays: "Too many requests. (Retry after X seconds)"

### 3. Toast Component
**File:** `src/app/components/notification/toast.component.ts` (NEW)
**Features:**
- Displays toast notifications in top-right corner
- Animated entrance (slide-in from right)
- Color-coded: success (green), error (red), warning (yellow), info (blue)
- Auto-removes after specified duration
- Responsive on mobile devices

### 4. App Component
**File:** `src/app/app.ts` (UPDATED)
**Change:** Added `ToastNotificationComponent` to imports

---

## Testing the Rate Limiting

### Test 1: Using Postman
1. Open Postman
2. Create a GET request to: `http://localhost:5000/api/products`
3. Click "Send" 11+ times in quick succession
4. **Expected:** 11th request returns 429 status with error JSON

### Test 2: Using curl (Windows PowerShell)
```powershell
# Send 15 requests to test rate limit
for ($i = 1; $i -le 15; $i++) {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/products" -Method Get -ErrorAction SilentlyContinue
    Write-Host "Request $i : $($response.StatusCode)" 
    Start-Sleep -Milliseconds 100
}
```

### Test 3: Using curl (Linux/Mac/WSL)
```bash
# Send 15 requests
for i in {1..15}; do
  curl -X GET http://localhost:5000/api/products -w "\nStatus: %{http_code}\n"
  sleep 0.1
done
```

### Expected Responses

**Requests 1-10:** ✅ 200 OK
```json
{
  "statusCode": 200,
  "data": [...products...]
}
```

**Requests 11+:** ❌ 429 Too Many Requests
```json
{
  "statusCode": 429,
  "message": "Rate limit exceeded. Too many requests.",
  "retryAfter": 45
}
```

### Frontend Test
1. Start the Angular frontend: `npm start`
2. Open browser DevTools (F12)
3. Switch to Network tab
4. Click on a button that makes API calls rapidly (e.g., "Load Products" multiple times)
5. **Expected:** After 10 requests, you'll see a red toast notification at top-right:
   ```
   "Rate limit exceeded. Too many requests. (Retry after 45 seconds)"
   ```

---

## How to Test Rate Limiting in Real Scenario

### Scenario: Rapid API Calls
1. Open the Angular app in browser
2. Go to Products page
3. Rapidly click "Load More" or refresh button 11+ times
4. **Result:** 11th+ clicks will show rate limit toast

### Scenario: Check Toast UI
1. Artificially trigger 429 by rapid API calls
2. Observe toast notification:
   - Position: Top-right corner
   - Color: Red background
   - Text: "Rate limit exceeded..." with retry duration
   - Auto-disappears after retry period (+ 1 second)

---

## Response Format

### When Rate Limit is Exceeded (429)
```json
{
  "statusCode": 429,
  "message": "Rate limit exceeded. Too many requests.",
  "retryAfter": 45
}
```

**Fields:**
- `statusCode`: HTTP 429 status
- `message`: User-friendly error message
- `retryAfter`: Seconds to wait before retrying

---

## How Token Bucket Works

### Timeline Example:
```
Minute 1 (0-60s):
├─ 0s:  Start with 10 tokens
├─ 0s:  Request #1 → 9 tokens left
├─ 0.1s: Request #2 → 8 tokens left
├─ ...
├─ 0.9s: Request #10 → 0 tokens left
├─ 1.0s: Request #11 → BLOCKED (429 Too Many Requests)
│
└─ 60s: Bucket refilled to 10 tokens ✅
         Request #11 now succeeds
```

### Per-IP Isolation:
```
IP 192.168.1.1: [10 tokens] ← Independent bucket
IP 192.168.1.2: [10 tokens] ← Independent bucket
IP 192.168.1.3: [10 tokens] ← Independent bucket
```

---

## Verification Checklist

- [ ] Backend builds without errors
- [ ] `app.UseRateLimiter()` is in middleware pipeline
- [ ] `RequireRateLimiting("api-limiter")` applied to endpoints
- [ ] Frontend imports `ToastNotificationComponent`
- [ ] NotificationService is provided in root
- [ ] HTTP Interceptor is registered
- [ ] Making 11+ rapid requests returns 429 on 11th+
- [ ] Toast notification appears when rate limit hit
- [ ] Toast disappears after retry period
- [ ] Different IPs have independent buckets

---

## Monitoring & Debugging

### Enable Logging (Program.cs)
```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

### Check Rate Limiter Status
- Monitor HTTP 429 responses in browser DevTools
- Check Network tab for rate limit responses
- Observe Console for toast notifications

### Performance Impact
- Minimal overhead per request
- In-memory token bucket (very fast)
- No database calls
- Scales to thousands of concurrent users

---

## Disabling Rate Limiting (If Needed)

### Option 1: Remove from specific endpoint
```csharp
app.MapGet("/api/health", handler)
    .WithoutRateLimiting();  // Skip rate limiting for health check
```

### Option 2: Disable globally
```csharp
// Comment out in Program.cs:
// app.UseRateLimiter();
// app.MapControllers().RequireRateLimiting("api-limiter");
```

### Option 3: Adjust limits
```csharp
TokenLimit = 100,           // Increase from 10
TokensPerPeriod = 100,
ReplenishmentPeriod = TimeSpan.FromSeconds(30),  // Faster refill
```

---

## Common Issues & Solutions

| Issue | Cause | Solution |
|-------|-------|----------|
| Rate limit not working | Middleware not ordered correctly | Ensure `UseRateLimiter()` is near top |
| Toast not showing | Interceptor not registered | Check `HTTP_INTERCEPTORS` in app.config |
| Same IP still blocked after 60s | Bucket not refilling | Verify `AutoReplenishment = true` |
| Different users sharing limit | Wrong limiter type | Ensure `AddIpBasedLimiter` is used |
| Wrong retry duration shown | Metadata parsing error | Check `MetadataName.RetryAfter` handling |

---

## Production Considerations

1. **Load Balancing:** Each server instance has its own in-memory bucket
   - Solution: Use distributed rate limiting (Redis-based) for load-balanced deployments

2. **Proxy/WAF:** X-Forwarded-For header handling
   ```csharp
   context.Request.HttpContext.Connection.RemoteIpAddress
   ```

3. **User-Based Limits:** Different rates for authenticated vs. anonymous
4. **Endpoint-Specific Limits:** Stricter limits for expensive operations
5. **Analytics:** Track rate limit hits per IP/user

---

## Next Steps

1. ✅ Test with multiple concurrent users
2. ✅ Monitor 429 response rates in production
3. ✅ Adjust token limits based on usage patterns
4. ✅ Consider user-based rate limiting for premium features
5. ✅ Implement Redis-based limiting for distributed systems

---

**Implementation Complete!** 🎉
