import { Component, OnInit } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { IonButton } from '@ionic/angular/standalone';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { KyKhaoSatService } from 'src/app/service/ky-khao-sat.service';
import { AppReportService } from 'src/app/service/app-report.service';
import { AuthService } from 'src/app/service/auth.service';

@Component({
  selector: 'app-scoring-five-s',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  standalone: true,
  imports: [SharedModule, IonButton],
})
export class ListComponent implements OnInit {

  filter: any = {
    filterKiKhaoSat: {},
    filterDoiTuong: {},
    filterNguoiCham: {},
    cuaHangToiCham: false,
    chuaCham: false
  }
  inputSearchKiKhaoSat: any = {};
  searchNguoiCham: any = '';
  inSearchStore: any = '';
  selectValue = '1';
  lstAccout: any = [];
  doiTuong: any = ''
  lstSearchChamDiem: any = [];
  lstSearchDoiTuong: any = [];
  lstDoiTuong: any = [];
  lstKiKhaoSat: any = [];
  surveyId: any;
  filterForm!: FormGroup;
  isOpen = false;
  lstAccount: any = []
  lstPointStore: any = []
  user: any = {}

  constructor(
    private _service: KyKhaoSatService,
    private _authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem('UserInfo') ?? "")
    this.route.paramMap.subscribe({
      next: (params) => {
        const id = params.get('id');
        this.surveyId = id;
        this.getAllKyKhaoSat();
      },
    });
    this.getAllAccount();
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
    const account = this.lstAccout.find((acc: any) => acc.userName === userName);
    return account?.fullName;
  }

  getAllKyKhaoSat() {
    this._service
      .search({ keyWord: this.surveyId })
      .subscribe({
        next: (data) => {
          this.lstKiKhaoSat = data.data;
          console.log("lstKiKhaoSat", this.lstKiKhaoSat);

          const filter = localStorage.getItem('filterLS') ?? ""
          const filter2 = data.data.reduce((a: any, b: any) =>
            new Date(a.endDate) > new Date(b.endDate) ? a : b
          );
          if (filter != "") {
            this.filter = JSON.parse(filter)
            if (this.filter.filterKiKhaoSat.surveyMgmtId != this.surveyId) {
              this.filter.filterKiKhaoSat = filter2
            }
          } else {
            this.filter.filterKiKhaoSat = filter2
          }

          this.getAllDoiTuong();

          this.inputSearchKiKhaoSat = this.filter.filterKiKhaoSat;
        },
        error: (response) => {
          console.log(response);
        },
      });
  }

  getAllDoiTuong() {
    this._service.getInputKiKhaoSat(this.filter.filterKiKhaoSat.id).subscribe({
      next: (data) => {
        if (data.lstInputStore.length != 0) {
          this.doiTuong = 'cửa hàng'
          this.lstDoiTuong = data.lstInputStore;
          this.lstSearchDoiTuong = data.lstInputStore;
          return;
        } else if (data.lstInputWareHouse.length != 0) {
          this.doiTuong = 'kho xăng dầu'
          this.lstDoiTuong = data.lstInputWareHouse;
          this.lstSearchDoiTuong = data.lstInputWareHouse;
        }
      },
      error: (response) => {
        console.log(response);
      },
    });
  }


  filterPoint(inStoreId: any) {
    const record = this.lstPointStore.find((x: any) => x.inStoreId === inStoreId);

    return record?.point ?? 0;
  }

  // getAllAccount() {
  //   this._authService.(this.filter.filterKiKhaoSat.id).subscribe({
  //     next: (data) => {
  //       this.lstDoiTuong = data.lstInputStore;
  //       this.lstSearchDoiTuong = data.lstInputStore;
  //     },
  //     error: (response) => {
  //       console.log(response);
  //     },
  //   });
  // }

  onFilter2() {
    if (this.selectValue === '1') {
      this.lstDoiTuong = this.lstDoiTuong.sort((a: any, b: any) => a.name.localeCompare(b.name));
    } else if (this.selectValue === '2') {
      this.lstDoiTuong = this.lstDoiTuong.sort((a: any, b: any) => (b.point ?? 0) - (a.point ?? 0));
    } else if (this.selectValue === '3') {
      this.lstDoiTuong = this.lstDoiTuong.sort((a: any, b: any) => (a.point ?? 0) - (b.point ?? 0));
    }
  }



  searchDoiTuong(kiKhaoSat: any) {
    this.inputSearchKiKhaoSat = kiKhaoSat;
    this._service.getInputKiKhaoSat(kiKhaoSat.id).subscribe({
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
  selectSearchStore(item: any) {
    this.filter.filterDoiTuong = item;
    console.log('Selected store:', this.filter.filterDoiTuong);
    this.lstSearchChamDiem = item.lstInChamDiem
  }

  selectSearchChamDiem(item: any) {
    this.filter.filterNguoiCham = item
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

    this.lstDoiTuong = this.lstSearchDoiTuong
      .filter((s: any) =>
        !this.filter.filterDoiTuong?.id || s.id == this.filter.filterDoiTuong?.id)
      .filter((s: any) =>
        !this.filter.filterNguoiCham?.userName || s.lstChamDiem?.some((x: any) => x == this.filter.filterNguoiCham.userName))
      .filter((s: any) =>
        this.filter.cuaHangToiCham !== true || s.lstChamDiem?.some((x: any) => x == this.user.userName))
      .filter((s: any) =>
        this.filter.chuaCham !== true || s?.point == 0)
    localStorage.setItem('filterLS', JSON.stringify(this.filter))
    this.closeFilterModal();
  }


  navigateTo(item: any) {
    localStorage.setItem('filterCS', JSON.stringify({ kiKhaoSat: this.filter.filterKiKhaoSat, doiTuong: item }))
    this.router.navigate([`/survey/check-list/${item.id}`]);
  }

  resetFilters() {
    this.filter.filterKiKhaoSat = this.lstKiKhaoSat.reduce((a: any, b: any) =>
      new Date(a.endDate) > new Date(b.endDate) ? a : b
    );
    this.filter.filterDoiTuong = {}
    this.inputSearchKiKhaoSat = this.filter.filterKiKhaoSat
    this.filter.filterNguoiCham = {}
    this.filter.inSearchStore = '';
    this.filter.searchNguoiCham = '';
    this.filter.cuaHangToiCham = false;
    localStorage.setItem('filterLS', JSON.stringify(this.filter))

  }
}
