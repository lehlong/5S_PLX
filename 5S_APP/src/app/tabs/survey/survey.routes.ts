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
      { path: 'store/list/:id', component: StoreListComponent },
      { path: 'store/check-list/:id', component: StoreCheckListComponent },
      { path: 'store/evaluate/:mode/:code', component: EvaluateComponent },
      { path: 'ware-house/list/:id', component: WareHouseListComponent },
      { path: 'ware-house/evaluate', component: EvaluateComponent },
    ],
  },
];
