import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NewsService } from 'src/app/service/news.service';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-news-detail',
  templateUrl: './news-detail.component.html',
  styleUrls: ['./news-detail.component.scss'],
  imports: [SharedModule],
})
export class NewsDetailComponent implements OnInit {
  newsId: any | null = null;
  newsDetail: any;
  DataGetAll: any = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: NewsService
  ) {}

  ngOnInit() {
    this.newsId = this.route.snapshot.paramMap.get('id');
    console.log('newsId', this.newsId);
    console.log('newsDetail', this.newsDetail);
    this.getAllNews()
  }
  getAllNews() {
    this.service.getAll().subscribe({
      next: (data) => {
        this.DataGetAll = data;
        console.log('Danh sách news:', data);
        this.newsDetail = this.DataGetAll.find(
          (n: any) => n.id === this.newsId
        );
        console.log('newsDetail:', this.newsDetail);
      },
      error: (err) => {
        console.error('Lỗi khi gọi getAll:', err);
      },
    });
  }
}
