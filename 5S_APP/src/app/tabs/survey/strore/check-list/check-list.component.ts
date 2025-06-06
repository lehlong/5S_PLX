import { Component, OnInit } from '@angular/core';
import { ButtonFilterComponent } from 'src/app/shared/button-filter/button-filter.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonActionSheet, IonButton } from '@ionic/angular/standalone';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-check-list',
  imports: [SharedModule, ButtonFilterComponent, IonActionSheet, IonButton],
  templateUrl: './check-list.component.html',
  styleUrls: ['./check-list.component.scss'],
})
export class CheckListComponent  implements OnInit {

  constructor() { }

  ngOnInit() {}

}
