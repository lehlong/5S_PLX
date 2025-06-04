import { Routes } from '@angular/router';
import { HomeComponent } from './home.component';
import { ScoringFiveSComponent } from './grading/scoring-five-s/scoring-five-s.component';
import { NewsComponent } from '../news/news.component';
export const homeRouter: Routes = [
  {path: '',component: HomeComponent},
  {path: 'cham-diem-5s', component: ScoringFiveSComponent},
  {path: 'news', component: NewsComponent}
];
