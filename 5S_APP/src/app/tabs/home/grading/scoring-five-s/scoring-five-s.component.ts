import { Component, OnInit } from '@angular/core';
import { ButtonFilterComponent } from 'src/app/shared/button-filter/button-filter.component';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-scoring-five-s',
  templateUrl: './scoring-five-s.component.html',
  styleUrls: ['./scoring-five-s.component.scss'],
  standalone: true,
  imports: [SharedModule, ButtonFilterComponent],
})
export class ScoringFiveSComponent implements OnInit {
  selectValue = '1';
  constructor() {}

  ngOnInit() {}
}
