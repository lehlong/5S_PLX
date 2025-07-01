import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AppReportService } from 'src/app/service/app-report.service';
import { AuthService } from 'src/app/service/auth.service';
import { KyKhaoSatService } from 'src/app/service/ky-khao-sat.service';
import { SurveyService } from 'src/app/service/survey.service';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-cham-diem-theo-khung-thoi-gian',
  templateUrl: './cham-diem-theo-khung-thoi-gian.component.html',
  styleUrls: ['./cham-diem-theo-khung-thoi-gian.component.scss'],
  standalone: true,
  imports: [SharedModule],
})
export class ChamDiemTheoKhungThoiGianComponent implements OnInit {
  lstData: any[] = [];
  filter: any = {
    filterKiKhaoSat: {},
    filterDoiTuong: {},
    filterSurvey: {},
  };
  inputSearchKiKhaoSat: any = {};

  searchNguoiCham: any = '';
  inSearchStore: any = '';
  selectValue = '1';
  lstStore: any;
  lstAccout: any = [];
  lstSearchChamDiem: any = [];
  lstSearchStore: any = [];
  lstKiKhaoSat: any = [];
  surveyId: any;
  filterForm!: FormGroup;
  isOpen = false;
  lstAccount: any = [];
  lstSurvey: any = [];
  lstSearchKiKhaoSat: any = [];
  lstSearchDoiTuong: any = [];

  user: any = {};

  constructor(
    private _service: AppReportService,
    private _kyKhaoSatService: KyKhaoSatService,
    private _authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private _surveyService: SurveyService
  ) {}

  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem('UserInfo') ?? '');
    this.getAllAccount();
    this.route.paramMap.subscribe({
      next: (params) => {
        const id = params.get('id');
        this.surveyId = id;
        this.getAllKyKhaoSat();
        this.getAllSurvey()
      },
    });
  }
  getReport() {
    this._service
      .TheoKhungThoiGian({
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
  // getDataTheoKhungThoiGian() {
  //   this._service
  //     .TheoKhungThoiGian({
  //       kiKhaoSatId: this.filter.filterKiKhaoSat.id,
  //       InstoreId: this.filter.filterDoiTuong.id,
  //       AccountUserName: this.filter.filterNguoiCham,
  //       SurveyId: this.filter.cuaHangToiCham,
  //     })
  //     .subscribe({
  //       next: (data) => {
  //         this.lstData = data;
  //         console.log(data);
  //         console.log('store', this.filter.filterStore.id);
  //         console.log('AccountUserName', this.filter.filterNguoiCham);
  //         console.log('SurveyId', this.filter.cuaHangToiCham);
  //       },
  //     });
  // }

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
  getAllAccount() {
    this._authService.GetAllAccount().subscribe({
      next: (data) => {
        this.lstAccout = data;
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  getFullName(userName: string): string {
    const account = this.lstAccout.find(
      (acc: any) => acc.userName === userName
    );
    return account?.fullName;
  }

  // getAllAccount() {
  //   this._authService.(this.filter.filterKiKhaoSat.id).subscribe({
  //     next: (data) => {
  //       this.lstStore = data.lstInputStore;
  //       this.lstSearchStore = data.lstInputStore;
  //     },
  //     error: (response) => {
  //       console.log(response);
  //     },
  //   });
  // }

  searchStore(kiKhaoSat: any) {
    this.inputSearchKiKhaoSat = kiKhaoSat;
    this._kyKhaoSatService.getInputKiKhaoSat(kiKhaoSat.id).subscribe({
      next: (data) => {
        this.lstSearchStore = data.lstInputStore;
        this.lstSearchChamDiem = Array.from(
          new Map(
            this.lstSearchStore
              .flatMap((store: any) => store.lstInChamDiem || [])
              .map((item: any) => [item.userName, item])
          ).values()
        );
      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  selectSearchStore(item: any) {
    this.filter.filterStore = item;
    console.log('Selected store:', this.filter.filterStore);
    this.lstSearchChamDiem = item.lstInChamDiem;
  }

  selectSearchChamDiem(item: any) {
    this.filter.filterNguoiCham = item;
  }

  openFilterModal() {
    this.searchStore(this.filter.filterKiKhaoSat);
    this.isOpen = true;
  }

  closeFilterModal() {
    this.isOpen = false;
  }

  // onFilter() {
  //   this.filter.filterKiKhaoSat = this.inputSearchKiKhaoSat;
  //   this.lstStore = this.lstSearchStore
  //     .filter(
  //       (s: any) =>
  //         !this.filter.filterStore?.id || s.id == this.filter.filterStore?.id
  //     )
  //     .filter(
  //       (s: any) =>
  //         !this.filter.filterNguoiCham?.userName ||
  //         s.lstChamDiem?.some(
  //           (x: any) => x == this.filter.filterNguoiCham.userName
  //         )
  //     )
  //     .filter(
  //       (s: any) =>
  //         this.filter.cuaHangToiCham !== true ||
  //         s.lstChamDiem?.some((x: any) => x == this.user.userName)
  //     );
  //   localStorage.setItem('filterLS', JSON.stringify(this.filter));
  //   this.closeFilterModal();
  //   this.getReport();
  // }

   resetFilters() {
    this.filter.filterSurvey = {};
    this.filter.filterDoiTuong = {};
    this.filter.filterKiKhaoSat = {};
    // localStorage.setItem('filterLS', JSON.stringify(this.filter));
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

  selectSearchKyKhaoSat(item: any) {
    this.filter.filterSurvey = item;
    this.lstSearchKiKhaoSat = this.lstKiKhaoSat.filter(
      (x: any) => x.surveyMgmtId == item.id
    );
    this.filter.filterKiKhaoSat = this.lstSearchKiKhaoSat.reduce(
      (a: any, b: any) => (new Date(a.endDate) > new Date(b.endDate) ? a : b)
    );
    this.filter.filterDoiTuong = {};
    console.log(this.filter.filterKiKhaoSat);
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
}
