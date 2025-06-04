import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  imports: [SharedModule, RouterModule],
  standalone: true
})
export class HomeComponent  implements OnInit {

  constructor(
    private router: Router,

  ) { }

  ngOnInit() {}

}
