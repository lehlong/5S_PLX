import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { StorageService } from 'src/app/service/storage.service';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
  imports: [SharedModule],
  standalone: true,
})
export class AccountComponent implements OnInit {
  constructor(
    private router: Router,
    private _storageService: StorageService,

  ) { }

  ngOnInit() { }
  navigateTo(route: string) {
    this.router.navigate([route]);
  }


  Logout() {
    localStorage.clear()
    this.router.navigate(['/login'])
  }

}
