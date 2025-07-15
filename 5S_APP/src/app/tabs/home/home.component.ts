import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';
import { SurveyService } from 'src/app/service/survey.service';
import { IonAccordionGroup } from '@ionic/angular';
import { PushNotifications } from '@capacitor/push-notifications';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [SharedModule, RouterModule],
  standalone: true
})
export class HomeComponent implements OnInit {
  data: any[] = [];

  @ViewChild('accordionGroup', { static: true })
  accordionGroup!: IonAccordionGroup;

  constructor(
    private _service: SurveyService,
    private router: Router
  ) { }

  ngOnInit() {
    this.getSurveyMgmt();
  }


  navigateTo(item: any) {
    const { doiTuongId, id } = item
    localStorage.setItem('surveyId', id);
      this.router.navigate([`/survey/list/${id}`]);
  }

  getSurveyMgmt() {
    this._service.getAllSurveyMgmt({}).subscribe({
      next: (data) => {
        this.data = data.data;
        // console.log('data', data);
      },
      error: (response) => {
        console.log(response)
      },
    });
  }

  addlisteners = async () => {
    await PushNotifications.requestPermissions().then(result => {
      if (result.receive === 'granted') {
        PushNotifications.register();
      }
    });

    await PushNotifications.addListener('registration', token => {
      console.log('Push registration success, token:', token.value);
      alert('Push registration success, token:'+ token.value);
      // Gửi token lên server nếu cần
    });

    await PushNotifications.addListener('registrationError', error => {
      console.error('Push registration error:', error);
      alert('Push registration error:'+ error);

    });

    await PushNotifications.addListener('pushNotificationReceived', notification => {
      console.log('Push received: ', notification);
      alert('Push received:'+ notification);

    });

    await PushNotifications.addListener('pushNotificationActionPerformed', notification => {
      console.log('Push action performed', notification);
      alert('Push action performed:'+ notification);

    });
  }

  async registerPushNotifications() {
    let permStatus = await PushNotifications.checkPermissions();
    alert('permStatus: ' + permStatus);

    if (permStatus.receive === 'prompt') {
      permStatus = await PushNotifications.requestPermissions();
    }
    if(permStatus.receive !== 'granted') {
      alert('Push notifications permission denied');
    }
    if (permStatus.receive === 'granted') {
      try {
        await PushNotifications.register();
        alert('Push notifications registered successfully');
      } catch (error) {
        console.error('Error registering push notifications:', error);
        alert('Error registering push notifications: ' + error);
      }
    }

  }

  getDeliveredNotifications = async () => {
    const notifications = await PushNotifications.getDeliveredNotifications();
    console.log('Delivered notifications:', JSON.stringify(notifications));

  }

}
