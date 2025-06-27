import { Routes } from '@angular/router';
import { ListComponent as StoreListComponent } from './list/list.component';
import { EvaluateComponent } from './evaluate/evaluate.component';
import { SurveyComponent } from './survey.component';
import { CheckListComponent as StoreCheckListComponent } from './check-list/check-list.component';

export const surveyRoutes: Routes = [
  {
    path: '',
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: '', component: SurveyComponent },
      // { path: 'store/list/:id', component: StoreListComponent },
      {
        path: 'list/:id',
        loadComponent: () =>
          import('./list/list.component').then((m) => m.ListComponent),
      },
      {
        path: 'check-list/:id',
        loadComponent: () =>
          import('./check-list/check-list.component').then(
            (m) => m.CheckListComponent
          ),
      },
      {
        path: 'evaluate/:mode/:code',
        loadComponent: () =>
          import('./evaluate/evaluate.component').then(
            (m) => m.EvaluateComponent
          ),
      },
    ],
  },
];
