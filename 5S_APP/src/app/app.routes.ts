import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { HomeComponent } from './tabs/home/home.component';
import { ReportComponent } from './tabs/report/report.component';
import { NewsComponent } from './tabs/news/news.component';
import { NotificationsComponent } from './tabs/notifications/notifications.component';
import { AccountComponent } from './tabs/account/account.component';
import { homeRouter } from './tabs/home/home.routes';
import { surveyRoutes } from './tabs/survey/survey.routes';
import { accountRoutes } from './tabs/account/account.routes';
import { LoginComponent } from './auth/login/login.component';
import AuthGuard from './auth/guards/auth.guard';
import { reportRoutes } from './tabs/report/report.routes';

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
        path: 'home',
        loadChildren: () =>
          import('./tabs/home/home.routes').then((m) => m.homeRouter),
        canActivate: [AuthGuard],
      },
      {
        path: 'report',
        loadChildren: () =>
          import('./tabs/report/report.routes').then((m) => m.reportRoutes),
        canActivate: [AuthGuard],
      },
      // { path: 'report', component: ReportComponent, canActivate: [AuthGuard] },
      { path: 'news', component: NewsComponent, canActivate: [AuthGuard] },
      {
        path: 'notifications',
        loadComponent: () =>
          import('./tabs/notifications/notifications.component').then(
            (m) => m.NotificationsComponent
          ),
        canActivate: [AuthGuard],
      },
      // {
      //   path: 'account',
      //   loadComponent: ()=>import('./tabs/account/account.component').then((m)=>m.AccountComponent),
      //   canActivate: [AuthGuard],
      // },
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
