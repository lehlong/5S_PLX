import { Routes } from '@angular/router';
import { ListComponent as StoreListComponent } from './strore/list/list.component';
import { ListComponent as WareHouseListComponent } from './ware-house/list/list.component';
import { EvaluateComponent } from './strore/evaluate/evaluate.component';
import { SurveyComponent } from './survey.component';
import { CheckListComponent as StoreCheckListComponent } from './strore/check-list/check-list.component';

export const surveyRoutes: Routes = [
  {
    path: '',
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: '', component: SurveyComponent },
      // { path: 'store/list/:id', component: StoreListComponent },
      {
        path: 'store/list/:id',
        loadComponent: () =>
          import('./strore/list/list.component').then((m) => m.ListComponent),
      },
      {
        path: 'store/check-list/:id',
        loadComponent: () =>
          import('./strore/check-list/check-list.component').then(
            (m) => m.CheckListComponent
          ),
      },
      {
        path: 'store/evaluate/:mode/:code',
        loadComponent: () =>
          import('./strore/evaluate/evaluate.component').then(
            (m) => m.EvaluateComponent
          ),
      },
      {
        path: 'ware-house/list/:id',
        loadComponent: () =>
          import('./ware-house/list/list.component').then(
            (m) => m.ListComponent
          ),
      },
      {
        path: 'ware-house/evaluate',
        loadComponent: () =>
          import('./ware-house/evaluate/evaluate.component').then(
            (m) => m.EvaluateComponent
          ),
      },
    ],
  },
];
