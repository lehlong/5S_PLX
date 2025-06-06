import { Component, OnInit } from '@angular/core';
import { ButtonFilterComponent } from 'src/app/shared/button-filter/button-filter.component';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  standalone: true,
  imports: [SharedModule, ButtonFilterComponent],
})
export class ListComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
