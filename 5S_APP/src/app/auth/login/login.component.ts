
import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { GlobalService } from '../../service/global.service';
import { AuthService } from 'src/app/service/auth.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { MessageService } from 'src/app/service/message.service';
import { PushNotifications } from '@capacitor/push-notifications';
import { Device } from '@capacitor/device';
import { FCM } from '@capacitor-community/fcm';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  constructor(
    private router: Router,
    private globalService: GlobalService,
    private authService: AuthService,
    private messageService: MessageService,

  ) { }
  info: any = {};
  deviceId: string = '';
  isLogin: boolean = false;
  model = {
    userName: '',
    password: '',
    deviceId: '',
    deviceName: '',
    model: '',
    operatingSystem: '',
    osVersion: '',
    manufacturer: '',

  }
  ngOnInit() {
    this.logDeviceID();
    this.logDeviceInfo();
  }

  logDeviceID = async () => {
    const Id = await Device.getId();
    this.deviceId = Id ? Id.identifier : '';

  };

  logDeviceInfo = async () => {
    const infoDevice = await Device.getInfo();
    this.info = infoDevice
  }

  saveAccount: boolean = false

  processLogin() {
    this.isLogin = true;
    this.model.deviceId = this.deviceId;
    // this.model.deviceId = 'faf599b6-41d0-4156-b34d-ad6b16d69282';
    // this.model.deviceId = "47d6a7ca58006a50";
    this.model.deviceName = this.info.name ;
    // this.model.deviceName = 'iphone';
    this.model.model = this.info.model;
    this.model.operatingSystem = this.info.operatingSystem;
    this.model.osVersion = this.info.osVersion;
    this.model.manufacturer = this.info.manufacturer;

    if (this.model.userName == '' || this.model.password == '') {
      this.messageService.show('Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!', 'danger');
      this.isLogin = false;
      return;
    }

    this.authService.login(this.model).subscribe({
      next: (response) => {

        localStorage.setItem('token', response.accessToken)
        localStorage.setItem('refreshToken', response.refreshToken)
        this.globalService.setUserInfo(response.accountInfo)
        localStorage.setItem('openSidebar', 'true')
        localStorage.setItem('companyCode', response?.accountInfo?.organizeCode)
        localStorage.setItem('warehouseCode', response?.accountInfo?.warehouseCode)
        this.subscribeToTestTopic('PLX5S_NOTI');

        const userName = response?.accountInfo?.userName;
        if (userName) {
          this.globalService.setUserName(userName);
        }
        this.authService
          .getRightOfUser({ userName: response?.accountInfo?.userName })
          .subscribe({
            next: (rights) => {
              this.registerPushNotifications()
              this.globalService.setRightData(JSON.stringify(rights || []))
              this.router.navigate(['/'])
            },
            error: (error) => {

              this.messageService.show(`Lỗi không lấy được danh sách quyền của user!`, 'warning');
              console.log('Lỗi hệ thống:', error)
            },
          })
      },
      error: (error) => {

        this.messageService.show(`${error}`, 'warning');
        console.log(error);
        this.isLogin = false;
      }
    });
  }

  showPassword = false;

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  async subscribeToTestTopic(topic: string) {
    try {
      await FCM.subscribeTo({ topic: topic });
      console.log('Đã đăng ký topic', topic);
    } catch (error: any) {
      console.error('Lỗi đăng ký topic:', error);
    }
  }


  async registerPushNotifications() {
    let permStatus = await PushNotifications.checkPermissions();
    // alert('permStatus: ' + permStatus);

    if (permStatus.receive === 'prompt') {
      permStatus = await PushNotifications.requestPermissions();
    }
    if(permStatus.receive !== 'granted') {
      // alert('Push notifications permission denied');
    }
    if (permStatus.receive === 'granted') {
      try {
        await PushNotifications.register();
        // alert('Push notifications registered successfully');
      } catch (error) {
        console.error('Error registering push notifications:', error);
        // alert('Error registering push notifications: ' + error);
      }
    }

  }
}
