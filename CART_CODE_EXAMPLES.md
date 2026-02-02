# Add to Cart - Code Examples & Reference

## Backend API Examples

### 1. Add Item to Cart
**Request:**
```http
POST /api/Cart/AddToCart HTTP/1.1
Host: localhost:7067
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "productId": 1,
  "quantity": 2
}
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "data": {
    "id": 1,
    "userId": 1,
    "cartItems": [
      {
        "id": 1,
        "cartId": 1,
        "productId": 1,
        "quantity": 2,
        "priceAtAddTime": 999.99,
        "productName": "Product Name",
        "productImageUrl": "https://...",
        "totalPrice": 1999.98
      }
    ],
    "totalPrice": 1999.98,
    "totalItems": 2,
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:35:00Z"
  },
  "message": "Product added to cart successfully"
}
```

### 2. Get Cart
**Request:**
```http
GET /api/Cart/GetCart HTTP/1.1
Host: localhost:7067
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "data": {
    "id": 1,
    "userId": 1,
    "cartItems": [...],
    "totalPrice": 5000,
    "totalItems": 5,
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:35:00Z"
  },
  "message": "Cart retrieved successfully"
}
```

### 3. Update Cart Item Quantity
**Request:**
```http
PUT /api/Cart/UpdateCartItem HTTP/1.1
Host: localhost:7067
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "cartItemId": 1,
  "quantity": 5
}
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "data": {
    "id": 1,
    "userId": 1,
    "cartItems": [
      {
        "id": 1,
        "cartId": 1,
        "productId": 1,
        "quantity": 5,
        "priceAtAddTime": 999.99,
        "productName": "Product Name",
        "productImageUrl": "https://...",
        "totalPrice": 4999.95
      }
    ],
    "totalPrice": 4999.95,
    "totalItems": 5,
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:40:00Z"
  },
  "message": "Cart item updated successfully"
}
```

### 4. Remove Item from Cart
**Request:**
```http
DELETE /api/Cart/RemoveCartItem/1 HTTP/1.1
Host: localhost:7067
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "data": {
    "id": 1,
    "userId": 1,
    "cartItems": [],
    "totalPrice": 0,
    "totalItems": 0,
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:45:00Z"
  },
  "message": "Cart item removed successfully"
}
```

### 5. Clear Entire Cart
**Request:**
```http
DELETE /api/Cart/ClearCart HTTP/1.1
Host: localhost:7067
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "data": {
    "id": 1,
    "userId": 1,
    "cartItems": [],
    "totalPrice": 0,
    "totalItems": 0,
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:46:00Z"
  },
  "message": "Cart cleared successfully"
}
```

### 6. Get Cart Item Count
**Request:**
```http
GET /api/Cart/CartItemCount HTTP/1.1
Host: localhost:7067
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

**Response (200 OK):**
```json
{
  "statusCode": 200,
  "data": 5,
  "message": "Cart item count retrieved successfully"
}
```

---

## Frontend Component Examples

### 1. Using CartService in a Component

```typescript
import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-my-component',
  template: `
    <div>
      <p>Cart Items: {{ (cartService.cartItemCount$ | async) || 0 }}</p>
      <div *ngIf="(cartService.loading$ | async)">Loading...</div>
    </div>
  `
})
export class MyComponent implements OnInit {
  constructor(public cartService: CartService) {}

  ngOnInit() {
    // Load cart on init
    this.cartService.loadCart();
  }

  addItem() {
    this.cartService.addToCart(1, 2).subscribe({
      next: (response) => console.log('Item added:', response),
      error: (error) => console.error('Error:', error)
    });
  }
}
```

### 2. Displaying Cart with Async Pipe

```typescript
// component.ts
import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-cart-summary',
  template: `
    <div class="cart-summary" *ngIf="cart$ | async as cart">
      <h3>Your Cart ({{ cart.totalItems }} items)</h3>
      <p>Total: ₹{{ cart.totalPrice.toFixed(2) }}</p>
      
      <div *ngFor="let item of cart.cartItems">
        <img [src]="item.productImageUrl" alt="product" />
        <p>{{ item.productName }} x {{ item.quantity }}</p>
        <p>₹{{ item.totalPrice.toFixed(2) }}</p>
      </div>
    </div>
  `
})
export class CartSummaryComponent {
  cart$ = this.cartService.cart$;

  constructor(private cartService: CartService) {}
}
```

### 3. Adding Item with Quantity Selector

```typescript
// component.ts
@Component({
  selector: 'app-product-card',
  template: `
    <div class="product">
      <h3>{{ product.name }}</h3>
      <p>₹{{ product.price }}</p>
      
      <div class="quantity-control">
        <button (click)="quantity = quantity - 1" [disabled]="quantity <= 1">
          -
        </button>
        <input [(ngModel)]="quantity" type="number" min="1" />
        <button (click)="quantity = quantity + 1" [disabled]="quantity >= product.stock">
          +
        </button>
      </div>
      
      <button (click)="addToCart()" [disabled]="isAdding">
        {{ isAdding ? 'Adding...' : 'Add to Cart' }}
      </button>
    </div>
  `
})
export class ProductCardComponent {
  @Input() product: any;
  quantity = 1;
  isAdding = false;

  constructor(private cartService: CartService) {}

  addToCart() {
    this.isAdding = true;
    this.cartService.addToCart(this.product.id, this.quantity).subscribe({
      next: () => {
        this.isAdding = false;
        this.quantity = 1;
      },
      error: (error) => {
        this.isAdding = false;
        alert('Error: ' + error.error.message);
      }
    });
  }
}
```

### 4. Update Cart Item Quantity

```typescript
updateQuantity(cartItemId: number, newQuantity: number) {
  if (newQuantity < 1) return;

  this.cartService.updateCartItem(cartItemId, newQuantity).subscribe({
    next: (response) => {
      console.log('Updated cart:', response.data);
    },
    error: (error) => {
      alert('Failed to update: ' + error.error.message);
    }
  });
}
```

### 5. Remove Item from Cart

```typescript
removeFromCart(cartItemId: number) {
  if (!confirm('Remove this item?')) return;

  this.cartService.removeCartItem(cartItemId).subscribe({
    next: (response) => {
      console.log('Item removed');
    },
    error: (error) => {
      alert('Failed to remove: ' + error.error.message);
    }
  });
}
```

### 6. Clear Entire Cart

```typescript
clearCart() {
  if (!confirm('Clear entire cart?')) return;

  this.cartService.clearCart().subscribe({
    next: (response) => {
      console.log('Cart cleared');
    },
    error: (error) => {
      alert('Failed to clear cart: ' + error.error.message);
    }
  });
}
```

---

## Navbar Integration Example

```typescript
// navbar.component.ts
import { Component, OnInit } from '@angular/core';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-navbar',
  template: `
    <nav class="navbar">
      <div class="nav-brand">E-Commerce</div>
      
      <div class="nav-items">
        <a routerLink="/products">Products</a>
        
        <a routerLink="/cart" class="cart-link">
          <i class="fas fa-shopping-cart"></i>
          Cart
          <span class="badge" *ngIf="(cartItemCount$ | async) as count">
            {{ count }}
          </span>
        </a>
        
        <button (click)="logout()">Logout</button>
      </div>
    </nav>
  `,
  styles: [`
    .cart-link {
      position: relative;
    }
    
    .badge {
      position: absolute;
      top: -8px;
      right: -8px;
      background-color: #dc3545;
      color: white;
      border-radius: 50%;
      width: 20px;
      height: 20px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 12px;
      font-weight: bold;
    }
  `]
})
export class NavbarComponent {
  cartItemCount$ = this.cartService.cartItemCount$;

  constructor(private cartService: CartService) {}

  logout() {
    // Logout logic
  }
}
```

---

## Route Configuration Example

```typescript
// app.routes.ts
import { Routes } from '@angular/router';
import { UserProductsComponent } from './components/user-products/user-products.component';
import { ShoppingCartComponent } from './components/shopping-cart/shopping-cart.component';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'products',
    pathMatch: 'full'
  },
  {
    path: 'products',
    component: UserProductsComponent
  },
  {
    path: 'cart',
    component: ShoppingCartComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'checkout',
    component: CheckoutComponent,
    canActivate: [AuthGuard]
  }
];
```

---

## Error Handling Examples

### API Error Response
```json
{
  "statusCode": 400,
  "message": "Not enough stock available. Available: 5"
}
```

### Frontend Error Handler
```typescript
addToCart(productId: number, quantity: number) {
  this.cartService.addToCart(productId, quantity).subscribe({
    next: (response) => {
      this.showSuccess('Item added to cart!');
    },
    error: (error) => {
      const errorMessage = error?.error?.message || 'Failed to add item';
      
      if (error.status === 404) {
        this.showError('Product not found');
      } else if (error.status === 400) {
        this.showError(errorMessage);
      } else if (error.status === 401) {
        this.showError('Please login to continue');
      } else {
        this.showError('Something went wrong');
      }
    }
  });
}

showSuccess(message: string) {
  console.log('✓', message);
  // Show toast or alert
}

showError(message: string) {
  console.error('✗', message);
  // Show error toast or alert
}
```

---

## Database Query Examples

### Get User's Cart
```csharp
var cart = await _context.Carts
  .Include(c => c.CartItems)
  .ThenInclude(ci => ci.Product)
  .Where(c => c.UserId == userId && !c.IsDeleted)
  .FirstOrDefaultAsync();
```

### Get Cart Item Count
```csharp
var itemCount = await _context.CartItems
  .Where(ci => ci.Cart!.UserId == userId && !ci.IsDeleted)
  .SumAsync(ci => ci.Quantity);
```

### Clear Cart
```csharp
var cartItems = await _context.CartItems
  .Where(ci => ci.Cart!.UserId == userId && !ci.IsDeleted)
  .ToListAsync();

foreach (var item in cartItems) {
  item.IsDeleted = true;
}
await _context.SaveChangesAsync();
```

---

## Testing with Postman/Thunder Client

### Postman Collection Variable
```json
{
  "baseUrl": "https://localhost:7067/api",
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

### Environment Setup
```json
{
  "baseUrl": "https://localhost:7067/api",
  "Authorization": "Bearer {{token}}"
}
```

### Test Sequence
1. **Add Item**: POST to `/Cart/AddToCart` with quantity 2
2. **Get Cart**: GET `/Cart/GetCart` to verify
3. **Update Quantity**: PUT `/Cart/UpdateCartItem` with new quantity
4. **Get Count**: GET `/Cart/CartItemCount`
5. **Remove Item**: DELETE `/Cart/RemoveCartItem/1`
6. **Clear Cart**: DELETE `/Cart/ClearCart`

---

## Common Patterns

### Pattern 1: Subscribe and Store in Component
```typescript
cart: Cart | null = null;

ngOnInit() {
  this.cartService.cart$.subscribe(cart => {
    this.cart = cart;
  });
}
```

### Pattern 2: Using takeUntil for Cleanup
```typescript
private destroy$ = new Subject<void>();

ngOnInit() {
  this.cartService.cart$
    .pipe(takeUntil(this.destroy$))
    .subscribe(cart => { /* ... */ });
}

ngOnDestroy() {
  this.destroy$.next();
  this.destroy$.complete();
}
```

### Pattern 3: Async Pipe (Recommended)
```html
<div *ngIf="(cartService.cart$ | async) as cart">
  {{ cart.totalItems }}
</div>
```

---

## Performance Tips

1. **Use Async Pipe** instead of manual subscriptions in templates
2. **Unsubscribe Properly** using takeUntil pattern
3. **Avoid Multiple Subscriptions** to same Observable
4. **Use OnPush Change Detection** for better performance:
   ```typescript
   @Component({
     changeDetection: ChangeDetectionStrategy.OnPush
   })
   ```
5. **Implement Pagination** for large carts

---

This guide covers all the common scenarios and patterns for the add-to-cart functionality!
