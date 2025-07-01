import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/service/auth.service';
import { KyKhaoSatService } from 'src/app/service/ky-khao-sat.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { AppReportService } from 'src/app/service/app-report.service';
import { SurveyService } from 'src/app/service/survey.service';

@Component({
  selector: 'app-thiet-bi-cham-diem',
  templateUrl: './thiet-bi-cham-diem.component.html',
  styleUrls: ['./thiet-bi-cham-diem.component.scss'],
  imports: [SharedModule],
  standalone: true,
})
export class ThietBiChamDiemComponent implements OnInit {
  filter: any = {
    filterKiKhaoSat: {},
    filterDoiTuong: {},
    filterSurvey: {},
  };
  lstData: any = []
  lstSearchKiKhaoSat: any = [];
  lstSearchDoiTuong: any = [];
  lstKiKhaoSat: any = [];
  isOpen = false;
  lstSurvey: any = [];

  user: any = {};

  constructor(
    private _kyKhaoSatService: KyKhaoSatService,
    private _service: AppReportService,
    private _surveyService: SurveyService,
  ) { }

  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem('UserInfo') ?? '');
    this.getAllKyKhaoSat();
    this.getAllSurvey();
  }

  getReport() {
    this._service
      .ThietBiChamDiem({
        surveyId: this.filter.filterSurvey.doiTuongId,
        kiKhaoSatId: this.filter.filterKiKhaoSat.id,
        doiTuongId: this.filter.filterDoiTuong.id,
      })
      .subscribe({
        next: (data) => {
          console.log(data);
          this.lstData = data;
          this.isOpen = false;
        },
      });
  }
  getAllSurvey() {
    this._surveyService.getAllSurveyMgmt({}).subscribe({
      next: (data) => {
        this.lstSurvey = data.data;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  getAllKyKhaoSat() {
    this._kyKhaoSatService.search({ }).subscribe({
      next: (data) => {
        this.lstKiKhaoSat = data.data;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  selectSearchKyKhaoSat(item: any) {
    this.filter.filterSurvey = item;
    this.lstSearchKiKhaoSat = this.lstKiKhaoSat.filter((x: any) => x.surveyMgmtId == item.id);
    this.filter.filterKiKhaoSat = this.lstSearchKiKhaoSat.reduce((a: any, b: any) =>
      new Date(a.endDate) > new Date(b.endDate) ? a : b
    )
    this.filter.filterDoiTuong = {};
    console.log(this.filter.filterKiKhaoSat);
  }

  searchDoiTuong(kiKhaoSat: any) {
    this.filter.filterKiKhaoSat = kiKhaoSat;
    this._kyKhaoSatService.getInputKiKhaoSat(kiKhaoSat.id).subscribe({
      next: (data) => {
        if (data.lstInputStore.length != 0) {
          this.lstSearchDoiTuong = data.lstInputStore;
        } else if (data.lstInputWareHouse.length != 0) {
          this.lstSearchDoiTuong = data.lstInputWareHouse;
        }
        this.filter.filterDoiTuong = {};
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  selectSearchDoiTuong(item: any) {
    this.filter.filterDoiTuong = item;
  }

  openFilterModal() {
    this.isOpen = true;
  }

  closeFilterModal() {
    this.isOpen = false;
  }

  resetFilters() {
    this.filter.filterSurvey = {};
    this.filter.filterDoiTuong = {};
    this.filter.filterKiKhaoSat = {};
    // localStorage.setItem('filterLS', JSON.stringify(this.filter));
  }
}
