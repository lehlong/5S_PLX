import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AppReportService } from 'src/app/service/app-report.service';
import { AuthService } from 'src/app/service/auth.service';
import { KyKhaoSatService } from 'src/app/service/ky-khao-sat.service';
import { SharedModule } from 'src/app/shared/shared.module';

@Component({
  selector: 'app-thoi-gian-cham-diem',
  templateUrl: './thoi-gian-cham-diem.component.html',
  styleUrls: ['./thoi-gian-cham-diem.component.scss'],
  imports: [SharedModule],
  standalone: true,
})
export class ThoiGianChamDiemComponent implements OnInit {
  filter: any = {
    filterKiKhaoSat: {},
    filterStore: {},
    filterNguoiCham: {},
    cuaHangToiCham: false,
  };
  inputSearchKiKhaoSat: any = {};

  searchNguoiCham: any = '';
  inSearchStore: any = '';
  selectValue = '1';
  lstData: any[] = [];
  lstAccout: any = [];
  lstSearchChamDiem: any = [];
  lstSearchStore: any = [];
  lstKiKhaoSat: any = [];
  surveyId: any;
  filterForm!: FormGroup;
  isOpen = false;
  lstAccount: any = [];

  user: any = {};

  constructor(
    private _service: AppReportService,
    private _kyKhaoSatService: KyKhaoSatService,
    private _authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem('UserInfo') ?? '');
    this.route.paramMap.subscribe({
      next: (params) => {
        const id = params.get('id');
        this.surveyId = id;
        this.getAllKyKhaoSat();
      },
    });
  }

  getThoiGianChamDiem() {
    this._service
      .ThoiGianChamDiem({
        kiKhaoSatId: this.filter.filterKiKhaoSat.id,
        InstoreId: this.filter.filterStore.id,
        AccountUserName: this.filter.filterNguoiCham,
        SurveyId: this.filter.cuaHangToiCham
      })
      .subscribe({
        next: (data) => {
          this.lstData = data;
          console.log(data);
        },
      });
  }
  getAllKyKhaoSat() {
    this._kyKhaoSatService.search({ keyWord: this.surveyId }).subscribe({
      next: (data) => {
        this.lstKiKhaoSat = data.data;

        const filter = localStorage.getItem('filterLS') ?? '';
        const filter2 = data.data.reduce((a: any, b: any) =>
          new Date(a.endDate) > new Date(b.endDate) ? a : b
        );
        if (filter != '') {
          this.filter = JSON.parse(filter);
          if (this.filter.filterKiKhaoSat.surveyMgmtId != this.surveyId) {
            this.filter.filterKiKhaoSat = filter2;
          }
        } else {
          this.filter.filterKiKhaoSat = filter2;
        }

        this.inputSearchKiKhaoSat = this.filter.filterKiKhaoSat;
        this.getThoiGianChamDiem();
      },
      error: (response) => {
        console.log(response);
      },
    });
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

  onFilter() {
    this.filter.filterKiKhaoSat = this.inputSearchKiKhaoSat;
    this.lstData = this.lstSearchStore
      .filter(
        (s: any) =>
          !this.filter.filterStore?.id || s.id == this.filter.filterStore?.id
      )
      .filter(
        (s: any) =>
          !this.filter.filterNguoiCham?.userName ||
          s.lstChamDiem?.some(
            (x: any) => x == this.filter.filterNguoiCham.userName
          )
      )
      .filter(
        (s: any) =>
          this.filter.cuaHangToiCham !== true ||
          s.lstChamDiem?.some((x: any) => x == this.user.userName)
      );
    localStorage.setItem('filterLS', JSON.stringify(this.filter));
    this.getThoiGianChamDiem();
    this.closeFilterModal();
  }

  resetFilters() {
    this.filter.filterKiKhaoSat = this.lstKiKhaoSat.reduce((a: any, b: any) =>
      new Date(a.endDate) > new Date(b.endDate) ? a : b
    );
    this.filter.filterStore = {};
    this.inputSearchKiKhaoSat = this.filter.filterKiKhaoSat;
    this.filter.filterNguoiCham = {};
    this.filter.inSearchStore = '';
    this.filter.searchNguoiCham = '';
    this.filter.cuaHangToiCham = false;
    localStorage.setItem('filterLS', JSON.stringify(this.filter));
  }
}
