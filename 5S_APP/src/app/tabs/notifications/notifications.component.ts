import { Component, OnInit } from '@angular/core';
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

  constructor(
    private _service: AppEvaluateService,

  ) { }

  ngOnInit() {
    this.getNotifications();
   }


  getNotifications() {
    this._service.getNotifications().subscribe({
      next: (data) => {

        console.log(data);
        this.lstData = data;
      }
    }
    );
    // Logic to fetch notifications
    console.log('Fetching notifications...');
  }

}
