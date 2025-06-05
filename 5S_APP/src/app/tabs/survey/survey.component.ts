import { Component, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { IonAccordionGroup } from '@ionic/angular';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-survey',
  imports: [RouterModule, SharedModule],
  templateUrl: './survey.component.html',
  styleUrls: ['./survey.component.scss'],
  standalone: true,
})
export class SurveyComponent implements OnInit {
  data: any[] = [
    {
      id: '03805572-e6b7-4455-90fe-9b6584eef46f',
      ma: 'KHAOSATCHXD',
      name: 'Chấm điểm 5S cửa hàng',
      moTa: 'Chấm điểm 5S',
      doiTuongId: 'DT1',
      image: '',
      state: 'Đang hoạt động',
      isActive: true,
    },
    {
      id: '16d30d78-0b80-4323-bd86-2498aae676a1',
      ma: 'KHAOSATHOXD',
      name: 'Chấm điểm kho',
      moTa: 'Chấm điểm kho xăng đàu',
      doiTuongId: 'DT2',
      image: '',
      state: 'Đang hoạt động',
      isActive: true,
    },
    {
      id: '97ac55ed-db17-457b-be3e-49b7f2130c36',
      ma: 'KHAOSATKHO',
      name: 'Chấm điểm Kho',
      moTa: 'Chấm điểm Kho Xăng Dầu',
      doiTuongId: 'DT2',
      image: '',
      state: 'Đang hoạt động',
      isActive: true,
    },
    {
      id: 'a27f5d72-fd07-4591-882b-22f829bcc464',
      ma: 'KSCUAHANG',
      name: 'Khảo sát Cửa hàng Xăng dầu',
      moTa: 'khảo sát',
      doiTuongId: 'DT1',
      image: '',
      state: 'Đang hoạt động',
      isActive: true,
    },
  ];

  @ViewChild('accordionGroup', { static: true })
  accordionGroup!: IonAccordionGroup;

  constructor(private router: Router) {}

  ngOnInit() {
    // this.getSurveyMgmt()
  }

  navigateTo(item: any) {
    const {doiTuongId, id} = item
    if (doiTuongId === 'DT1') {
      this.router.navigate([`/survey/store/list/${id}`]);
    } else if (doiTuongId === 'DT2') {
      this.router.navigate([`/survey/ware-house/${id}`]);
    }
  }

  // getSurveyMgmt() {
  //   this._service.getAllSurveyMgmt({}).subscribe({
  //     next: (data) => {
  //       this.data = data.data;
  //       console.log('data', data);
  //     },
  //   });
  // }
}
