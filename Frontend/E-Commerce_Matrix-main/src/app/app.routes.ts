import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegistrationComponent } from './auth/registration/registration.component';
import { HomepageComponent } from './home/homepage.component';
import { UserProductsComponent } from './components/user-products/user-products.component';
import { AdminProductsComponent } from './components/admin-products/admin-products.component';
import { AdminCategoriesComponent } from './components/admin-categories/admin-categories.component';
import { BulkImportProductsComponent } from './components/bulk-import-products/bulk-import-products.component';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegistrationComponent },
  { path: 'home', component: HomepageComponent, canActivate: [AuthGuard] },
  { path: 'products', component: UserProductsComponent, canActivate: [AuthGuard] },
  { path: 'admin/products', component: AdminProductsComponent, canActivate: [AuthGuard] },
  { path: 'admin/categories', component: AdminCategoriesComponent, canActivate: [AuthGuard] },
  { path: 'admin/bulk-import', component: BulkImportProductsComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '/home' },
];
  