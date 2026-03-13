import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoginComponent } from 'src/app/auth/login/login.component';
import { PushNotificationService } from 'src/app/service/common/push-notification.service';
import { StorageService } from 'src/app/service/storage.service';
import { SharedModule } from 'src/app/shared/shared.module';

interface UserInfo {
  fullName: string;
  phoneNumber: string;
  email: string;
}
@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss'],
  imports: [SharedModule],
  standalone: true,
})
export class AccountComponent implements OnInit {
  apiUrl: any;
  userInfo: UserInfo = {
    fullName: '',
    phoneNumber: '',
    email: '',
  };
  constructor(
    private router: Router,
    private _storageService: StorageService,
    private _pushNotiS: PushNotificationService
  ) { }

  ngOnInit() {
    this.loadUserInfo();
  }
  navigateTo(route: string) {
    this.router.navigate([route]);
  }

  loadUserInfo() {
    const userInfoString = localStorage.getItem('UserInfo');
    if (userInfoString) {
      const userInfo = JSON.parse(userInfoString);
      console.log(userInfo);
      this.userInfo = userInfo;
    }
  }

  Logout() {
    this.apiUrl = localStorage.getItem('CapacitorStorage.apiUrl');
    localStorage.clear();
    sessionStorage.clear();
    this._pushNotiS.clear()
    localStorage.setItem('CapacitorStorage.apiUrl', this.apiUrl);
    this.router.navigate(['/login']).then(() => {
      window.location.reload();
    });
  }


}
