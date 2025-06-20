import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IonContent } from "@ionic/angular/standalone";
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.scss'],
  imports:[SharedModule],
  standalone: true
})
export class ReportComponent  implements OnInit {

  constructor(
    private router: Router
  ) { }

  ngOnInit() {}
  navigateTo(route: string){
    this.router.navigate([route])
  }
}
