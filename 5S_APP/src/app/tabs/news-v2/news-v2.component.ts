import { routes } from './../../app.routes';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-news-v2',
  templateUrl: './news-v2.component.html',
  styleUrls: ['./news-v2.component.scss'],
  imports: [SharedModule],
})
export class NewsV2Component implements OnInit {
  fuelPrices = [
    { name: 'Xăng RON 95-IV', price: 24500, unit: 'đ/lít' },
    { name: 'Xăng E5 RON 92-II', price: 23600, unit: 'đ/lít' },
    { name: 'Dầu DO 0,05S-II', price: 21300, unit: 'đ/lít' },
    { name: 'Dầu hỏa', price: 20900, unit: 'đ/lít' },
  ];
  newsList = [
    {
      id: 1,
      title: 'Giá xăng dầu trong nước giữ ổn định',
      date: '20/08/2025',
      summary:
        'Bộ Công Thương vừa công bố giá bán lẻ xăng dầu trong kỳ điều chỉnh mới nhất...',
    },
    {
      id: 2,
      title: 'Petrolimex ra mắt ứng dụng tra cứu giá',
      date: '18/08/2025',
      summary:
        'Người dùng có thể dễ dàng tra cứu giá xăng dầu và tin tức mới nhất ngay trên app.',
    },
    {
      id: 3,
      title: 'Xu hướng tiêu thụ nhiên liệu 2025',
      date: '15/08/2025',
      summary: 'Nhu cầu nhiên liệu dự kiến tăng trưởng nhẹ trong nửa cuối năm.',
    },
  ];
  constructor(private router: Router) {}

  ngOnInit() {}
  goToLogin() {
    this.router.navigate(['/login']);
    console.log('goLogin');
  }
  goToDetail(newsId: any) {
    this.router.navigate(['/news', newsId]);
  }
}
