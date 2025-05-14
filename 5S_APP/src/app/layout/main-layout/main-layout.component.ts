import { Component, EnvironmentInjector, inject, OnInit } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss'],
  imports: [SharedModule],
  standalone: true
})
export class MainLayoutComponent implements OnInit {
  public environmentInjector = inject(EnvironmentInjector);

  constructor() {}

  ngOnInit() {}
}
