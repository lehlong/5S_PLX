import { routes } from './../../app.routes';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NewsService } from 'src/app/service/news.service';
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
  newsList: any = [];
  constructor(private router: Router, private service: NewsService) {}

  ngOnInit() {
    this.getAllNews()
  }
  getAllNews() {
    this.service.getAll().subscribe({
      next: (data) => {
        console.log('Danh sách news:', data);
        this.newsList = data; 
      },
      error: (err) => {
        console.error('Lỗi khi gọi getAll:', err);
      },
    });
  }

  goToLogin() {
    this.router.navigate(['/login'], { replaceUrl: true });
    console.log('goLogin');
  }
  goToDetail(newsId: any) {
    this.router.navigate(['/news', newsId]);
  }
}
