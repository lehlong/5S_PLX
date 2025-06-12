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
      { path: 'account', children: accountRoutes},
      { path: 'home', children: homeRouter, canActivate: [AuthGuard] },
      // { path: 'report', component: ReportComponent, canActivate: [AuthGuard] },
      { path: 'news', component: NewsComponent, canActivate: [AuthGuard] },
      { path: 'notifications', component: NotificationsComponent, canActivate: [AuthGuard] },
      { path: 'account', component: AccountComponent, canActivate: [AuthGuard] },
    ],
  },
  {
    path: 'survey', children: surveyRoutes, canActivate: [AuthGuard]
  },
  {
    path:'report', children: reportRoutes, canActivate: [AuthGuard]
  },
  {
    path: 'login',
    component: LoginComponent,
  }
];
