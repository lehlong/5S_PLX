import { Component, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { KyKhaoSatService } from 'src/app/service/ky-khao-sat.service';
import { AppReportService } from 'src/app/service/app-report.service';
import { AuthService } from 'src/app/service/auth.service';
import { IonAccordionGroup } from '@ionic/angular';

@Component({
  selector: 'app-scoring-five-s',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  standalone: true,
  imports: [SharedModule],
})
export class ListComponent implements OnInit {

  @ViewChild('accordionGroup', { static: true }) accordionGroup!: IonAccordionGroup;


  filter: any = {
    filterKiKhaoSat: {},
    filterDoiTuong: {},
    filterNguoiCham: {},
    cuaHangToiCham: false,
    chuaCham: false,
  };
  inputSearchKiKhaoSat: any = {};
  searchNguoiCham: any = '';
  inSearchStore: any = '';
  selectValue = '1';
  lstAccout: any = [];
  doiTuong: any = '';
  lstSearchChamDiem: any = [];
  lstSearchDoiTuong: any = [];
  lstDoiTuong: any = [];
  lstKiKhaoSat: any = [];
  surveyId: any;
  filterForm!: FormGroup;
  isOpen = false;
  lstAccount: any = [];
  lstPointStore: any = [];
  user: any = {};
  searchKeyword: string = '';
  lstSearchDoiTuongFiltered: any[] = [];
  searchNguoiChamKeyword: string = '';
  lstSearchChamDiemFiltered: any[] = [];

  constructor(
    private _service: KyKhaoSatService,
    private _authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.user = JSON.parse(localStorage.getItem('UserInfo') ?? '');
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
    const account = this.lstAccout.find(
      (acc: any) => acc.userName === userName
    );
    return account?.fullName;
  }

  getAllKyKhaoSat() {
    this._service.search({ keyWord: this.surveyId }).subscribe({
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
          this.filter.filterKiKhaoSat.doiTuong = 'Cửa hàng';
          this.doiTuong = 'Cửa hàng';
          this.lstDoiTuong = data.lstInputStore;
          this.lstSearchDoiTuong = data.lstInputStore;
          return;
        } else if (data.lstInputWareHouse.length != 0) {
          this.filter.filterKiKhaoSat.doiTuong = 'Kho xăng dầu';
          this.doiTuong = 'Kho xăng dầu';
          this.lstDoiTuong = data.lstInputWareHouse;
          this.lstSearchDoiTuong = data.lstInputWareHouse;
        }
        this.lstSearchDoiTuongFiltered = [...this.lstSearchDoiTuong];
      },
      error: (response) => {
        console.log(response);
      },
    });
  }

  onFilter2() {
    if (this.selectValue === '1') {
      this.lstDoiTuong = this.lstDoiTuong.sort((a: any, b: any) =>
        a.name.localeCompare(b.name)
      );
    } else if (this.selectValue === '2') {
      this.lstDoiTuong = this.lstDoiTuong.sort(
        (a: any, b: any) => (b.point ?? 0) - (a.point ?? 0)
      );
    } else if (this.selectValue === '3') {
      this.lstDoiTuong = this.lstDoiTuong.sort(
        (a: any, b: any) => (a.point ?? 0) - (b.point ?? 0)
      );
    }
  }
  accordionValue: any

  openSelect(value: any) {
    this.accordionValue = value
  }

  searchDoiTuong(kiKhaoSat: any) {
    this.inputSearchKiKhaoSat = kiKhaoSat;
    this.inputSearchKiKhaoSat.doiTuong = this.doiTuong;
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
        this.lstSearchDoiTuongFiltered = [...this.lstSearchDoiTuong];
        this.lstSearchChamDiemFiltered = [...this.lstSearchChamDiem];

        this.accordionValue = null
      },
      error: (response) => {
        console.log(response);
      },
    });
  }
  selectSearchStore(item: any) {
    this.filter.filterDoiTuong = item;
    console.log('Selected store:', this.filter.filterDoiTuong);
    this.lstSearchChamDiem = item.lstInChamDiem;
    this.accordionValue = null
  }

  selectSearchChamDiem(item: any) {
    this.filter.filterNguoiCham = item;
    this.accordionValue = null
  }

  openFilterModal() {
    this.searchDoiTuong(this.filter.filterKiKhaoSat);
    this.isOpen = true;
  }

  closeFilterModal() {
    this.isOpen = false;
  }

  onFilter() {
    this.filter.filterKiKhaoSat = this.inputSearchKiKhaoSat;

    this.lstDoiTuong = this.lstSearchDoiTuong
      .filter(
        (s: any) =>
          !this.filter.filterDoiTuong?.id ||
          s.id == this.filter.filterDoiTuong?.id
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
      )
      .filter((s: any) => this.filter.chuaCham !== true || s?.point == 0);
    localStorage.setItem('filterLS', JSON.stringify(this.filter));
    this.closeFilterModal();
  }

  navigateTo(item: any) {
    localStorage.setItem(
      'filterCS',
      JSON.stringify({ kiKhaoSat: this.filter.filterKiKhaoSat, doiTuong: item })
    );
    this.router.navigate([`/survey/check-list/${item.id}`]);
  }

  resetFilters() {
    this.filter.filterKiKhaoSat = this.lstKiKhaoSat.reduce((a: any, b: any) =>
      new Date(a.endDate) > new Date(b.endDate) ? a : b
    );
    this.filter.filterDoiTuong.doiTuong = this.doiTuong;
    this.filter.filterDoiTuong = {};
    this.inputSearchKiKhaoSat = this.filter.filterKiKhaoSat;
    this.filter.filterNguoiCham = {};
    this.filter.inSearchStore = '';
    this.filter.searchNguoiCham = '';
    this.filter.cuaHangToiCham = false;
    localStorage.setItem('filterLS', JSON.stringify(this.filter));
  }

  //search cửa hàng
  onSearchChange() {
    const keyword = this.searchKeyword?.toLowerCase() || '';
    if (!keyword) {
      this.lstSearchDoiTuongFiltered = this.lstSearchDoiTuong;
    } else {
      this.lstSearchDoiTuongFiltered = this.lstSearchDoiTuong.filter(
        (item: any) => item.name.toLowerCase().includes(keyword)
      );
    }
  }
  onSearchNguoiChamChange() {
    const keyword = this.searchNguoiChamKeyword?.toLowerCase() || '';
    if (!keyword) {
      this.lstSearchChamDiemFiltered = [...this.lstSearchChamDiem];
    } else {
      this.lstSearchChamDiemFiltered = this.lstSearchChamDiem.filter((item: any) =>
        this.getFullName(item.userName)?.toLowerCase().includes(keyword)
      );
    }
  }
}
