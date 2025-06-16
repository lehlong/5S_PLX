import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-terms-policies',
  templateUrl: './terms-policies.component.html',
  styleUrls: ['./terms-policies.component.scss'],
  standalone: true,
  imports: [SharedModule]
})
export class TermsPoliciesComponent  implements OnInit {

  constructor() { }

  ngOnInit() {}

}
