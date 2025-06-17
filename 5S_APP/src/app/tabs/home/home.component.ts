import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';
import { SurveyService } from 'src/app/service/survey.service';
import { IonAccordionGroup } from '@ionic/angular';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [SharedModule, RouterModule],
  standalone: true
})
export class HomeComponent  implements OnInit {
data: any[] = [];

  @ViewChild('accordionGroup', { static: true })
  accordionGroup!: IonAccordionGroup;

  constructor(
    private _service: SurveyService,
    private router: Router
  ) { }

  ngOnInit() {
    this.getSurveyMgmt();

  }


  navigateTo(item: any) {
    const { doiTuongId, id } = item
    if (doiTuongId === 'DT1') {
      this.router.navigate([`/survey/store/list/${id}`]);
    } else if (doiTuongId === 'DT2') {
      this.router.navigate([`/survey/ware-house/list/${id}`]);
    }
  }

  getSurveyMgmt() {
    this._service.getAllSurveyMgmt({}).subscribe({
      next: (data) => {
        this.data = data.data;
        // console.log('data', data);
      },
      error: (response) => {
        console.log(response)
      },
    });
  }
}
