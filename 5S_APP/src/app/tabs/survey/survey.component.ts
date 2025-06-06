import { Component, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { IonAccordionGroup } from '@ionic/angular';
import { Router, RouterModule } from '@angular/router';
import { SurveyService } from 'src/app/service/survey.service';

@Component({
  selector: 'app-survey',
  imports: [RouterModule, SharedModule],
  templateUrl: './survey.component.html',
  styleUrls: ['./survey.component.scss'],
  standalone: true,
})
export class SurveyComponent implements OnInit {
  data: any[] = [];

  @ViewChild('accordionGroup', { static: true })
  accordionGroup!: IonAccordionGroup;

  constructor(
    private _service: SurveyService,
    private router: Router
  ) { }

  ngOnInit() {
    this.getSurveyMgmt()
  }

  navigateTo(item: any) {
    const { doiTuongId, id } = item
    if (doiTuongId === 'DT1') {
      this.router.navigate([`/survey/store/list/${id}`]);
    } else if (doiTuongId === 'DT2') {
      this.router.navigate([`/survey/ware-house/${id}`]);
    }
  }

  getSurveyMgmt() {
    this._service.getAllSurveyMgmt({}).subscribe({
      next: (data) => {
        this.data = data.data;
        console.log('data', data);
      },
      error: (response) => {
        console.log(response)
      },
    });
  }
}
