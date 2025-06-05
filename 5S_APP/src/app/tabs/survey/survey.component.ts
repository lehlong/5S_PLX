import { Component, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
// import { IonicModule } from '@ionic/angular';
import { IonAccordionGroup } from '@ionic/angular';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-survey',
  imports: [
    RouterModule,
    SharedModule
  ],
  templateUrl: './survey.component.html',
  styleUrls: ['./survey.component.scss'],
})
export class SurveyComponent implements OnInit {
  @ViewChild('accordionGroup', { static: true }) accordionGroup!: IonAccordionGroup;

  constructor() {
  }
  ngOnInit() { }

}
