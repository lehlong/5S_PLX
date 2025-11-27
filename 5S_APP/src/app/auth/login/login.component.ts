import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { GlobalService } from '../../service/global.service';
import { AuthService } from 'src/app/service/auth.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { MessageService } from 'src/app/service/message.service';
import { PushNotifications } from '@capacitor/push-notifications';
import { Device } from '@capacitor/device';
import { FCM } from '@capacitor-community/fcm';
import { environment } from 'src/environments/environment';
import { Preferences } from '@capacitor/preferences';
import { ConfigService } from 'src/app/service/config.service';
import { Capacitor } from '@capacitor/core';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  constructor(
    private router: Router,
    private globalService: GlobalService,
    private authService: AuthService,
    private messageService: MessageService,
    private configService: ConfigService
  ) {}
  customUrl: string = '';
  private clickTimeout: any;
  clickCount = 0;
  showAccordion: boolean = false;
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
  };
  async ngOnInit() {
    const { value } = await Preferences.get({ key: 'apiUrl' });
    if (value) {
      this.customUrl = value;
    } else {
      this.customUrl = environment.apiUrl;
      await Preferences.set({ key: 'apiUrl', value: this.customUrl });
      this.configService.setApiUrl(this.customUrl);
    }
    this.logDeviceID();
    this.logDeviceInfo();
  }
  logDeviceID = async () => {
    const Id = await Device.getId();
    this.deviceId = Id ? Id.identifier : '';
  };

  logDeviceInfo = async () => {
    const infoDevice = await Device.getInfo();
    this.info = infoDevice;
  };

  saveAccount: boolean = false;

  async onUrlChange(newValue: string) {
    this.customUrl = newValue.trim();
    await this.configService.setApiUrl(this.customUrl);
  }
  async processLogin() {
    this.isLogin = true;
    const { value } = await Preferences.get({ key: 'apiUrl' });
    if (this.customUrl === '' || this.customUrl === '/api') {
      this.messageService.show(
        'Vui lòng nhập cấu hình đường dẫn url',
        'danger'
      );
      this.isLogin = false;
      return;
    }
    if (value && value.trim() !== '') {
      this.configService.setApiUrl(value.trim());
    }

    this.model.deviceId = this.deviceId;
    // this.model.deviceId = 'faf599b6-41d0-4156-b34d-ad6b16d69282';
    // this.model.deviceId = "47d6a7ca58006a50";
    this.model.deviceName = this.info.name;
    // this.model.deviceName = 'iphone';
    this.model.model = this.info.model;
    this.model.operatingSystem = this.info.operatingSystem;
    this.model.osVersion = this.info.osVersion;
    this.model.manufacturer = this.info.manufacturer;

    if (this.model.userName == '' || this.model.password == '') {
      this.messageService.show(
        'Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!',
        'warning'
      );
      this.isLogin = false;
      return;
    }

    this.authService.login(this.model).subscribe({
      next: (response) => {
        localStorage.setItem('token', response.accessToken);
        localStorage.setItem('refreshToken', response.refreshToken);
        this.globalService.setUserInfo(response.accountInfo);
        localStorage.setItem('openSidebar', 'true');
        localStorage.setItem(
          'companyCode',
          response?.accountInfo?.organizeCode
        );
        localStorage.setItem(
          'warehouseCode',
          response?.accountInfo?.warehouseCode
        );
        this.subscribeToTestTopic('PLX5S_NOTI');

        const userName = response?.accountInfo?.userName;
        if (userName) {
          this.globalService.setUserName(userName);
        }
        this.authService
          .getRightOfUser({ userName: response?.accountInfo?.userName })
          .subscribe({
            next: (rights) => {
              this.registerPushNotifications();
              this.globalService.setRightData(JSON.stringify(rights || []));
              this.router.navigate(['/home'], { replaceUrl: true });
            },
            error: (error) => {
              this.messageService.show(
                `Lỗi không lấy được danh sách quyền của user!`,
                'warning'
              );
              console.log('Lỗi hệ thống:', error);
            },
          });
      },
      error: (error) => {
        // this.messageService.show(`${error}`, 'warning');
        // this.messageService.show('Đường dẫn API bị sai !', 'warning');
        if (error === 'Thiết bị không có quyền đăng nhập') {
          this.messageService.show(
            'Thiết bị không có quyền đăng nhập',
            'warning'
          );
        } else if (error === 'Tên đăng nhập hoặc mật khẩu không đúng') {
          this.messageService.show(
            'Tên đăng nhập hoặc mật khẩu không đúng',
            'warning'
          );
        } else {
          this.messageService.show('Cấu hình API không chính xác !', 'warning');
        }
        console.log(error);
        this.isLogin = false;
      },
    });
  }

  showPassword = false;

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  // async subscribeToTestTopic(topic: string) {
  //   try {
  //     await FCM.subscribeTo({ topic: topic });
  //     console.log('Đã đăng ký topic', topic);
  //   } catch (error: any) {
  //     console.error('Lỗi đăng ký topic:', error);
  //   }
  // }

  async subscribeToTestTopic(topic: string) {
    if (Capacitor.getPlatform() !== 'web') {
      try {
        await FCM.subscribeTo({ topic });
        console.log('Đã đăng ký topic', topic);
      } catch (error: any) {
        console.error('Lỗi đăng ký topic:', error);
      }
    } else {
      console.warn('FCM subscribeTo không hỗ trợ trên web. Topic:', topic);
    }
  }

  async registerPushNotifications() {
    let permStatus = await PushNotifications.checkPermissions();
    // alert('permStatus: ' + permStatus);

    if (permStatus.receive === 'prompt') {
      permStatus = await PushNotifications.requestPermissions();
    }
    if (permStatus.receive !== 'granted') {
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

  showUrl() {
    this.clickCount++;
    console.log('click', this.clickCount);
    if (this.clickTimeout) {
      clearTimeout(this.clickTimeout);
    }
    this.clickTimeout = setTimeout(() => {
      this.clickCount = 0;
    }, 3000);
    if (this.clickCount >= 5) {
      this.showAccordion = true;
      this.clickCount = 0;
      clearTimeout(this.clickTimeout);
    }
  }
}
