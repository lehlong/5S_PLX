import { Component, EnvironmentInjector, inject, OnInit } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.scss'],
  imports: [SharedModule],
  standalone: true,
})
export class MainLayoutComponent implements OnInit {
  showFooter = true;
  public environmentInjector = inject(EnvironmentInjector);

  constructor(private router: Router) {}

  ngOnInit() {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        // check url
        if (
          event.urlAfterRedirects.startsWith('/survey/evaluate/draft') ||
          event.urlAfterRedirects.startsWith('/survey/check-list')
        ) {
          this.showFooter = false;
        } else {
          this.showFooter = true;
        }
      }
    });
  }
}
