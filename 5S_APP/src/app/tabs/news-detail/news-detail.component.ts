import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-news-detail',
  templateUrl: './news-detail.component.html',
  styleUrls: ['./news-detail.component.scss'],
  imports: [SharedModule],
})
export class NewsDetailComponent implements OnInit {
  newsId: number | null = null;
  newsDetail: any;
  fakeNewsData = [
    {
      id: 1,
      title: 'Giá xăng dầu trong nước giữ ổn định',
      date: '20/08/2025',
      content: `
        Bộ Công Thương vừa công bố giá bán lẻ xăng dầu trong kỳ điều chỉnh mới nhất.
        Theo đó, giá xăng RON 95-IV giữ ở mức 24.500 đ/lít, E5 RON 92-II ở mức 23.600 đ/lít.
        Việc giữ ổn định giá bán nhằm hỗ trợ người dân và doanh nghiệp trong bối cảnh giá dầu thế giới biến động.
      `,
      image: 'assets/img/1.webp',
    },
    {
      id: 2,
      title: 'Petrolimex ra mắt ứng dụng tra cứu giá',
      date: '18/08/2025',
      content: `
        Ứng dụng Petrolimex cho phép người dùng tra cứu giá xăng dầu, 
        cập nhật tin tức mới nhất và tích điểm khi mua nhiên liệu. 
        Đây là bước tiến quan trọng trong việc chuyển đổi số ngành xăng dầu tại Việt Nam.
      `,
      image: 'assets/img/2.webp',
    },
    {
      id: 3,
      title: 'Xu hướng tiêu thụ nhiên liệu 2025',
      date: '15/08/2025',
      content: `
        Nhu cầu nhiên liệu dự kiến tăng trưởng nhẹ trong nửa cuối năm 2025, 
        chủ yếu nhờ hoạt động sản xuất và vận tải. 
        Các chuyên gia dự báo giá dầu thế giới sẽ duy trì trong khoảng 80 - 85 USD/thùng.
      `,
      image: 'assets/img/3.webp',
    },
  ];

  constructor(private route: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    this.newsId = Number(this.route.snapshot.paramMap.get('id'));
    this.newsDetail = this.fakeNewsData.find((n) => n.id === this.newsId);
  }
}
