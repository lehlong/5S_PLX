import { Routes } from '@angular/router';

export const surveyRoutes: Routes = [
  {
    path: '',
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      // { path: '', component: HomeComponent },
    ],
  },
];
