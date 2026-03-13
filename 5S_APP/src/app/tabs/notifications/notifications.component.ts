import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { navigate } from 'ionicons/icons';
import { AppEvaluateService } from 'src/app/service/app-evaluate.service';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss'],
  imports: [SharedModule],
  standalone: true
})
export class NotificationsComponent implements OnInit {

  lstData: any = [];
  account: any

  constructor(
    private _service: AppEvaluateService,
    private router: Router,
  ) { }

  ngOnInit() {
    this.account = JSON.parse(localStorage.getItem('UserInfo') ?? '');
    this.getNotifications();
  }


  getNotifications() {
    this._service.getNotifications(this.account.userName).subscribe({
      next: (data) => {
        this.lstData = data;
      }
    }
    );
  }

  readNoti(noti: any) {
    console.log(noti);
    this._service.readNoti(noti).subscribe({
      next: () => {
        this.getNotifications()
      }
    })

    if (noti.link) {
      this.router.navigate([`home`]);
    }


  }

}
