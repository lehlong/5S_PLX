import { Component, OnInit, ViewChild } from '@angular/core';
import { IonContent, IonSegmentContent, IonSegmentView  } from "@ionic/angular/standalone";
import { SharedModule } from 'src/app/shared/shared.module';
@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrls: ['./news.component.scss'],
  imports: [
    SharedModule,
    IonSegmentContent,
    IonSegmentView
  ],
  standalone: true
})
export class NewsComponent implements OnInit {

  constructor() { }

  ngOnInit() { }

  slideOpts = {
    initialSlide: 0,
    speed: 400,
    loop: true,
    autoplay: {
      delay: 3000,
    },
  };

}
