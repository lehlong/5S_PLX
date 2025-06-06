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
export class CheckListComponent implements OnInit {
  fakeApi: any[] = [
    {
      lanCham: 'Lần chấm thứ 1',
      diem: '100 điểm',
      nguoiCham: 'Lê Thị Lệ Thuý',
      date: '07-03-2025 07:53',
    },
    {
      lanCham: 'Lần chấm thứ 2',
      diem: '99 điểm',
      nguoiCham: 'Nguyễn Thị Lân CH17',
      date: '12-03-2025 07:53',
    },
    {
      lanCham: 'Lần chấm thứ 3',
      diem: '99 điểm',
      nguoiCham: 'Lê Thị Lệ Thuý',
      date: '18-03-2025 07:53',
    },
    {
      lanCham: 'Lần chấm thứ 4',
      diem: '100 điểm',
      nguoiCham: 'Nguyễn Thị Lân CH17',
      date: '25-03-2025 07:53',
    },
  ];
  constructor(private router: Router) {}

  ngOnInit() {}
  navigateTo() {
    this.router.navigate([`survey/store/evaluate`]);
  }
}
