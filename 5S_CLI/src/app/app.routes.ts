import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { BlankLayoutComponent } from './layouts/blank-layout/blank-layout.component';
import AuthGuard from './guards/auth.guard';
import { HomeComponent } from './home/home.component';
import UnauthGuard from './guards/unauth.guard';
import { LoginComponent } from './auth/login/login.component';
import { NotFoundComponent } from './layouts/not-found/not-found.component';
import { systemManagerRoutes } from './@system-manager/system-manager.routes';
import { masterDataRoutes } from './@master-data/account-type/master-data.routes';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {path: '', redirectTo: 'home', pathMatch: 'full'},
      {path: 'home', component: HomeComponent, canActivate: [AuthGuard]},
      {path: 'system-manager', children: systemManagerRoutes, canActivate: [AuthGuard]},
      {path: 'master-data', children: masterDataRoutes, canActivate: [AuthGuard]},
    ],
  },
  {
    path: '',
    component: BlankLayoutComponent,
    children: [
      {path: 'login', component: LoginComponent, canActivate: [UnauthGuard]},
    ],
  },
  { path: '**', component: NotFoundComponent },
];

