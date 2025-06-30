import { IonButton } from '@ionic/angular/standalone';
import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/service/auth.service';
import { KyKhaoSatService } from 'src/app/service/ky-khao-sat.service';
import { SharedModule } from 'src/app/shared/shared.module';
import { AppReportService } from 'src/app/service/app-report.service';

@Component({
  selector: 'app-ket-qua-cham-diem',
  templateUrl: './ket-qua-cham-diem.component.html',
  styleUrls: ['./ket-qua-cham-diem.component.scss'],
  imports: [SharedModule, IonButton],
  standalone: true,
})
export class KetQuaChamDiemComponent implements OnInit {
  filter: any = {
    filterKiKhaoSat: {},
    filterDoiTuong: {},
    filterNguoiCham: {},
    cuaHangToiCham: false,
  };
  inputSearchKiKhaoSat: any = {};

  searchNguoiCham: any = '';
  inSearchDoiTuong: any = '';
  selectValue = '1';
  lstData: any = [  ]
  lstAccout: any = [];
  lstSearchChamDiem: any = [];
  lstSearchDoiTuong: any = [];
  lstKiKhaoSat: any = [];
  surveyId: any;
  filterForm!: FormGroup;
  isOpen = false;
  lstAccount: any = [];

  user: any = {};

  constructor(
    private _kyKhaoSatService: KyKhaoSatService,
    private _service : AppReportService,
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

  getReport(){
    this._service.KetQuaChamDiem({kiKhaoSatId: this.filter.filterKiKhaoSat.id}).subscribe({
      next: (data)=> {
        console.log(data);
        this.lstData = data
      }
    })
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
        this.getReport()
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
  //       this.lstSearchDoiTuong = data.lstInputStore;
  //     },
  //     error: (response) => {
  //       console.log(response);
  //     },
  //   });
  // }

  searchDoiTuong(kiKhaoSat: any) {
    this.inputSearchKiKhaoSat = kiKhaoSat;
    this._kyKhaoSatService.getInputKiKhaoSat(kiKhaoSat.id).subscribe({
      next: (data) => {
        if (data.lstInputStore.length != 0) {
          this.lstSearchDoiTuong = data.lstInputStore;
          this.lstSearchChamDiem = Array.from(
            new Map(
              this.lstSearchDoiTuong
                .flatMap((store: any) => store.lstInChamDiem || [])
                .map((item: any) => [item.userName, item])
            ).values()
          );
        } else if (data.lstInputWareHouse.length != 0) {
            this.lstSearchDoiTuong = data.lstInputWareHouse;
            this.lstSearchChamDiem = Array.from(
              new Map(
                this.lstSearchDoiTuong
                  .flatMap((wareHouse: any) => wareHouse.lstInChamDiem || [])
                  .map((item: any) => [item.userName, item])
              ).values()
            );
        }
      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  selectSearchDoiTuong(item: any) {
    this.filter.filterDoiTuong = item;
    console.log('Selected store:', this.filter.filterStore);
    this.lstSearchChamDiem = item.lstInChamDiem;
  }

  selectSearchChamDiem(item: any) {
    this.filter.filterNguoiCham = item;
  }

  openFilterModal() {
    this.searchDoiTuong(this.filter.filterKiKhaoSat);
    this.isOpen = true;
  }

  closeFilterModal() {
    this.isOpen = false;
  }

  onFilter() {
    this.filter.filterKiKhaoSat = this.inputSearchKiKhaoSat
    this.lstData = this.lstSearchDoiTuong
      .filter((s: any) => !this.filter.filterStore?.id || s.id == this.filter.filterStore?.id)
      .filter((s: any) => !this.filter.filterNguoiCham?.userName ||
        s.lstChamDiem?.some((x: any) => x == this.filter.filterNguoiCham.userName))
      .filter((s: any) => this.filter.cuaHangToiCham !== true || s.lstChamDiem?.some((x: any) => x == this.user.userName))
    localStorage.setItem('filterLS', JSON.stringify(this.filter))
    this.getReport()
    this.closeFilterModal();
  }

  resetFilters() {
    this.filter.filterKiKhaoSat = this.lstKiKhaoSat.reduce((a: any, b: any) =>
      new Date(a.endDate) > new Date(b.endDate) ? a : b
    );
    this.filter.filterStore = {};
    this.inputSearchKiKhaoSat = this.filter.filterKiKhaoSat;
    this.filter.filterNguoiCham = {};
    this.filter.inSearchDoiTuong = '';
    this.filter.searchNguoiCham = '';
    this.filter.cuaHangToiCham = false;
    localStorage.setItem('filterLS', JSON.stringify(this.filter));
  }
}
