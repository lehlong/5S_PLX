import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { HomeComponent } from './tabs/home/home.component';
import { ReportComponent } from './tabs/report/report.component';
import { NewsComponent } from './tabs/news/news.component';
import { NotificationsComponent } from './tabs/notifications/notifications.component';
import { AccountComponent } from './tabs/account/account.component';
import { SurveyComponent } from './tabs/survey/survey.component';
import { homeRouter } from './tabs/home/home.routes';
import { surveyRoutes } from './tabs/survey/survey.routes';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: 'home', children: homeRouter },
      { path: 'report', component: ReportComponent },
      { path: 'news', component: NewsComponent },
      { path: 'notifications', component: NotificationsComponent },
      { path: 'account', component: AccountComponent },
      { path: 'survey', children: surveyRoutes},
      { path: 'survey', component: SurveyComponent },
    ],
  },
];
