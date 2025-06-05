import { Routes } from '@angular/router';
import { ListComponent } from './strore/list/list.component';
import { EvaluateComponent } from './strore/evaluate/evaluate.component';
import { SurveyComponent } from './survey.component';

export const surveyRoutes: Routes = [
  {
    path: '',
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: '', component: SurveyComponent },
      { path: 'store/list/:id', component: ListComponent },
      { path: 'store/evaluate', component: EvaluateComponent },
      { path: 'ware-house/list', component: ListComponent },
      { path: 'ware-house/evaluate', component: EvaluateComponent },
    ],
  },
];
