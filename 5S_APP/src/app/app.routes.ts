import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { NewsComponent } from './tabs/news/news.component';
import { LoginComponent } from './auth/login/login.component';
import AuthGuard from './auth/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      {
        path: 'account',
        loadChildren: () =>
          import('./tabs/account/account.routes').then((m) => m.accountRoutes),
      },
      {
        path: 'report',
        loadChildren: () =>
          import('./tabs/report/report.routes').then((m) => m.reportRoutes),
        canActivate: [AuthGuard],
      },
      { path: 'home', component: NewsComponent, canActivate: [AuthGuard] },
      {
        path: 'notifications',
        loadComponent: () =>
          import('./tabs/notifications/notifications.component').then(
            (m) => m.NotificationsComponent
          ),
        canActivate: [AuthGuard],
      },
    ],
  },
  {
    path: 'survey',
    loadChildren: () =>
      import('./tabs/survey/survey.routes').then((m) => m.surveyRoutes),
    canActivate: [AuthGuard],
  },
  {
    path: 'login',
    component: LoginComponent,
  },
];
