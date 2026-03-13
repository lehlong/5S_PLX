import { Component, OnInit, ViewChild } from '@angular/core';
import { LoadingController, ToastController } from '@ionic/angular';
import { Router } from '@angular/router';
import {
  IonContent,
  IonSegmentContent,
  IonSegmentView,
} from '@ionic/angular/standalone';
import { HomeService } from 'src/app/service/home.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { ConfigService } from 'src/app/service/config.service';
import { PaginationResult } from 'src/app/models/base.model';
import { GlobalService } from 'src/app/service/global.service';
import { PushNotificationService } from 'src/app/service/common/push-notification.service';
import { FCM } from '@capacitor-community/fcm';
import { AuthService } from 'src/app/service/auth.service';
interface UserInfo {
  fullName: string;
  phoneNumber: string;
  email: string;
  chucVuId: string;
  chucVuName: string;
  userName: string;
}
@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrls: ['./news.component.scss'],
  imports: [SharedModule, IonSegmentContent, IonSegmentView],
  standalone: true,
})
export class NewsComponent implements OnInit {
  fullData: any;
  dataHomeStore: any;
  dataHomeWareHouse: any;
  dataHomeAll: any;
  dataHome: any;
  dataHomeChuaCham: any;
  dataChucVu: any;
  paginationResult = new PaginationResult()
  formattedDate: string = '';
  storeLength: string = '(0)';
  wareHouseLength: string = '(0)';
  chuaChamLength: string = '(0)';
  selected: string = 'all';
  userInfo: UserInfo = {
    fullName: '',
    phoneNumber: '',
    email: '',
    chucVuId: '',
    chucVuName: '',
    userName: '',
  };

  filter: any = {
    doiTuong: null,
    kiKhaoSat: {
      id: null,
      code: null,
      name: null,
      doiTuong: null,
      surveyMgmtId: null,
    },
  };
  buttons = [
    { label: 'Tất cả', value: 'all' },
    { label: 'Tôi chấm', value: 'toiCham' },
    // { label: 'Cửa hàng', value: 'store' },
    // { label: 'Kho', value: 'warehouse' },
    { label: 'Chưa chấm', value: 'chuaCham' },
  ];
  constructor(
    private _service: HomeService,
    private router: Router,
    private loadingController: LoadingController,
    private configService: ConfigService,
    private globalS: GlobalService,
    private _pushNotiS: PushNotificationService,
    private authService: AuthService,
  ) { }

  ngOnInit() {
    this.loadUserInfo();
    this.configService.apiUrl$.subscribe((url) => {
      this.getChucVu();
      this.getDataHome();
      this.search();
    });
    this.formatToday();
  }

  slideOpts = {
    initialSlide: 0,
    speed: 400,
    loop: true,
    autoplay: {
      delay: 3000,
    },
  };

  search() {
    this._service.search().subscribe({
      next: (data) => {
        this.paginationResult = data
      },
      error: (response) => {
      },
    })
  }

  loadUserInfo() {
    const userInfoString = localStorage.getItem('UserInfo');
    if (userInfoString) {
      const userInfo = JSON.parse(userInfoString);
      this.userInfo = userInfo;
    }
  }
  getChucVu() {
    this._service.getAllChucVu().subscribe((data: any) => {
      this.dataChucVu = data;
    });
  }
  getChucVuName(id: string): string {
    const found = this.dataChucVu?.find((x: any) => x.id === id);
    return found ? found.name : 'Không xác định';
  }
  formatToday() {
    const today = new Date();

    const days = [
      'Chủ Nhật',
      'Thứ Hai',
      'Thứ Ba',
      'Thứ Tư',
      'Thứ Năm',
      'Thứ Sáu',
      'Thứ Bảy',
    ];
    const dayName = days[today.getDay()];
    const day = today.getDate();
    const month = today.getMonth() + 1;
    const year = today.getFullYear();

    this.formattedDate = `${dayName}, Ngày ${day} tháng ${month} năm ${year}`;
  }

  navigateTo(id: any) {
    this.router.navigate([`/survey/list/${id}`]);
  }

  select(value: string) {
    if (value === 'all') {
      this.dataHome = this.dataHomeAll;
    } else if (value === 'store') {
      this.dataHome = this.dataHomeAll.filter((x: any) => x.type === 'DT1');
    } else if (value === 'warehouse') {
      this.dataHome = this.dataHomeAll.filter((x: any) => x.type === 'DT2');
    } else if (value === 'chuaCham') {
      this.dataHome = this.dataHomeAll.filter((x: any) => x.isScore === true);
    } else if (value === 'toiCham') {
      this.dataHome = this.dataHomeAll.filter((x: any) => x.description != "");
    }
    this.selected = value;
  }

  initPush(userName: any) {
    const tokenNoti = localStorage.getItem("tokenNoti")
    console.log(tokenNoti);
    if (tokenNoti == null) {
      this._pushNotiS.initialize()
      FCM.getToken().then(token => {
        console.log(token);
        
        if (token?.token) {
          this.saveFirebaseTokenToServer(token.token, userName);
          localStorage.setItem('tokenNoti', token.token)
        } else {
          console.warn("Không lấy được token!");
        }
      });
    }
  }

  saveFirebaseTokenToServer(token: any, userName: string) {
    console.log('qqqqqqqqqqqqqqqqq', token);
    this.authService.saveUserTokenNoti({
      id: '',
      userName: userName,
      token: token ?? ""
    }).subscribe({
      next: (resp) => {
      }
    })
  }
  getDataHome() {
    this._service.getDataHome(this.userInfo.userName).subscribe({
      next: (data: any) => {
        this.fullData = data;
        console.log(this.fullData);

        const sortedList = [...data.lstDoiTuong].sort((a: any, b: any) => {
          return b.isScore === true ? 1 : -1;
        });

        this.dataHomeAll = sortedList;
        this.storeLength = `(${sortedList.filter((x: any) => x.type === 'DT1').length
          })`;
        this.wareHouseLength = `(${sortedList.filter((x: any) => x.type === 'DT2').length
          })`;
        this.chuaChamLength = `(${sortedList.filter((x: any) => x.isScore === true).length
          })`;
        this.select(this.selected);
        this.initPush(this.userInfo.userName)
        this.dataHomeStore = sortedList.filter((x: any) => x.type === 'DT1');
        this.dataHomeWareHouse = sortedList.filter((x: any) => x.type === 'DT2');
        this.dataHomeChuaCham = sortedList.filter((x: any) => x.isScore === true);
      }
    })
  }


  navigateItem(item: any) {
    this.filter.doiTuong = item;

    const doiTuongText =
      item.type === 'DT1'
        ? 'Cửa hàng'
        : item.type === 'DT2'
          ? 'Kho'
          : 'Không xác định';

    this.filter.kiKhaoSat.doiTuong = doiTuongText;

    this.filter.kiKhaoSat.id = item.kiKhaoSatId;
    this.filter.kiKhaoSat.name = item.kiKhaoSatName;
    this.filter.kiKhaoSat.code = item.kiKhaoSatCode;
    this.filter.kiKhaoSat.trangThaiKi = '2';
    this.filter.kiKhaoSat.endDate = item.endDate;
    this.filter.kiKhaoSat.surveyMgmtId = item.surveyId;

    localStorage.setItem('filterCS', JSON.stringify(this.filter));

    this.router.navigate([`survey/check-list/${item.id}`]);
  }

  async doRefresh(event: any) {
    console.log('🔄 Pull to refresh triggered');

    try {
      this.getDataHome();
      this.getChucVu();
      this.search()
    } catch (error) {
      console.error('❌ Lỗi khi làm mới dữ liệu:', error);
    } finally {
      // 🚨 Quan trọng: Phải gọi complete() để ẩn spinner
      event.target.complete();
    }
  }

  async reload() {
    const loading = await this.loadingController.create({
      message: 'Đang tải...',
      spinner: 'circles',
    });

    await loading.present();

    try {
      await this.getDataHome();
      await this.getChucVu();
    } catch (error) {
      console.error('❌ Lỗi reload:', error);
    } finally {
      await loading.dismiss();
    }
  }

  // 🚀 Tùy chỉnh thêm: Tự động refresh mỗi 30 giây
  private autoRefreshInterval: any;

  ionViewWillLeave() {
    // Dừng auto refresh khi rời khỏi trang
    this.stopAutoRefresh();
  }

  private stopAutoRefresh() {
    if (this.autoRefreshInterval) {
      clearInterval(this.autoRefreshInterval);
    }
  }
}
