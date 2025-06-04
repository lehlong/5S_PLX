import { Component, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { IonAccordionGroup } from '@ionic/angular';

@Component({
  selector: 'app-survey',
  imports: [SharedModule],
  templateUrl: './survey.component.html',
  styleUrls: ['./survey.component.scss'],
})
export class SurveyComponent implements OnInit {
  @ViewChild('accordionGroup', { static: true }) accordionGroup!: IonAccordionGroup;

  constructor() {
  }
  ngOnInit() { }

  toggleAccordion = () => {
    const nativeEl = this.accordionGroup;
    if (nativeEl.value === 'second') {
      nativeEl.value = undefined;
    } else {
      nativeEl.value = 'second';
    }
  };
  treeData = [
    {
      label: 'First Accordion',
      value: 'first',
      children: [
        {
          label: 'First Child',
          value: 'a',
          children: [
            {
              label: 'First Child',
              value: 'd',

            },
            { label: 'Second Child', value: 'e' },
            { label: 'Third Child', value: 'f' },
          ],

        },
        { label: 'Second Child', value: 'b' },
        { label: 'Third Child', value: 'c' },
      ],
    },
    {
      label: 'Second Accordion',
      value: 'second',
    },
    {
      label: 'Third Accordion',
      value: 'third',
    },
  ];
}
