import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
  imports: [SharedModule],
  standalone: true,
})
export class ChangePasswordComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
