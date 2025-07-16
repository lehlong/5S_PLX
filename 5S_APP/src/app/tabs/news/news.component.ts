import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import {
  IonContent,
  IonSegmentContent,
  IonSegmentView,
} from '@ionic/angular/standalone';
import { HomeService } from 'src/app/service/home.service';
import { SharedModule } from 'src/app/shared/shared.module';
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
  dataHomeStore: any;
  dataHomeWareHouse: any;
  dataHomeAll: any;
  dataHomeChuaCham: any;
  dataChucVu: any;
  formattedDate: string = '';
  selected: string = 'all';
  userInfo: UserInfo = {
    fullName: '',
    phoneNumber: '',
    email: '',
    chucVuId: '',
    chucVuName: '',
    userName: '',
  };
  buttons = [
    { label: 'Tất cả', value: 'all' },
    { label: 'Cửa hàng', value: 'store' },
    { label: 'Kho', value: 'warehouse' },
    { label: 'Chưa chấm', value: 'chuaCham' },
  ];
  constructor(private _service: HomeService, private router: Router) {}

  ngOnInit() {
    this.loadUserInfo();
    this.getChucVu();
    this.formatToday();
    this.getDataHome();
  }

  slideOpts = {
    initialSlide: 0,
    speed: 400,
    loop: true,
    autoplay: {
      delay: 3000,
    },
  };
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
    this.selected = value;
  }
  getDataHome() {
    this._service.getDataHome(this.userInfo.userName).subscribe((data: any) => {
      const sortedList = [...data.lstDoiTuong].sort((a: any, b: any) => {
        return b.isScore === false ? 1 : -1; 
      });

      this.dataHomeAll = sortedList;
      this.dataHomeStore = sortedList.filter((x: any) => x.type === 'DT1');
      this.dataHomeWareHouse = sortedList.filter((x: any) => x.type === 'DT2');
      this.dataHomeChuaCham = sortedList.filter(
        (x: any) => x.isScore === true
      );
    });
  }

  formatToMonthYear(dateStr: string): string {
    const date = new Date(dateStr);
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    return `T${month}/${year}`;
  }
  getChamDiemStatus(fDate: string): string {
    const date = new Date(fDate);
    const now = new Date();

    const currentMonth = now.getMonth();
    const currentYear = now.getFullYear();

    const dateMonth = date.getMonth();
    const dateYear = date.getFullYear();
    const dateDay = now.getDate();

    if (dateMonth !== currentMonth || dateYear !== currentYear) {
      return 'Ngoài thời gian chấm';
    }

    if (dateDay >= 1 && dateDay <= 7) {
      return `Trong thời gian (01-07/${(currentMonth + 1)
        .toString()
        .padStart(2, '0')})`;
    }

    if (dateDay >= 15 && dateDay <= 23) {
      return `Trong thời gian (15-23/${(currentMonth + 1)
        .toString()
        .padStart(2, '0')})`;
    }

    return 'Ngoài thời gian chấm';
  }

  navigateItem(item: any) {
    const filterStr = localStorage.getItem('filterCS');
    if (!filterStr) return;

    const filter = JSON.parse(filterStr);

    filter.doiTuong = item;

    filter.doiTuong.lstChamDiem = [this.userInfo.userName];
    const kiKhaoSatId = item.kiKhaoSatId;
    const kiKhaoSatName = item.kiKhaoSatName;
    const doiTuongText =
      item.type === 'DT1'
        ? 'Cửa hàng'
        : item.type === 'DT2'
        ? 'Kho'
        : 'Không xác định';
    filter.kiKhaoSat.doiTuong = doiTuongText;
    filter.kiKhaoSat.id = kiKhaoSatId;
    filter.kiKhaoSat.code = kiKhaoSatName;
    filter.kiKhaoSat.name = kiKhaoSatName;

    localStorage.setItem('filterCS', JSON.stringify(filter));

    this.router.navigate([`survey/check-list/${item.id}`]);
  }
}
