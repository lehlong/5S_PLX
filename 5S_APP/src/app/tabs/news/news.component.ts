import { Component, OnInit } from '@angular/core';
import { IonContent } from "@ionic/angular/standalone";
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-news',
  templateUrl: './news.component.html',
  styleUrls: ['./news.component.scss'],
  imports: [SharedModule],
  standalone: true
})
export class NewsComponent  implements OnInit {

  constructor() { }

  ngOnInit() {}

}
