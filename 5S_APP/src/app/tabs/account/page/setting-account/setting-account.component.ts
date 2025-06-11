import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-setting-account',
  templateUrl: './setting-account.component.html',
  styleUrls: ['./setting-account.component.scss'],
  standalone: true,
  imports: [SharedModule]
})
export class SettingAccountComponent  implements OnInit {

  constructor(
    private router: Router
  ) { }

  ngOnInit() {}
  navigateTo(route: string){
    this.router.navigate([route])
  }
}
