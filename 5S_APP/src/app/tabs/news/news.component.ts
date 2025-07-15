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
}
@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrls: ['./news.component.scss'],
  imports: [SharedModule, IonSegmentContent, IonSegmentView],
  standalone: true,
})
export class NewsComponent implements OnInit {
  dataChucVu: any;
  formattedDate: string = '';
  userInfo: UserInfo = {
    fullName: '',
    phoneNumber: '',
    email: '',
    chucVuId: '',
    chucVuName: '',
  };

  constructor(private _service: HomeService, private router: Router) {}

  ngOnInit() {
    this.loadUserInfo();
    this.getChucVu();
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
  loadUserInfo() {
    const userInfoString = localStorage.getItem('UserInfo');
    if (userInfoString) {
      const userInfo = JSON.parse(userInfoString);
      console.log(userInfo);
      this.userInfo = userInfo;
    }
  }
  getChucVu() {
    this._service.getAllChucVu().subscribe((data: any) => {
      this.dataChucVu = data;
      console.log(this.dataChucVu);
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
}
