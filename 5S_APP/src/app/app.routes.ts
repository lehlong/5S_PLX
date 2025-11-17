import { Routes } from '@angular/router';
import AuthGuard from './auth/guards/auth.guard';
import { StartupGuard } from './auth/guards/startup.guard';
import { LoginComponent } from './auth/login/login.component';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { NewsV2Component } from './tabs/news-v2/news-v2.component';
import { NewsComponent } from './tabs/news/news.component';

export const routes: Routes = [
  {
    path: '',
    canActivate: [StartupGuard],
    children: [],
  },

  { path: 'news-v2', component: NewsV2Component, canActivate: [StartupGuard] },
  {
    path: 'news/:id',
    loadComponent: () =>
      import('./tabs/news-detail/news-detail.component').then(
        (m) => m.NewsDetailComponent
      ),
  },
  // { path: '', component: LoginComponent },
  { path: 'login', component: LoginComponent },

  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: 'home', component: NewsComponent, canActivate: [AuthGuard] },
      {
        path: 'account',
        loadChildren: () =>
          import('./tabs/account/account.routes').then((m) => m.accountRoutes),
        canActivate: [AuthGuard],
      },
      {
        path: 'report',
        loadChildren: () =>
          import('./tabs/report/report.routes').then((m) => m.reportRoutes),
        canActivate: [AuthGuard],
      },
      {
        path: 'notifications',
        loadComponent: () =>
          import('./tabs/notifications/notifications.component').then(
            (m) => m.NotificationsComponent
          ),
        canActivate: [AuthGuard],
      },
      {
        path: 'survey',
        loadChildren: () =>
          import('./tabs/survey/survey.routes').then((m) => m.surveyRoutes),
        canActivate: [AuthGuard],
      },
    ],
  },
];
