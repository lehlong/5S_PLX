import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss'],
  imports: [SharedModule],
  standalone: true
})
export class NotificationsComponent  implements OnInit {

  constructor() { }

  ngOnInit() {}

}
