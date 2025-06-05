import { Routes } from '@angular/router';
import { ListComponent as StoreListComponent } from './strore/list/list.component';
import { ListComponent as WareHouseListComponent } from './ware-house/list/list.component';
import { EvaluateComponent } from './strore/evaluate/evaluate.component';
import { SurveyComponent } from './survey.component';

export const surveyRoutes: Routes = [
  {
    path: '',
    children: [
      { path: '', redirectTo: 'home', pathMatch: 'full' },
      { path: '', component: SurveyComponent },
      { path: 'store/list/:id', component: StoreListComponent },
      { path: 'store/list', component: StoreListComponent },
      { path: 'store/evaluate', component: EvaluateComponent },
      { path: 'ware-house/list', component: WareHouseListComponent },
      { path: 'ware-house/evaluate', component: EvaluateComponent },
    ],
  },
];
