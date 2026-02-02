import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

export interface CartItem {
  id: number;
  cartId: number;
  productId: number;
  quantity: number;
  priceAtAddTime: number;
  productName?: string;
  productImageUrl?: string;
  totalPrice?: number;
}

export interface Cart {
  id: number;
  userId: number;
  cartItems: CartItem[];
  totalPrice: number;
  totalItems: number;
  createdAt: string;
  updatedAt: string;
}

export interface AddToCartRequest {
  productId: number;
  quantity: number;
}

export interface UpdateCartItemRequest {
  cartItemId: number;
  quantity: number;
}

export interface ApiResponse<T> {
  statusCode: number;
  data: T;
  message: string;
}

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private apiUrl = 'https://localhost:7067/api/Cart';
  
  // Create empty cart as default
  private readonly emptyCart: Cart = {
    id: 0,
    userId: 0,
    cartItems: [],
    totalPrice: 0,
    totalItems: 0,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  };

  // BehaviorSubject to manage cart state
  private cartSubject = new BehaviorSubject<Cart>(this.emptyCart);
  public cart$ = this.cartSubject.asObservable();

  // BehaviorSubject to manage cart item count
  private cartItemCountSubject = new BehaviorSubject<number>(0);
  public cartItemCount$ = this.cartItemCountSubject.asObservable();

  // BehaviorSubject to manage loading state
  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  constructor(private http: HttpClient) {
    this.loadCart();
  }

  /**
   * Load cart from server
   */
  public loadCart(): void {
    this.setLoading(true);
    this.http
      .get<ApiResponse<Cart>>(`${this.apiUrl}/GetCart`)
      .pipe(
        tap((response) => {
          this.cartSubject.next(response.data);
          this.updateCartItemCount(response.data);
          this.setLoading(false);
        }),
        catchError((error) => {
          console.error('Error loading cart:', error);
          this.setLoading(false);
          return throwError(() => error);
        })
      )
      .subscribe();
  }

  /**
   * Add product to cart
   */
  public addToCart(productId: number, quantity: number): Observable<ApiResponse<Cart>> {
    this.setLoading(true);
    const request: AddToCartRequest = { productId, quantity };

    return this.http.post<ApiResponse<Cart>>(`${this.apiUrl}/AddToCart`, request).pipe(
      tap((response) => {
        this.cartSubject.next(response.data);
        this.updateCartItemCount(response.data);
        this.setLoading(false);
      }),
      catchError((error) => {
        console.error('Error adding to cart:', error);
        this.setLoading(false);
        return throwError(() => error);
      })
    );
  }

  /**
   * Update cart item quantity
   */
  public updateCartItem(cartItemId: number, quantity: number): Observable<ApiResponse<Cart>> {
    this.setLoading(true);
    const request: UpdateCartItemRequest = { cartItemId, quantity };

    return this.http.put<ApiResponse<Cart>>(`${this.apiUrl}/UpdateCartItem`, request).pipe(
      tap((response) => {
        this.cartSubject.next(response.data);
        this.updateCartItemCount(response.data);
        this.setLoading(false);
      }),
      catchError((error) => {
        console.error('Error updating cart item:', error);
        this.setLoading(false);
        return throwError(() => error);
      })
    );
  }

  /**
   * Remove cart item
   */
  public removeCartItem(cartItemId: number): Observable<ApiResponse<Cart>> {
    this.setLoading(true);
    return this.http.delete<ApiResponse<Cart>>(`${this.apiUrl}/RemoveCartItem/${cartItemId}`).pipe(
      tap((response) => {
        this.cartSubject.next(response.data);
        this.updateCartItemCount(response.data);
        this.setLoading(false);
      }),
      catchError((error) => {
        console.error('Error removing cart item:', error);
        this.setLoading(false);
        return throwError(() => error);
      })
    );
  }

  /**
   * Clear entire cart
   */
  public clearCart(): Observable<ApiResponse<Cart>> {
    this.setLoading(true);
    return this.http.delete<ApiResponse<Cart>>(`${this.apiUrl}/ClearCart`).pipe(
      tap((response) => {
        this.cartSubject.next(response.data);
        this.updateCartItemCount(response.data);
        this.setLoading(false);
      }),
      catchError((error) => {
        console.error('Error clearing cart:', error);
        this.setLoading(false);
        return throwError(() => error);
      })
    );
  }

  /**
   * Get current cart value
   */
  public getCurrentCart(): Cart {
    return this.cartSubject.value;
  }

  /**
   * Get current cart item count
   */
  public getCurrentCartItemCount(): number {
    return this.cartItemCountSubject.value;
  }

  /**
   * Update cart item count based on cart data
   */
  private updateCartItemCount(cart: Cart): void {
    this.cartItemCountSubject.next(cart.totalItems);
  }

  /**
   * Set loading state
   */
  private setLoading(loading: boolean): void {
    this.loadingSubject.next(loading);
  }
}
